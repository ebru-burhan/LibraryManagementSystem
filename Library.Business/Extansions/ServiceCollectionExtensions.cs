using Library.Business.Security.Hashing;
using Library.Business.Security.Jwt;
using Library.Model.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {    
        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<JwtOptions>(configuration.GetRequiredSection(JwtOptions.SectionName));


        services.AddScoped<IHashingHelper, HmacSha512HashingHelper>();
        services.AddScoped<ITokenHelper, JwtTokenHelper>();

        return services;
    }
}