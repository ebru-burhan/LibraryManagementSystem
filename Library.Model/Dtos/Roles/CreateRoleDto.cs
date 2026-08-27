namespace Library.Model.Dtos.Roles;

public class CreateRoleDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    // React'ten burası "view_dashboard,edit_users" gibi düz bir metin olarak gelecek.
    public string? Permissions { get; set; }
}