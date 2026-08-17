using Library.Entity.Concrete;
using Library.Model.Dtos.Auth;

namespace Library.Business.Security.Jwt;

public interface ITokenHelper
{
    AccessToken CreateToken(User user, IEnumerable<string> roles);
}
