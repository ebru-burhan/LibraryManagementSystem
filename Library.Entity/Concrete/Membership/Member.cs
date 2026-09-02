using Library.Entity.Abstract;
using Library.Entity.Concrete.Auth;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Operations;

namespace Library.Entity.Concrete.Membership;

public class Member : AuditableEntity
{
    public string MemberNumber { get; set; } = null!; // Kütüphanenin atadığı özel numara (Örn: LUM-2023-001)

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Üye kaydı, onaylanan başvurudan doğar (bire-bir). FK Member tarafında durur.
    public int MembershipApplicationId { get; set; }
    public MembershipApplication MembershipApplication { get; set; } = null!;

    public int StatusId { get; set; }
    public MemberStatus Status { get; set; } = null!;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
}