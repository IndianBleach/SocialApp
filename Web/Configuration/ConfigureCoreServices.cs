using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Repositories;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace WebUi.Configuration
{
    public static class ConfigureCoreServices
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthorizationService, AuthorizationService>();

            services.AddSignalR(x => {
                x.EnableDetailedErrors = true;
            });

            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));

            services.AddScoped(typeof(IAdminRepository), typeof(AdminRepository));

            services.AddScoped(typeof(IIdeaRepository), typeof(IdeaRepository));

            services.AddScoped<ITagService, TagService>();

            services.AddScoped(typeof(IGlobalService<>), typeof(GlobalService<>));

            

            services.AddScoped<IAsyncLoadService, AsyncLoadService>();
        }
    }
}
