using Library.Business.Abstracts;
using Library.Business.Concretes;
using Library.Business.Mappings;
using Library.Business.Security.Hashing;
using Library.Business.Security.Jwt;
using Library.DataAccess.Contexts;
using Library.DataAccess.Repositories.Abstracts;
using Library.DataAccess.Repositories.Concretes;
using Library.Model.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore; // UseSqlServer için
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Library.Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // UOW ve repo kaydı
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));



        // AutoMapper Kaydı
        // Mevcut projedeki (Business katmanı) tüm "Profile" sınıflarını (MembershipProfile vb.) otomatik bulup kaydeder.
        //Merkezi Profil Yönetimi :)))
        services.AddAutoMapper(cfg =>
        {

            cfg.AddProfile<MembershipProfile>();
            cfg.AddProfile<MemberProfile>();
            cfg.AddProfile<BookCopyProfile>();
            cfg.AddProfile<BookProfile>();
            // İleride modüller eklendikçe buraya tek satır olarak şutlayacağız:
            // cfg.AddProfile<CatalogProfile>();
            // cfg.AddProfile<OperationsProfile>();
        });

        // (Business Services)
        // Dışarıdan IAuthService istendiğinde ona AuthManager ver (Dependency Inversion)
        // TODO: service manager ve mappingleri eklemeyi unutmaaaa!!!!!!
        services.AddScoped<IAuthService, AuthManager>();
        services.AddScoped<IRoleService, RoleManager>();
        services.AddScoped<IMembershipApplicationService, MembershipApplicationManager>();
        services.AddScoped<IMemberService, MemberManager>();
        services.AddScoped<IBookService, BookManager>();
        services.AddScoped<IBookCopyService, BookCopyManager>();

        //JWT - Güvenlik Araçları ve Ayarları
        services.Configure<JwtOptions>(configuration.GetRequiredSection(JwtOptions.SectionName));

        services.AddScoped<IHashingHelper, HmacSha512HashingHelper>();
        services.AddScoped<ITokenHelper, JwtTokenHelper>();

        //Sistem Kimlik Doğrulama Şeması (Authentication)
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() 
            ?? throw new InvalidOperationException("JWT settings are missing in configuration. (null)");// TODO: null gelebilir diyordu
        var key = Encoding.UTF8.GetBytes(jwtOptions.Key);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });


        return services;
    }
}