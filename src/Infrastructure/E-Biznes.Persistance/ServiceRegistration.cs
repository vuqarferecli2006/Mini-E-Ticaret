using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.Shared.Helpers;
using E_Biznes.Persistance.Repositories;
using E_Biznes.Persistance.Services;
using Microsoft.Extensions.DependencyInjection;

namespace E_Biznes.Persistance;

public static class ServiceRegistration
{
    public static void RegisterService(this IServiceCollection services)
    {
        #region Repositories

            services.AddScoped<ICategoryRepository, CategoryRepository>();

        #endregion

        #region Services

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<RoleCreationHelper>();
            services.AddScoped<RoleUpdateHelper>();


        #endregion

    }
}
