using System;

namespace Auth.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Salt { get; private set; }
        public bool IsLocked { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private User() { }

        public User(Guid id, string username, string email, string passwordHash, string salt)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Salt = salt ?? throw new ArgumentNullException(nameof(salt));
            IsLocked = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Lock() => IsLocked = true;
        public void Unlock() => IsLocked = false;
        public void UpdateEmail(string email) {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
