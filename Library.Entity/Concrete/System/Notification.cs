using Library.Entity.Abstract;
using Library.Entity.Concrete.Auth;

public class Notification : CreationAuditedEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string Message { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
}