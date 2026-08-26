namespace Library.Model.Dtos.Auth;

public class RegisterDto
{
    //üye olurken girilen bilgileri alıp apiden businese vericez 
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string IdentityNumber { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
    public string? Address { get; set; } = null!;
}