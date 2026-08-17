namespace Library.Model.Options;

public class JwtOptions
{
    public const string SectionName = "JwtSettings"; //appsettin de bu ismi yazıcaz burayı doldurcak

    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public double ExpirationInMinutes { get; set; } = 120;
}