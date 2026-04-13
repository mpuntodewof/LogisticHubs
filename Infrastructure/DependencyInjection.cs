using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)),
                    mysqlOptions => mysqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null)));

            // Multi-tenancy
            services.AddScoped<ITenantContext, TenantContext>();

            // Auth services
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Transaction management
            services.AddScoped<ITransactionManager, TransactionManager>();

            // Repositories — Auth & RBAC
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            // Repositories — Catalog
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
            services.AddScoped<IUnitConversionRepository, UnitConversionRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();

            // Repositories — Inventory
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IWarehouseStockRepository, WarehouseStockRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();

            // Repositories — Finance & Tax
            services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
            services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
            services.AddScoped<IPaymentTermRepository, PaymentTermRepository>();
            services.AddScoped<ITaxRateRepository, TaxRateRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            // Repositories — Settings
            services.AddScoped<ITenantSettingRepository, TenantSettingRepository>();
            services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();

            // Repositories — Audit
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<ISystemLogRepository, SystemLogRepository>();

            // Use Cases — Auth & RBAC
            services.AddScoped<Application.UseCases.Auth.AuthUseCase>();
            services.AddScoped<Application.UseCases.Users.UserUseCase>();
            services.AddScoped<Application.UseCases.Roles.RoleUseCase>();

            // Use Cases — Catalog
            services.AddScoped<Application.UseCases.Categories.CategoryUseCase>();
            services.AddScoped<Application.UseCases.Brands.BrandUseCase>();
            services.AddScoped<Application.UseCases.UnitsOfMeasure.UnitOfMeasureUseCase>();
            services.AddScoped<Application.UseCases.Products.ProductUseCase>();
            services.AddScoped<Application.UseCases.Products.ProductVariantUseCase>();
            services.AddScoped<Application.UseCases.Products.ProductImageUseCase>();

            // Use Cases — Inventory
            services.AddScoped<Application.UseCases.Warehouses.WarehouseUseCase>();
            services.AddScoped<Application.UseCases.Inventory.WarehouseStockUseCase>();
            services.AddScoped<Application.UseCases.Inventory.StockMovementUseCase>();

            // Use Cases — Finance & Tax
            services.AddScoped<Application.UseCases.Finance.ChartOfAccountUseCase>();
            services.AddScoped<Application.UseCases.Finance.JournalEntryUseCase>();
            services.AddScoped<Application.UseCases.Finance.PaymentTermUseCase>();
            services.AddScoped<Application.UseCases.Tax.TaxRateUseCase>();
            services.AddScoped<Application.UseCases.Tax.InvoiceUseCase>();

            // Use Cases — Settings
            services.AddScoped<Application.UseCases.Settings.TenantSettingUseCase>();
            services.AddScoped<Application.UseCases.Settings.SystemSettingUseCase>();

            // Use Cases — Audit
            services.AddScoped<Application.UseCases.Audit.AuditLogUseCase>();
            services.AddScoped<Application.UseCases.Audit.SystemLogUseCase>();

            return services;
        }
    }
}
