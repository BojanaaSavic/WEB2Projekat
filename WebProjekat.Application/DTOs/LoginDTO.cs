using System.ComponentModel.DataAnnotations;

namespace WebProjekat.Application.DTOs
{
    /// <summary>
    /// Login credentials for user authentication
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// User email address
        /// </summary>
        /// <example>user@example.com</example>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        /// <example>SecurePassword123!</example>
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}