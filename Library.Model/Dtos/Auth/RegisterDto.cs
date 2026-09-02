namespace Library.Model.Dtos.Auth;

public class RegisterDto
{
    //üye olurken girilen bilgileri alıp apiden businese vericez 
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;


    // Onaylar (Bunlar true gelmeden işlemi Business katmanında reddedeceğiz)
    public bool IsKvkkApproved { get; set; }
    public bool IsTermsAccepted { get; set; }

}