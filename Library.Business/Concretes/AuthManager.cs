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

        if (!registerDto.IsKvkkApproved || !registerDto.IsTermsAccepted)
        {
            return new ErrorDataResult<AccessToken>("KVKK Aydınlatma Metni ve Kullanım Şartları onaylanmadan başvuru yapılamaz.");
        }


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
                IsActive = true,
                IsKvkkApproved = true,
                IsTermsAccepted = true
            };


            // 4. Veritabanına kaydet
            await _unitOfWork.GetRepository<User>().AddAsync(user);
            await _unitOfWork.CompleteAsync();


            //  Token üret (Yeni parametre permission için)
            var accessToken = _tokenHelper.CreateToken(user, new List<string>(), new List<string>());

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


       //tokensız giriş de unathurized alındı şimdi rolleri ekleme tokena tek seferde gelsin ne gelecekse veritabanından yoksa git gel hep zor.
        var userRoles = await _unitOfWork.GetRepository<UserRole>()
            .FindAsync(ur => ur.UserId == user.Id, tracking: false);

        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        var roles = await _unitOfWork.GetRepository<Role>()
            .FindAsync(r => roleIds.Contains(r.Id), tracking: false);

        var roleNames = roles.Select(r => r.Name).ToList();


        //permisson!!!!!
        var permissions = new List<string>();
        foreach (var role in roles)
        {
            // Eğer rolün yetkisi boş değilse (null veya boşluk değilse)
            if (!string.IsNullOrWhiteSpace(role.Permissions))
            {
                // Virgüllerden böl, etrafındaki boşlukları temizle ve listeye ekle
                var rolePermissions = role.Permissions.Split(',').Select(p => p.Trim());
                permissions.AddRange(rolePermissions);
            }
        }

        // Bir kullanıcının iki farklı rolü olabilir ve ikisinde de "view_dashboard" yetkisi olabilir.
        // Aynı yetkiyi token'a iki kez yazmamak için Distinct() ile tekrarları siliyoruz.
        var uniquePermissions = permissions.Distinct().ToList();



        // 3. Token üret ve dön
        var accessToken = _tokenHelper.CreateToken(user, roleNames, uniquePermissions);
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


    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        // 1. Kullanıcıyı bul (Güncelleme yapacağımız için tracking: true kullanıyoruz)
        var users = await _unitOfWork.GetRepository<User>()
            .FindAsync(u => u.Email == forgotPasswordDto.Email, tracking: true);

        var user = users.FirstOrDefault();

        if (user == null)
        {
            return new ErrorResult("Bu e-posta adresine kayıtlı kullanıcı bulunamadı.");
        }

        // 2. Rastgele 6 haneli bir kod üret (Örn: A4B7X9)
        string resetCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

        // 3. Kullanıcıya kodu ve süresini kaydet (1 saat geçerli)
        user.PasswordResetCode = resetCode;
        user.PasswordResetCodeExpiration = DateTime.Now.AddHours(1);

        // 4. Veritabanına yansıt
        await _unitOfWork.CompleteAsync();

        // Not: Gerçek senaryoda burada mail atılır. Biz testi kolaylaştırmak için kodu API yanıtında dönüyoruz.
        return new SuccessResult($"Şifre sıfırlama kodu oluşturuldu (Mail atılmış varsayalım). Kodunuz: {resetCode}");
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        // 1. Kullanıcıyı bul
        var users = await _unitOfWork.GetRepository<User>()
            .FindAsync(u => u.Email == resetPasswordDto.Email, tracking: true);

        var user = users.FirstOrDefault();

        if (user == null)
        {
            return new ErrorResult("Kullanıcı bulunamadı.");
        }

        // 2. Güvenlik Kontrolleri: Kod eşleşiyor mu ve süresi geçerli mi?
        if (user.PasswordResetCode != resetPasswordDto.ResetCode)
        {
            return new ErrorResult("Sıfırlama kodu hatalı.");
        }

        if (user.PasswordResetCodeExpiration < DateTime.Now)
        {
            return new ErrorResult("Sıfırlama kodunun süresi dolmuş. Lütfen tekrar kod isteyin.");
        }

        // 3. Yeni Şifreyi Hashle
        _hashingHelper.CreatePasswordHash(resetPasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        // 4. Tek kullanımlık olduğu için kodu ve süresini sıfırla (İptal et)
        user.PasswordResetCode = null;
        user.PasswordResetCodeExpiration = null;

        // 5. Veritabanına kaydet
        await _unitOfWork.CompleteAsync();

        return new SuccessResult("Şifreniz başarıyla güncellendi. Yeni şifrenizle giriş yapabilirsiniz.");
    }

}