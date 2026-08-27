using Library.Entity.Abstract;

namespace Library.Entity.Concrete.Auth;

public class Role : AuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    // Yetkileri tutacağımız kolon
    public string? Permissions { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}