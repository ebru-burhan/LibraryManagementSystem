using Library.Entity.Abstract;
using Library.Entity.Concrete.Auth;
using Library.Entity.Concrete.Operations;

namespace Library.Entity.Concrete.Membership;

public class Member : AuditableEntity
{
    public string MemberNumber { get; set; } = null!; // Kütüphanenin atadığı özel numara (Örn: LUM-2023-001)

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
}