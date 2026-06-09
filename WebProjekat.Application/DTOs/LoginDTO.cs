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
        public string Email { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        /// <example>SecurePassword123!</example>
        public string Password { get; set; }
    }
}