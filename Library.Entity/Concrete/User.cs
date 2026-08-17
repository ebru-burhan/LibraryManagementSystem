using Library.Entity.Abstract;

namespace Library.Entity.Concrete;

public class User : AuditableEntity
{
    public string Email { get; set; } = null!;
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;

    //eklenecek var notification

}