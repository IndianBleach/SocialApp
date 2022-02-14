using ApplicationCore.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace WebUi.Configuration
{
    public static class ConfigureCoreServices
    {
        public static void AddCoreServices(this IServiceCollection services)
        {

            services.AddSignalR(x => {
                x.EnableDetailedErrors = true;
            });

            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));

            //services.AddTransient(typeof(IPageService), typeof(PageService));

            services.AddScoped(typeof(IIdeaRepository), typeof(IdeaRepository));

            services.AddScoped<ITagService, TagService>();

            services.AddScoped(typeof(IGlobalService<>), typeof(GlobalService<>));

            services.AddTransient<IAuthorizationService, AuthorizationService>();

            services.AddScoped<IAsyncLoadService, AsyncLoadService>();
        }
    }
}
