namespace Library.Model.Dtos.Operations;


public class CreateLoanDto
{
    // Dış dünya int ID'leri bilmeyeceği için Guid (ExternalId) alıyoruz
    public Guid MemberId { get; set; }
    //katalogdan veya bir listeden (dropdown) kitap seçip ödünç vermek istediğimizde
    public Guid? BookCopyId { get; set; } 
    public string? Barcode { get; set; }

    // Gerçek hayatta kütüphaneci barkod okutur. ama sadece barkod olsa admin falan ödünç veremez atayamaz sanki seçip hnagisi olursa diye ikisi de nulable


    public DateTime? LoanDate { get; set; }
}

public class LoanListDto : BaseExternalDto
{
    public string MemberFullName { get; set; } = null!;
    public string MemberNumber { get; set; } = null!;

    public string BookTitle { get; set; } = null!;
    public string Barcode { get; set; } = null!;

    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public string Status { get; set; } = null!;
    public bool IsOverdue { get; set; }
    public int DelayDays { get; set; }
}

// Detay ve İade
public class LoanDetailDto : LoanListDto
{
    public string? MemberEmail { get; set; }
    public string? MemberPhone { get; set; }

    // Eğer iade gecikmişse, anlık hesaplanacak ceza tutarı (Modül 9'a hazırlık)
    public decimal CurrentPenaltyAmount { get; set; }
}

public class ReturnLoanDto
{
    public bool IsDamaged { get; set; }
    public decimal? DamageAmount { get; set; }
}