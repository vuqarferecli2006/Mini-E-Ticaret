using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.Shared.Helpers;
using E_Biznes.Infrastructure.Services;
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
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();

        #endregion

        #region Services

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<RoleCreationHelper>();
            services.AddScoped<RoleUpdateHelper>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAccountService,AccountService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IFileServices, FileUploadService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IReviewService, ReviewService>();


        #endregion

    }
}
