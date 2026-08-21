using Library.Business.Abstracts;
using Library.Business.Security.Hashing;
using Library.Business.Security.Jwt;
using Library.DataAccess.Repositories.Abstracts;
using Library.Entity.Concrete.Auth;
using Library.Model.Dtos.Auth;
using Library.Model.Results;

namespace Library.Business.Concretes;

public class AuthManager : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashingHelper _hashingHelper;
    private readonly ITokenHelper _tokenHelper;

    public AuthManager(IUnitOfWork unitOfWork, IHashingHelper hashingHelper, ITokenHelper tokenHelper)
    {
        _unitOfWork = unitOfWork;
        _hashingHelper = hashingHelper;
        _tokenHelper = tokenHelper;
    }

    public async Task<IDataResult<AccessToken>> RegisterAsync(RegisterDto registerDto)
    {
        // 1. E-posta kullanımda mı kontrolü
        var userExists = await UserExistsAsync(registerDto.Email);
        if (!userExists.Success)
        {
            return new ErrorDataResult<AccessToken>(userExists.Message);
        }

        // 2. Şifreyi Hashle
        _hashingHelper.CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        // 3. User Entity'sini oluştur
        var user = new User
        {
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            IdentityNumber = registerDto.IdentityNumber,
            PhoneNumber = registerDto.PhoneNumber,
            Address = registerDto.Address,
            IsActive = true
        };

        // 4. Veritabanına kaydet
        await _unitOfWork.GetRepository<User>().AddAsync(user);
        await _unitOfWork.CompleteAsync();

        // 5. Token üret
        var accessToken = _tokenHelper.CreateToken(user, new List<string>());

        return new SuccessDataResult<AccessToken>(accessToken, "Kayıt işlemi başarıyla tamamlandı.");
    }

    public async Task<IDataResult<AccessToken>> LoginAsync(LoginDto loginDto)
    {
        // 1. Kullanıcıyı e-posta ile bul (GenericRepository FindAsync geriye koleksiyon döner)
        // Sadece okuma yapacağımız için tracking: false kullanarak performansı artırıyoruz
        var users = await _unitOfWork.GetRepository<User>()
            .FindAsync(u => u.Email == loginDto.Email, tracking: false);

        var user = users.FirstOrDefault();

        if (user == null)
        {
            return new ErrorDataResult<AccessToken>("Kullanıcı bulunamadı.");
        }

        // 2. Şifreyi doğrula
        if (!_hashingHelper.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
        {
            return new ErrorDataResult<AccessToken>("Hatalı şifre.");
        }

        // 3. Token üret ve dön
        var accessToken = _tokenHelper.CreateToken(user, new List<string>());
        return new SuccessDataResult<AccessToken>(accessToken, "Giriş başarılı.");
    }

    public async Task<IResult> UserExistsAsync(string email)
    {
        // Sadece okuma yapacağımız için tracking = false kullanıyoruz
        var users = await _unitOfWork.GetRepository<User>()
            .FindAsync(u => u.Email == email, tracking: false);

        if (users.Any())
        {
            return new ErrorResult("Bu e-posta adresi ile zaten bir kayıt mevcut.");
        }

        return new SuccessResult();
    }
}