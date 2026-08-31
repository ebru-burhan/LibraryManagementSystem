namespace Library.Model.Dtos.Auth;

public class ResetPasswordDto
{
    public string Email { get; set; } = null!;
    public string ResetCode { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}