using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebProjekat.Application.DTOs;
using WebProjekat.Application.Interfaces;
using WebProjekat.Domain.Entities;
using WebProjekat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace WebProjekat.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _hasher = new();

        public UserService(UserDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            if (!IsValidEmail(dto.Email))
                throw new ArgumentException("Invalid email address format.");

            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existing != null) 
                throw new ArgumentException("Email already registered.");

            var passwordValidation = ValidatePasswordStrength(dto.Password);
            if (!passwordValidation.IsValid)
                throw new ArgumentException(passwordValidation.ErrorMessage);

            var user = new User(dto.Name, dto.Email, dto.Password);
            var hashed = _hasher.HashPassword(user, dto.Password);
            user.SetPasswordHash(hashed);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateToken(user);
            return new AuthResponseDTO { Token = token, UserId = user.Id, Email = user.Email, Name = user.Name, Role = user.Role };
        }

        public async Task<AuthResponseDTO> AuthenticateAsync(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return null;

            var result = _hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (result == PasswordVerificationResult.Failed) return null;

            var token = GenerateToken(user);
            return new AuthResponseDTO { Token = token, UserId = user.Id, Email = user.Email, Name = user.Name, Role = user.Role };
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private (bool IsValid, string ErrorMessage) ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password cannot be empty.");

            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]"))
                return (false, "Password must contain at least one lowercase letter.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]"))
                return (false, "Password must contain at least one uppercase letter.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
                return (false, "Password must contain at least one digit.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[@$!%*?&]"))
                return (false, "Password must contain at least one special character (@$!%*?&).");

            return (true, "");
        }

        private string GenerateToken(User user)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiresDays = int.TryParse(_configuration["Jwt:ExpiresDays"], out var d) ? d : 7;

            if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("JWT Key not configured.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddDays(expiresDays),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}