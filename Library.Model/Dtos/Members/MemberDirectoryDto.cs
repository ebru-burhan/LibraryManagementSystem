namespace Library.Model.Dtos.Members;

public class MemberDirectoryDto
{
    public List<MemberListDto> Members { get; set; } = new();
    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public int PassiveCount { get; set; }
    public int SuspendedCount { get; set; }
}
