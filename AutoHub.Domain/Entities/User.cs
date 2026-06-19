using AutoHub.Domain.Enums;

namespace AutoHub.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Dealer? Dealer { get; set; }

    public ICollection<Favourite> Favourites { get; set; } = [];

    public ICollection<Inquiry> Inquiries { get; set; }
    = [];
    public ICollection<Reservation> Reservations { get; set; }
    = [];
}