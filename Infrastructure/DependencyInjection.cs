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

            // Repositories
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IShipmentRepository, ShipmentRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            // Catalog repositories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
            services.AddScoped<IUnitConversionRepository, UnitConversionRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            // Inventory repositories
            services.AddScoped<IWarehouseStockRepository, WarehouseStockRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            // CRM repositories
            services.AddScoped<ICustomerGroupRepository, CustomerGroupRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
            // Branch repositories
            services.AddScoped<IBranchRepository, BranchRepository>();
            // Sales repositories
            services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
            // Purchase repositories
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IGoodsReceiptRepository, GoodsReceiptRepository>();
            // Promotion repositories
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            // E-commerce repositories
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();
            services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
            services.AddScoped<ICouponCodeRepository, CouponCodeRepository>();
            // Storefront repositories
            services.AddScoped<IStorefrontConfigRepository, StorefrontConfigRepository>();
            services.AddScoped<IBannerRepository, BannerRepository>();
            services.AddScoped<IPageRepository, PageRepository>();
            // Payment Gateway repositories
            services.AddScoped<IPaymentGatewayConfigRepository, PaymentGatewayConfigRepository>();
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            // Logistics repositories
            services.AddScoped<IDeliveryZoneRepository, DeliveryZoneRepository>();
            services.AddScoped<IDeliveryRateRepository, DeliveryRateRepository>();
            services.AddScoped<IShipmentNoteRepository, ShipmentNoteRepository>();
            // Tax repositories
            services.AddScoped<ITaxRateRepository, TaxRateRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            // Finance repositories
            services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
            services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
            services.AddScoped<IPaymentTermRepository, PaymentTermRepository>();
            // Loyalty repositories
            services.AddScoped<ILoyaltyProgramRepository, LoyaltyProgramRepository>();
            services.AddScoped<ILoyaltyTierRepository, LoyaltyTierRepository>();
            services.AddScoped<ILoyaltyMembershipRepository, LoyaltyMembershipRepository>();
            services.AddScoped<ILoyaltyPointTransactionRepository, LoyaltyPointTransactionRepository>();
            // Notification repositories
            services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();
            // HRM repositories
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            // Webhook repositories
            services.AddScoped<IWebhookSubscriptionRepository, WebhookSubscriptionRepository>();
            services.AddScoped<IWebhookDeliveryRepository, WebhookDeliveryRepository>();
            // Settings & API Key repositories
            services.AddScoped<ITenantSettingRepository, TenantSettingRepository>();
            services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
            services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
            // Reporting repositories
            services.AddScoped<IReportDefinitionRepository, ReportDefinitionRepository>();
            services.AddScoped<IReportExecutionRepository, ReportExecutionRepository>();
            services.AddScoped<IDashboardWidgetRepository, DashboardWidgetRepository>();
            // Audit repositories
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<ISystemLogRepository, SystemLogRepository>();

            // Use cases
            services.AddScoped<Application.UseCases.Auth.AuthUseCase>();
            services.AddScoped<Application.UseCases.Users.UserUseCase>();
            services.AddScoped<Application.UseCases.Shipments.ShipmentUseCase>();
            services.AddScoped<Application.UseCases.Drivers.DriverUseCase>();
            services.AddScoped<Application.UseCases.Vehicles.VehicleUseCase>();
            services.AddScoped<Application.UseCases.Warehouses.WarehouseUseCase>();
            services.AddScoped<Application.UseCases.Roles.RoleUseCase>();
            // Catalog use cases
            services.AddScoped<Application.UseCases.Categories.CategoryUseCase>();
            services.AddScoped<Application.UseCases.Brands.BrandUseCase>();
            services.AddScoped<Application.UseCases.UnitsOfMeasure.UnitOfMeasureUseCase>();
            services.AddScoped<Application.UseCases.Products.ProductUseCase>();
            services.AddScoped<Application.UseCases.Products.ProductVariantUseCase>();
            services.AddScoped<Application.UseCases.Products.ProductImageUseCase>();
            // Inventory use cases
            services.AddScoped<Application.UseCases.Inventory.WarehouseStockUseCase>();
            services.AddScoped<Application.UseCases.Inventory.StockMovementUseCase>();
            // CRM use cases
            services.AddScoped<Application.UseCases.Customers.CustomerGroupUseCase>();
            services.AddScoped<Application.UseCases.Customers.CustomerUseCase>();
            services.AddScoped<Application.UseCases.Customers.CustomerAddressUseCase>();
            // Branch use cases
            services.AddScoped<Application.UseCases.Branches.BranchUseCase>();
            // Sales use cases
            services.AddScoped<Application.UseCases.Sales.SalesOrderUseCase>();
            // Purchase use cases
            services.AddScoped<Application.UseCases.Purchase.SupplierUseCase>();
            services.AddScoped<Application.UseCases.Purchase.PurchaseOrderUseCase>();
            services.AddScoped<Application.UseCases.Purchase.GoodsReceiptUseCase>();
            // Promotion use cases
            services.AddScoped<Application.UseCases.Promotions.PromotionUseCase>();
            // E-commerce use cases
            services.AddScoped<Application.UseCases.Ecommerce.ShoppingCartUseCase>();
            services.AddScoped<Application.UseCases.Ecommerce.WishlistUseCase>();
            services.AddScoped<Application.UseCases.Ecommerce.ProductReviewUseCase>();
            services.AddScoped<Application.UseCases.Ecommerce.CouponCodeUseCase>();
            // Storefront use cases
            services.AddScoped<Application.UseCases.Storefront.StorefrontConfigUseCase>();
            services.AddScoped<Application.UseCases.Storefront.BannerUseCase>();
            services.AddScoped<Application.UseCases.Storefront.PageUseCase>();
            // Payment Gateway use cases
            services.AddScoped<Application.UseCases.PaymentGateway.PaymentGatewayConfigUseCase>();
            services.AddScoped<Application.UseCases.PaymentGateway.PaymentTransactionUseCase>();
            // Tax use cases
            services.AddScoped<Application.UseCases.Tax.TaxRateUseCase>();
            services.AddScoped<Application.UseCases.Tax.InvoiceUseCase>();
            // Finance use cases
            services.AddScoped<Application.UseCases.Finance.ChartOfAccountUseCase>();
            services.AddScoped<Application.UseCases.Finance.JournalEntryUseCase>();
            services.AddScoped<Application.UseCases.Finance.PaymentTermUseCase>();
            // Logistics use cases
            services.AddScoped<Application.UseCases.Logistics.DeliveryZoneUseCase>();
            services.AddScoped<Application.UseCases.Logistics.DeliveryRateUseCase>();
            services.AddScoped<Application.UseCases.Logistics.ShipmentNoteUseCase>();
            // Loyalty use cases
            services.AddScoped<Application.UseCases.Loyalty.LoyaltyProgramUseCase>();
            services.AddScoped<Application.UseCases.Loyalty.LoyaltyMembershipUseCase>();
            // Notification use cases
            services.AddScoped<Application.UseCases.Notifications.NotificationTemplateUseCase>();
            services.AddScoped<Application.UseCases.Notifications.NotificationUseCase>();
            services.AddScoped<Application.UseCases.Notifications.NotificationPreferenceUseCase>();
            // HRM use cases
            services.AddScoped<Application.UseCases.HRM.DepartmentUseCase>();
            services.AddScoped<Application.UseCases.HRM.EmployeeUseCase>();
            services.AddScoped<Application.UseCases.HRM.AttendanceUseCase>();
            services.AddScoped<Application.UseCases.HRM.LeaveRequestUseCase>();
            // Webhook use cases
            services.AddScoped<Application.UseCases.Api.WebhookSubscriptionUseCase>();
            services.AddScoped<Application.UseCases.Api.WebhookDeliveryUseCase>();
            // Settings & API Key use cases
            services.AddScoped<Application.UseCases.Settings.TenantSettingUseCase>();
            services.AddScoped<Application.UseCases.Settings.SystemSettingUseCase>();
            services.AddScoped<Application.UseCases.Settings.ApiKeyUseCase>();
            // Reporting use cases
            services.AddScoped<Application.UseCases.Reporting.ReportDefinitionUseCase>();
            services.AddScoped<Application.UseCases.Reporting.ReportExecutionUseCase>();
            services.AddScoped<Application.UseCases.Reporting.DashboardWidgetUseCase>();
            // Audit use cases
            services.AddScoped<Application.UseCases.Audit.AuditLogUseCase>();
            services.AddScoped<Application.UseCases.Audit.SystemLogUseCase>();

            return services;
        }
    }
}
