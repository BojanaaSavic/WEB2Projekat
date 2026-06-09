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
        public string Name { get; set; }

        /// <summary>
        /// User's email address (must be unique)
        /// </summary>
        /// <example>john.doe@example.com</example>
        public string Email { get; set; }

        /// <summary>
        /// Password for the account
        /// </summary>
        /// <example>SecurePassword123!</example>
        public string Password { get; set; }
    }
}