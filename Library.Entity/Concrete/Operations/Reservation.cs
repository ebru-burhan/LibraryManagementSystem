
using Library.Entity.Abstract;
using Library.Entity.Concrete.Catalog;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Membership;

namespace Library.Entity.Concrete.Operations;

public class Reservation : AuditableEntity
{
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int BookId { get; set; }
    public Book Book { get; set; } = null!; // Belli bir kopyaya değil, kitaba rezervasyon yapılır

    public DateTime ReservationDate { get; set; }
    public int QueueNumber { get; set; } // Sıra numarası
    public int StatusId { get; set; }
    public ReservationStatus Status { get; set; } = null!;
}