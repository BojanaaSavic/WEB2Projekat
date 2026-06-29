using System;
using System.Collections.Generic;

namespace WebProjekat.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }

        private User() { }

        public User(string name, string email, string password, string role = "User")
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required.", nameof(name));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email required.", nameof(email));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password required.", nameof(password));

            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("PasswordHash required.", nameof(passwordHash));
            Password = passwordHash;
        }
    }
}