using Library.Entity.Abstract;

namespace Library.Entity.Concrete.Auth;

public class UserRole : BaseEntity //loglanacak durumu yok ara tablo base olsun
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}