using Library.DataAccess.Contexts;
using Library.DataAccess.Repositories.Abstracts;
using Library.DataAccess.Repositories.Concretes;
using Library.Business.Security.Hashing;
using Library.Business.Security.Jwt;
using Library.Model.Options;
using Microsoft.EntityFrameworkCore; // UseSqlServer için
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        //JWT
        services.Configure<JwtOptions>(configuration.GetRequiredSection(JwtOptions.SectionName));

        services.AddScoped<IHashingHelper, HmacSha512HashingHelper>();
        services.AddScoped<ITokenHelper, JwtTokenHelper>();

        return services;
    }
}