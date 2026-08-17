using Library.Entity.Abstract;

namespace Library.Entity.Concrete;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public class UserRole : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}