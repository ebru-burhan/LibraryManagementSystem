using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Library.Entity;
using Library.Model.Dtos.Auth;
using Library.Model.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Library.Business.Security.Jwt;

public class JwtTokenHelper : ITokenHelper
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenHelper(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public AccessToken CreateToken(User user, IEnumerable<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expirationDate = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expirationDate,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,

            SigningCredentials = new SigningCredentials(

                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature) //hızlı varsayılan imza çünkü her işlemde yapıcak
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AccessToken
        {
            Token = tokenHandler.WriteToken(token),
            Expiration = expirationDate
        };
    }
}