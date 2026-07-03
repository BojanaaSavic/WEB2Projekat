using System.ComponentModel.DataAnnotations;

namespace WebProjekat.Application.DTOs
{
    /// <summary>
    /// User registration information
    /// </summary>
    public class RegisterDTO
    {
        /// <summary>
        /// User's full name
        /// </summary>
        /// <example>John Doe</example>
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// User's email address (must be unique)
        /// </summary>
        /// <example>john.doe@example.com</example>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; }

        /// <summary>
        /// Password for the account (min 8 characters, must contain uppercase, lowercase, number, and special character)
        /// </summary>
        /// <example>SecurePassword123!</example>
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 256 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character (@$!%*?&).")]
        public string Password { get; set; }
    }
}