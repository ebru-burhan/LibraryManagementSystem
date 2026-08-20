using Library.Entity.Abstract;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Membership;

namespace Library.Entity.Concrete.Operations;
// Ceza 
public class Penalty : AuditableEntity
{
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    // Ceza bir ödünç alma (gecikme) işlemine veya kayıp kitap durumuna bağlı olabilir
    public int? LoanId { get; set; }
    public Loan? Loan { get; set; }

    //ceza tutarı loan ise teslim ve beklenen teslim tarihi ordan günü bulup gecikme ücreti ile çarpıp falan filan
    //eğer kayıpkitap ise lostbook ta bookValue
    public decimal Amount { get; set; } // Ceza tutarı  
    public bool IsPaid { get; set; } = false; // Ödendi mi?
    public DateTime? PaidDate { get; set; }


    public int PenaltyTypeId { get; set; }
    public PenaltyType PenaltyType { get; set; } = null!;
}