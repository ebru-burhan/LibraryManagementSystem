namespace Library.Model.Dtos.Members.MemberDtos;

// 1. Temel Listeleme (Hafif Veri - Sadece Tabloda / Grid'de Gösterilecekler)
// BaseExternalDto'dan miras aldığı için Guid ExternalId otomatik gelir.
public class MemberListDto : BaseExternalDto
{
    public string MemberNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Status { get; set; } = null!;
    public string StatusName { get; set; } = null!;
}

public class MemberDetailDto : MemberListDto
{

    // Üyelik tarihi (ChangeTracker'ın otomatik atadığı Auditable alan)
    public DateTime CreatedAt { get; set; }
    public string? Address { get; set; }
    public string? PictureUrl { get; set; }
    public decimal UnpaidDebtAmount { get; set; }

    // Üyenin alt koleksiyonları
    public List<MemberLoanDto> ActiveLoans { get; set; } = new();
    public List<MemberReservationDto> Reservations { get; set; } = new();
    public List<MemberPenaltyDto> Penalties { get; set; } = new();
}

// Üye Alt Koleksiyonları İçin DTO'lar 

public class MemberLoanDto : BaseExternalDto
{
    public string BookTitle { get; set; } = null!;
    public string? Authors { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = null!;
    public bool IsOverdue { get; set; }
}

public class MemberReservationDto : BaseExternalDto
{
    public string BookTitle { get; set; } = null!;
    public string? Authors { get; set; }
    public int QueueNumber { get; set; }
    public DateTime ReservationDate { get; set; }
    public string Status { get; set; } = null!;
}

public class MemberPenaltyDto : BaseExternalDto
{
    public string Reason { get; set; } = null!;
    public string? RelatedBookTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public int DelayDays { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
}

// 4. Analitik ve Dizin Yönetimi (Dashboard / Sayfalama özeti)
public class MemberDirectoryDto
{
    public List<MemberListDto> Members { get; set; } = new();
    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public int PassiveCount { get; set; }
    public int SuspendedCount { get; set; }
}

// 5. Güncelleme (Update) İşlemleri
public class UpdateMemberStatusDto
{
    public string StatusCode { get; set; } = null!;
}