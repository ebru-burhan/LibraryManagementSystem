namespace Library.Model.Dtos.Operations;

// 1. Kütüphaneciden Gelecek İstek (Request)
public class ReportLostBookDto
{
    public Guid LoanExternalId { get; set; }
    public decimal BookValue { get; set; }
}

// 2. Üstteki KPI Kartları İçin (Dashboard Özet)
public class LostBookKpiDto
{
    public int TotalLostBooks { get; set; }
    public decimal TotalUnpaidAmount { get; set; }
    public int MonthlyReports { get; set; }
}

// 3. Alt Tablo İçin Liste Modeli (Response)
public class LostBookListDto : BaseExternalDto
{
    public string MemberFullName { get; set; } = null!;
    public string MemberNumber { get; set; } = null!;
    public string BookTitle { get; set; } = null!;
    public string Isbn { get; set; } = null!;
    public DateTime DeclaredDate { get; set; }
    public decimal BookValue { get; set; }
    public bool IsResolved { get; set; } // Ödendi / Bekliyor durumu
}