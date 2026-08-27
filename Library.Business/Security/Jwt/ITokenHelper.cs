using Library.Entity.Concrete.Auth;
using Library.Model.Dtos.Auth;

namespace Library.Business.Security.Jwt;

public interface ITokenHelper
{
    // AccessToken CreateToken(User user, IEnumerable<string> roles);


    // YENİ: IEnumerable<string> permissions parametresini ekledik
    AccessToken CreateToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);
}
