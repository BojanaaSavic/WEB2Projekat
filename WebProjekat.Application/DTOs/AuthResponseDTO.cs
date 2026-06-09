using System;

namespace WebProjekat.Application.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}