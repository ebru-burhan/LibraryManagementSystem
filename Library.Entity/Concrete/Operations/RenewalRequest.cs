
using Library.Entity.Abstract;

namespace Library.Entity.Concrete.Operations;

// 4. Süre Uzatma Talepleri
public class RenewalRequest : CreationAuditedEntity
{
    public int LoanId { get; set; }
    public Loan Loan { get; set; } = null!;

    public DateTime NewDueDate { get; set; } // İstenen yeni tarih
    public bool IsApproved { get; set; } = false;
}