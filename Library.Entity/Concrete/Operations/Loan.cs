using Library.Entity.Abstract;
using Library.Entity.Concrete.Membership;
using Library.Entity.Concrete.Catalog;
using Library.Entity.Concrete.Lookups;

namespace Library.Entity.Concrete.Operations;

//Ödünç alma
public class Loan : AuditableEntity
{
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int BookCopyId { get; set; }
    public BookCopy BookCopy { get; set; } = null!;

    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; } // Beklenen teslim tarihi
    public DateTime? ReturnDate { get; set; } // Gerçekleşen iade tarihi

    public int StatusId { get; set; }
    public LoanStatus Status { get; set; } = null!;

    public ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
    public ICollection<RenewalRequest> RenewalRequests { get; set; } = new List<RenewalRequest>();
}
