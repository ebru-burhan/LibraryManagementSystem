using Library.Model.Dtos.Auth;
using Library.Model.Results;

namespace Library.Business.Abstracts;

public interface IAuthService
{
    // Kayıt olma ve Giriş yapma işlemleri
    Task<IDataResult<AccessToken>> RegisterAsync(RegisterDto registerDto);
    Task<IDataResult<AccessToken>> LoginAsync(LoginDto loginDto);
    Task<IResult> UserExistsAsync(string email);
}