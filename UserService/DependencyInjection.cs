using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace Identity.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUserServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddUserDbContext(configuration);
            services.AddIdentity();
            services.AddJwtOptions(configuration);
            services.AddRolePolicyProvider();
            services.AddTokenProvider();
            services.AddMediateRServices();
            services.AddAuthentication(configuration);
            services.AddUserContextService();
            services.AddIdentityProvider();
            services.AddPolicy(configuration);
            services.AddAuthorization(configuration);
            return services;
        }
        private static IServiceCollection AddUserDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("UserConnectionString")));
            return services;
        }
        private static IServiceCollection AddIdentityProvider(this IServiceCollection services)
        {
            services.AddScoped<IIdmProvider<User>, IdmProviderService<User>>();
            return services;
        }
        private static IServiceCollection AddRolePolicyProvider(this IServiceCollection service)
        {
            service.AddSingleton<IAuthorizationPolicyProvider, RolePolicyProvider>();
            service.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
            return service;
        }
        private static IServiceCollection AddTokenProvider(this IServiceCollection service)
        {
            service.AddScoped<ITokenProvider, TokenService>();
            return service;
        }
        private static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 7;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;

            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();



            return services;
        }
        private static IServiceCollection AddJwtOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");
            if (!jwtSection.Exists())
            {
                throw new InvalidOperationException("JWT configuration section is missing in the appsettings.");
            }

            services.Configure<JwtOption>(jwtSection);
            return services;
        }
        private static IServiceCollection AddMediateRServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            });
            return services;
        }
        private static IServiceCollection AddAuthentication(this IServiceCollection service, IConfiguration configuration)
        {
            var jwtSettingsSection = configuration.GetSection("Jwt");

            service.Configure<JwtOption>(jwtSettingsSection);

            var jwtSettings = jwtSettingsSection.Get<JwtOption>();

            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            service.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                };
            });

            return service;
        }
        private static IServiceCollection AddUserContextService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContextService, UserContextService>();
            return services;
        }
        private static IServiceCollection AddPolicy(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddSingleton<IAuthorizationPolicyProvider, RolePolicyProvider>();
            service.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
            return service;
        }
        private static IServiceCollection AddAuthorization(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddAuthorization();
            return service;
        }
    }
}
