namespace Library.Model.Dtos.Members;

public class MemberLoanDto
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = null!;
    public string? Authors { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = null!;
    public bool IsOverdue { get; set; }
}
