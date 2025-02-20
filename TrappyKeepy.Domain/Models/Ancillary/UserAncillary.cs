﻿namespace TrappyKeepy.Domain.Models
{
    public interface IUserDto
    {
        Guid? Id { get; }
        string? Name { get; }
        string? Password { get; }
        string? Email { get; }
        string? Role { get; }
        DateTime? DateCreated { get; }
        DateTime? DateActivated { get; }
        DateTime? DateLastLogin { get; }
    }

    public class UserDto : IUserDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateActivated { get; set; }
        public DateTime? DateLastLogin { get; set; }
    }

    public interface IUserComplexDto
    {
        Guid? Id { get; }
        string? Name { get; }
        string? Password { get; }
        string? Email { get; }
        string? Role { get; }
        DateTime? DateCreated { get; }
        DateTime? DateActivated { get; }
        DateTime? DateLastLogin { get; }
        ICollection<Keeper>? Keepers { get; }
        ICollection<Membership>? Memberships { get; }
        ICollection<Permit>? Permits { get; }
    }

    public class UserComplexDto : IUserComplexDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateActivated { get; set; }
        public DateTime? DateLastLogin { get; set; }
        public virtual ICollection<Keeper>? Keepers { get; set; }
        public virtual ICollection<Membership>? Memberships { get; set; }
        public virtual ICollection<Permit>? Permits { get; set; }
    }

    public interface IUserSessionDto
    {
        string Email { get; }
        string Password { get; }
    }

    public class UserSessionDto : IUserSessionDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public UserSessionDto(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
