using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly ITenantContext? _tenantContext;
        private readonly ICurrentUserService? _currentUserService;

        public static Guid DefaultTenantId => TenantConstants.DefaultTenantId;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext? tenantContext = null, ICurrentUserService? currentUserService = null)
            : base(options)
        {
            _tenantContext = tenantContext;
            _currentUserService = currentUserService;
        }

        // Platform
        public DbSet<Tenant> Tenants => Set<Tenant>();

        // Existing
        public DbSet<User> Users => Set<User>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<ShipmentAssignment> ShipmentAssignments => Set<ShipmentAssignment>();
        public DbSet<ShipmentTracking> ShipmentTrackings => Set<ShipmentTracking>();

        // Auth & RBAC
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        // Catalog
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
        public DbSet<UnitConversion> UnitConversions => Set<UnitConversion>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();

        // Inventory
        public DbSet<WarehouseStock> WarehouseStocks => Set<WarehouseStock>();
        public DbSet<StockMovement> StockMovements => Set<StockMovement>();

        // CRM
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<CustomerGroup> CustomerGroups => Set<CustomerGroup>();
        public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();

        // Branches
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<BranchUser> BranchUsers => Set<BranchUser>();

        // Sales
        public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
        public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();
        public DbSet<SalesOrderPayment> SalesOrderPayments => Set<SalesOrderPayment>();

        // Purchase
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
        public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
        public DbSet<GoodsReceiptItem> GoodsReceiptItems => Set<GoodsReceiptItem>();

        // Promotions
        public DbSet<Promotion> Promotions => Set<Promotion>();
        public DbSet<PromotionRule> PromotionRules => Set<PromotionRule>();
        public DbSet<PromotionProduct> PromotionProducts => Set<PromotionProduct>();
        public DbSet<PromotionUsage> PromotionUsages => Set<PromotionUsage>();

        // Finance
        public DbSet<ChartOfAccount> ChartOfAccounts => Set<ChartOfAccount>();
        public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
        public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
        public DbSet<PaymentTerm> PaymentTerms => Set<PaymentTerm>();

        // Tax
        public DbSet<TaxRate> TaxRates => Set<TaxRate>();
        public DbSet<ProductTaxRate> ProductTaxRates => Set<ProductTaxRate>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

        // E-commerce
        public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
        public DbSet<ShoppingCartItem> ShoppingCartItems => Set<ShoppingCartItem>();
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
        public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
        public DbSet<CouponCode> CouponCodes => Set<CouponCode>();
        public DbSet<CouponUsage> CouponUsages => Set<CouponUsage>();

        // Storefront
        public DbSet<StorefrontConfig> StorefrontConfigs => Set<StorefrontConfig>();
        public DbSet<Banner> Banners => Set<Banner>();
        public DbSet<Page> Pages => Set<Page>();

        // Logistics
        public DbSet<DeliveryZone> DeliveryZones => Set<DeliveryZone>();
        public DbSet<DeliveryRate> DeliveryRates => Set<DeliveryRate>();
        public DbSet<ShipmentNote> ShipmentNotes => Set<ShipmentNote>();

        // Payment Gateway
        public DbSet<PaymentGatewayConfig> PaymentGatewayConfigs => Set<PaymentGatewayConfig>();
        public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();

        // Loyalty
        public DbSet<LoyaltyProgram> LoyaltyPrograms => Set<LoyaltyProgram>();
        public DbSet<LoyaltyTier> LoyaltyTiers => Set<LoyaltyTier>();
        public DbSet<LoyaltyMembership> LoyaltyMemberships => Set<LoyaltyMembership>();
        public DbSet<LoyaltyPointTransaction> LoyaltyPointTransactions => Set<LoyaltyPointTransaction>();

        // Notifications
        public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

        // HRM
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

        // Webhooks
        public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();
        public DbSet<WebhookDelivery> WebhookDeliveries => Set<WebhookDelivery>();

        // Settings & API Keys
        public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();
        public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
        public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

        // Reporting
        public DbSet<ReportDefinition> ReportDefinitions => Set<ReportDefinition>();
        public DbSet<ReportExecution> ReportExecutions => Set<ReportExecution>();
        public DbSet<DashboardWidget> DashboardWidgets => Set<DashboardWidget>();

        // Audit
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<SystemLog> SystemLogs => Set<SystemLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Global: map large string columns to TEXT to avoid MySQL row size limit ──
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string) && property.GetMaxLength() >= 4000)
                    {
                        property.SetColumnType("text");
                    }
                }
            }

            // ── Tenants (NOT tenant-scoped) ─────────────────────────────────────
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();
                entity.Property(e => e.CompanyName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // ── Users ───────────────────────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Drivers ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Driver>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.User).WithMany(u => u.Drivers).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Warehouses ──────────────────────────────────────────────────────
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Vehicles ────────────────────────────────────────────────────────
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.PlateNumber }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Shipments ───────────────────────────────────────────────────────
            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.TrackingNumber }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.OriginWarehouse).WithMany(w => w.Shipments).HasForeignKey(e => e.OriginWarehouseId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── ShipmentAssignments ─────────────────────────────────────────────
            modelBuilder.Entity<ShipmentAssignment>(entity =>
            {
                entity.HasOne(e => e.Shipment).WithMany(s => s.ShipmentAssignments).HasForeignKey(e => e.ShipmentId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Vehicle).WithMany(v => v.ShipmentAssignments).HasForeignKey(e => e.VehicleId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Driver).WithMany(d => d.ShipmentAssignments).HasForeignKey(e => e.DriverId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.TenantId).IsRequired();
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── ShipmentTracking ────────────────────────────────────────────────
            modelBuilder.Entity<ShipmentTracking>(entity =>
            {
                entity.HasOne(e => e.Shipment).WithMany(s => s.TrackingHistory).HasForeignKey(e => e.ShipmentId).OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.TenantId).IsRequired();
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── Roles ───────────────────────────────────────────────────────────
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IsSystem).HasDefaultValue(false);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Permissions ─────────────────────────────────────────────────────
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Resource).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
                entity.HasIndex(e => new { e.TenantId, e.Resource, e.Action }).IsUnique();
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── UserRoleAssignments ─────────────────────────────────────────────
            modelBuilder.Entity<UserRoleAssignment>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity.HasOne(e => e.User).WithMany(u => u.UserRoleAssignments).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Role).WithMany(r => r.UserRoleAssignments).HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.AssignedAt).IsRequired();
                entity.Property(e => e.AssignedBy).IsRequired(false);
                entity.Property(e => e.TenantId).IsRequired();
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── RolePermissions ─────────────────────────────────────────────────
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.PermissionId });
                entity.HasOne(e => e.Role).WithMany(r => r.RolePermissions).HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Permission).WithMany(p => p.RolePermissions).HasForeignKey(e => e.PermissionId).OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.TenantId).IsRequired();
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── RefreshTokens ───────────────────────────────────────────────────
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(e => e.TokenHash).IsUnique(); // Stays globally unique
                entity.Property(e => e.TokenHash).HasMaxLength(500).IsRequired();
                entity.Property(e => e.CreatedByIp).HasMaxLength(45);
                entity.Property(e => e.RevokedByIp).HasMaxLength(45);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
                entity.HasIndex(e => new { e.UserId, e.RevokedAt });
                entity.HasOne(e => e.User).WithMany(u => u.RefreshTokens).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.Ignore(e => e.IsActive);
                entity.Property(e => e.TenantId).IsRequired();
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── Categories ──────────────────────────────────────────────────────
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.ParentCategory).WithMany(e => e.ChildCategories).HasForeignKey(e => e.ParentCategoryId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Brands ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── UnitsOfMeasure ──────────────────────────────────────────────────
            modelBuilder.Entity<UnitOfMeasure>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Abbreviation }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── UnitConversions ─────────────────────────────────────────────────
            modelBuilder.Entity<UnitConversion>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.FromUnitId, e.ToUnitId }).IsUnique();
                entity.HasOne(e => e.FromUnit).WithMany(u => u.ConversionsFrom).HasForeignKey(e => e.FromUnitId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ToUnit).WithMany(u => u.ConversionsTo).HasForeignKey(e => e.ToUnitId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── Products ────────────────────────────────────────────────────────
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Category).WithMany(c => c.Products).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Brand).WithMany(b => b.Products).HasForeignKey(e => e.BrandId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.BaseUnitOfMeasure).WithMany(u => u.Products).HasForeignKey(e => e.BaseUnitOfMeasureId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── ProductVariants ─────────────────────────────────────────────────
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Sku }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Product).WithMany(p => p.Variants).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── ProductImages ───────────────────────────────────────────────────
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasOne(e => e.Product).WithMany(p => p.Images).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ProductVariant).WithMany(v => v.Images).HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── WarehouseStocks ─────────────────────────────────────────────────
            modelBuilder.Entity<WarehouseStock>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.WarehouseId, e.ProductVariantId }).IsUnique();
                entity.Ignore(e => e.QuantityAvailable);
                entity.HasOne(e => e.Warehouse).WithMany(w => w.WarehouseStocks).HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ProductVariant).WithMany(v => v.WarehouseStocks).HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── StockMovements ──────────────────────────────────────────────────
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasOne(e => e.Warehouse).WithMany().HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ProductVariant).WithMany().HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.SourceWarehouse).WithMany().HasForeignKey(e => e.SourceWarehouseId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.DestinationWarehouse).WithMany().HasForeignKey(e => e.DestinationWarehouseId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => new { e.TenantId, e.WarehouseId, e.ProductVariantId });
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── CustomerGroups ──────────────────────────────────────────────────
            modelBuilder.Entity<CustomerGroup>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Customers ──────────────────────────────────────────────────────
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.CustomerCode }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.CustomerGroup).WithMany(g => g.Customers).HasForeignKey(e => e.CustomerGroupId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── CustomerAddresses ──────────────────────────────────────────────
            modelBuilder.Entity<CustomerAddress>(entity =>
            {
                entity.HasOne(e => e.Customer).WithMany(c => c.Addresses).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── Branches ───────────────────────────────────────────────────────
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Warehouse).WithMany().HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── BranchUsers ────────────────────────────────────────────────────
            modelBuilder.Entity<BranchUser>(entity =>
            {
                entity.HasKey(e => new { e.BranchId, e.UserId });
                entity.HasOne(e => e.Branch).WithMany(b => b.BranchUsers).HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User).WithMany(u => u.BranchUsers).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.TenantId).IsRequired();
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── SalesOrders ────────────────────────────────────────────────────
            modelBuilder.Entity<SalesOrder>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.OrderNumber }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Customer).WithMany(c => c.SalesOrders).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Branch).WithMany(b => b.SalesOrders).HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ShippingAddress).WithMany().HasForeignKey(e => e.ShippingAddressId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Shipment).WithMany().HasForeignKey(e => e.ShipmentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── SalesOrderItems ────────────────────────────────────────────────
            modelBuilder.Entity<SalesOrderItem>(entity =>
            {
                entity.HasOne(e => e.SalesOrder).WithMany(o => o.Items).HasForeignKey(e => e.SalesOrderId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ProductVariant).WithMany(v => v.SalesOrderItems).HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── SalesOrderPayments ─────────────────────────────────────────────
            modelBuilder.Entity<SalesOrderPayment>(entity =>
            {
                entity.HasOne(e => e.SalesOrder).WithMany(o => o.Payments).HasForeignKey(e => e.SalesOrderId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── Suppliers ──────────────────────────────────────────────────────
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.SupplierCode }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.PaymentTerm).WithMany().HasForeignKey(e => e.PaymentTermId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── PurchaseOrders ────────────────────────────────────────────────
            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.PoNumber }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Supplier).WithMany(s => s.PurchaseOrders).HasForeignKey(e => e.SupplierId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Warehouse).WithMany(w => w.PurchaseOrders).HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Branch).WithMany().HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── PurchaseOrderItems ────────────────────────────────────────────
            modelBuilder.Entity<PurchaseOrderItem>(entity =>
            {
                entity.HasOne(e => e.PurchaseOrder).WithMany(o => o.Items).HasForeignKey(e => e.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ProductVariant).WithMany(v => v.PurchaseOrderItems).HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── GoodsReceipts ─────────────────────────────────────────────────
            modelBuilder.Entity<GoodsReceipt>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.ReceiptNumber }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.PurchaseOrder).WithMany(po => po.GoodsReceipts).HasForeignKey(e => e.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Warehouse).WithMany(w => w.GoodsReceipts).HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── GoodsReceiptItems ─────────────────────────────────────────────
            modelBuilder.Entity<GoodsReceiptItem>(entity =>
            {
                entity.HasOne(e => e.GoodsReceipt).WithMany(r => r.Items).HasForeignKey(e => e.GoodsReceiptId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.PurchaseOrderItem).WithMany().HasForeignKey(e => e.PurchaseOrderItemId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ProductVariant).WithMany().HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── Promotions ────────────────────────────────────────────────────
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Code }).HasFilter(null);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── PromotionRules ────────────────────────────────────────────────
            modelBuilder.Entity<PromotionRule>(entity =>
            {
                entity.HasOne(e => e.Promotion).WithMany(p => p.Rules).HasForeignKey(e => e.PromotionId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.CustomerGroup).WithMany().HasForeignKey(e => e.CustomerGroupId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Category).WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── PromotionProducts ─────────────────────────────────────────────
            modelBuilder.Entity<PromotionProduct>(entity =>
            {
                entity.HasOne(e => e.Promotion).WithMany(p => p.Products).HasForeignKey(e => e.PromotionId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product).WithMany(p => p.PromotionProducts).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ProductVariant).WithMany(v => v.PromotionProducts).HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── PromotionUsages ───────────────────────────────────────────────
            modelBuilder.Entity<PromotionUsage>(entity =>
            {
                entity.HasIndex(e => new { e.PromotionId, e.CustomerId });
                entity.HasOne(e => e.Promotion).WithMany(p => p.Usages).HasForeignKey(e => e.PromotionId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Customer).WithMany(c => c.PromotionUsages).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.SalesOrder).WithMany(o => o.PromotionUsages).HasForeignKey(e => e.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── ChartOfAccounts ────────────────────────────────────────────────
            modelBuilder.Entity<ChartOfAccount>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.AccountCode }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsSystemAccount).HasDefaultValue(false);
                entity.HasOne(e => e.ParentAccount).WithMany(e => e.ChildAccounts).HasForeignKey(e => e.ParentAccountId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── JournalEntries ────────────────────────────────────────────────
            modelBuilder.Entity<JournalEntry>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.EntryNumber }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── JournalEntryLines ─────────────────────────────────────────────
            modelBuilder.Entity<JournalEntryLine>(entity =>
            {
                entity.HasOne(e => e.JournalEntry).WithMany(j => j.Lines).HasForeignKey(e => e.JournalEntryId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Account).WithMany(a => a.JournalEntryLines).HasForeignKey(e => e.AccountId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── PaymentTerms ──────────────────────────────────────────────────
            modelBuilder.Entity<PaymentTerm>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── TaxRates ─────────────────────────────────────────────────────
            modelBuilder.Entity<TaxRate>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── ProductTaxRates ──────────────────────────────────────────────
            modelBuilder.Entity<ProductTaxRate>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.TaxRateId });
                entity.HasOne(e => e.Product).WithMany(p => p.ProductTaxRates).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.TaxRate).WithMany(t => t.ProductTaxRates).HasForeignKey(e => e.TaxRateId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── Invoices ─────────────────────────────────────────────────────
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.InvoiceNumber }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.SalesOrder).WithMany(o => o.Invoices).HasForeignKey(e => e.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Customer).WithMany().HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Branch).WithMany().HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.PaymentTerm).WithMany().HasForeignKey(e => e.PaymentTermId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── InvoiceItems ─────────────────────────────────────────────────
            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasOne(e => e.Invoice).WithMany(i => i.Items).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ProductVariant).WithMany().HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.SalesOrderItem).WithMany().HasForeignKey(e => e.SalesOrderItemId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.TaxRateEntity).WithMany().HasForeignKey(e => e.TaxRateId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── PaymentGatewayConfigs ────────────────────────────────────────────
            modelBuilder.Entity<PaymentGatewayConfig>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Provider, e.IsSandbox }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsSandbox).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── PaymentTransactions ─────────────────────────────────────────────
            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.TransactionNumber }).IsUnique();
                entity.HasOne(e => e.SalesOrderPayment).WithMany().HasForeignKey(e => e.SalesOrderPaymentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.PaymentGatewayConfig).WithMany().HasForeignKey(e => e.PaymentGatewayConfigId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── ShoppingCarts ───────────────────────────────────────────────────
            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Customer).WithMany(c => c.ShoppingCarts).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                entity.HasOne(e => e.ShoppingCart).WithMany(c => c.Items).HasForeignKey(e => e.ShoppingCartId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ProductVariant).WithMany(v => v.ShoppingCartItems).HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── Wishlists ──────────────────────────────────────────────────────
            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.CustomerId, e.Name }).IsUnique();
                entity.HasOne(e => e.Customer).WithMany(c => c.Wishlists).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── WishlistItems ─────────────────────────────────────────────────
            modelBuilder.Entity<WishlistItem>(entity =>
            {
                entity.HasIndex(e => new { e.WishlistId, e.ProductVariantId }).IsUnique();
                entity.HasOne(e => e.Wishlist).WithMany(w => w.Items).HasForeignKey(e => e.WishlistId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ProductVariant).WithMany(v => v.WishlistItems).HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── CouponCodes ───────────────────────────────────────────────────
            modelBuilder.Entity<CouponCode>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── CouponUsages ──────────────────────────────────────────────────
            modelBuilder.Entity<CouponUsage>(entity =>
            {
                entity.HasIndex(e => new { e.CouponCodeId, e.CustomerId });
                entity.HasOne(e => e.CouponCode).WithMany(c => c.Usages).HasForeignKey(e => e.CouponCodeId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Customer).WithMany(c => c.CouponUsages).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.SalesOrder).WithMany(o => o.CouponUsages).HasForeignKey(e => e.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── ProductReviews ──────────────────────────────────────────────────
            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Product).WithMany(p => p.Reviews).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Customer).WithMany(c => c.ProductReviews).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.SalesOrder).WithMany().HasForeignKey(e => e.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── StorefrontConfigs ───────────────────────────────────────────────
            modelBuilder.Entity<StorefrontConfig>(entity =>
            {
                entity.Property(e => e.CustomCss).HasColumnType("text");
                entity.Property(e => e.CustomJs).HasColumnType("text");
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId).IsUnique();
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── Banners ────────────────────────────────────────────────────────
            modelBuilder.Entity<Banner>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Pages ──────────────────────────────────────────────────────────
            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── DeliveryZones ──────────────────────────────────────────────────
            modelBuilder.Entity<DeliveryZone>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── DeliveryRates ─────────────────────────────────────────────────
            modelBuilder.Entity<DeliveryRate>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.DeliveryZone).WithMany(z => z.DeliveryRates).HasForeignKey(e => e.DeliveryZoneId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── ShipmentNotes ─────────────────────────────────────────────────
            modelBuilder.Entity<ShipmentNote>(entity =>
            {
                entity.HasOne(e => e.Shipment).WithMany(s => s.Notes).HasForeignKey(e => e.ShipmentId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── LoyaltyPrograms ────────────────────────────────────────────────
            modelBuilder.Entity<LoyaltyProgram>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── LoyaltyTiers ──────────────────────────────────────────────────
            modelBuilder.Entity<LoyaltyTier>(entity =>
            {
                entity.HasOne(e => e.LoyaltyProgram).WithMany(p => p.Tiers).HasForeignKey(e => e.LoyaltyProgramId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── LoyaltyMemberships ────────────────────────────────────────────
            modelBuilder.Entity<LoyaltyMembership>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.LoyaltyProgramId, e.CustomerId }).IsUnique();
                entity.HasOne(e => e.LoyaltyProgram).WithMany(p => p.Memberships).HasForeignKey(e => e.LoyaltyProgramId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Customer).WithMany(c => c.LoyaltyMemberships).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.CurrentTier).WithMany(t => t.Memberships).HasForeignKey(e => e.CurrentTierId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── LoyaltyPointTransactions ──────────────────────────────────────
            modelBuilder.Entity<LoyaltyPointTransaction>(entity =>
            {
                entity.HasOne(e => e.LoyaltyMembership).WithMany(m => m.Transactions).HasForeignKey(e => e.LoyaltyMembershipId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── NotificationTemplates ─────────────────────────────────────────
            modelBuilder.Entity<NotificationTemplate>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Notifications ────────────────────────────────────────────────────
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(e => e.User).WithMany(u => u.Notifications).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => new { e.TenantId, e.UserId, e.Status });
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── NotificationPreferences ──────────────────────────────────────────
            modelBuilder.Entity<NotificationPreference>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── HRM: Departments ────────────────────────────────────────────────
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.ParentDepartment).WithMany(e => e.ChildDepartments).HasForeignKey(e => e.ParentDepartmentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Manager).WithMany().HasForeignKey(e => e.ManagerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── HRM: Employees ─────────────────────────────────────────────────
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.EmployeeCode }).IsUnique();
                entity.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Department).WithMany(d => d.Employees).HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── HRM: Attendances ───────────────────────────────────────────────
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasOne(e => e.Employee).WithMany(e => e.Attendances).HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.EmployeeId, e.Date });
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── HRM: Leave Requests ────────────────────────────────────────────
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasOne(e => e.Employee).WithMany(e => e.LeaveRequests).HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── WebhookSubscriptions ──────────────────────────────────────────
            modelBuilder.Entity<WebhookSubscription>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasIndex(e => new { e.TenantId, e.EntityType, e.EventType });
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── WebhookDeliveries ─────────────────────────────────────────────
            modelBuilder.Entity<WebhookDelivery>(entity =>
            {
                entity.HasOne(e => e.WebhookSubscription).WithMany(ws => ws.Deliveries).HasForeignKey(e => e.WebhookSubscriptionId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.WebhookSubscriptionId, e.Status });
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── TenantSettings ────────────────────────────────────────────────
            modelBuilder.Entity<TenantSetting>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Key }).IsUnique();
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── SystemSettings (NOT tenant-scoped) ───────────────────────────────
            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.HasIndex(e => e.Key).IsUnique();
            });

            // ── ApiKeys ──────────────────────────────────────────────────────────
            modelBuilder.Entity<ApiKey>(entity =>
            {
                entity.HasIndex(e => e.KeyHash).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => new { e.TenantId, e.IsActive });
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Reporting: ReportDefinitions ──────────────────────────────────
            modelBuilder.Entity<ReportDefinition>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Reporting: ReportExecutions ───────────────────────────────────
            modelBuilder.Entity<ReportExecution>(entity =>
            {
                entity.HasOne(e => e.ReportDefinition).WithMany(r => r.Executions).HasForeignKey(e => e.ReportDefinitionId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── Reporting: DashboardWidgets ───────────────────────────────────
            modelBuilder.Entity<DashboardWidget>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── Audit: AuditLogs ──────────────────────────────────────────────
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.EntityType, e.EntityId });
                entity.HasIndex(e => new { e.TenantId, e.Timestamp });
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
            });

            // ── Audit: SystemLogs (NOT tenant-scoped) ─────────────────────────
            modelBuilder.Entity<SystemLog>(entity =>
            {
                entity.HasIndex(e => new { e.Level, e.Timestamp });
                entity.HasIndex(e => e.TenantId);
            });

            // ── Seed Data ───────────────────────────────────────────────────────
            SeedTenants(modelBuilder);
            SeedRolesAndPermissions(modelBuilder);
            SeedUsers(modelBuilder);
        }

        // ── SaveChanges: auto-stamp TenantId, audit fields, soft-delete ────
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var tenantId = _tenantContext?.TenantId;
            var userId = _currentUserService?.UserId;
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                // ── Tenant enforcement ──
                if (entry.Entity is ITenantScoped tenantEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            if (tenantId.HasValue && tenantEntity.TenantId == Guid.Empty)
                                tenantEntity.TenantId = tenantId.Value;
                            else if (!tenantId.HasValue && tenantEntity.TenantId == Guid.Empty)
                                throw new InvalidOperationException("TenantId must be set for new entities.");
                            break;

                        case EntityState.Modified:
                            if (tenantId.HasValue && tenantEntity.TenantId != tenantId.Value)
                                throw new InvalidOperationException("Cross-tenant write detected.");
                            break;

                        case EntityState.Deleted:
                            if (tenantId.HasValue && tenantEntity.TenantId != tenantId.Value)
                                throw new InvalidOperationException("Cross-tenant delete detected.");
                            break;
                    }
                }

                // ── Audit fields ──
                if (entry.Entity is BaseEntity baseEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            baseEntity.CreatedAt = now;
                            baseEntity.CreatedBy = userId;
                            break;

                        case EntityState.Modified:
                            baseEntity.UpdatedAt = now;
                            baseEntity.UpdatedBy = userId;
                            break;
                    }
                }

                // ── Soft delete interception: convert hard delete to soft delete ──
                if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeletable softDeletable)
                {
                    entry.State = EntityState.Modified;
                    softDeletable.IsDeleted = true;
                    softDeletable.DeletedAt = now;
                    softDeletable.DeletedBy = userId;
                }
            }
        }

        // ── Seed: Tenants ───────────────────────────────────────────────────
        private static void SeedTenants(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>().HasData(new Tenant
            {
                Id = DefaultTenantId,
                Slug = "default",
                CompanyName = "NiagaOne Default",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }

        // ── Seed: Roles & Permissions ───────────────────────────────────────
        private static void SeedRolesAndPermissions(ModelBuilder modelBuilder)
        {
            var adminRoleId          = new Guid("11111111-1111-1111-1111-111111111111");
            var managerRoleId        = new Guid("22222222-2222-2222-2222-222222222222");
            var driverRoleId         = new Guid("33333333-3333-3333-3333-333333333333");
            var viewerRoleId         = new Guid("44444444-4444-4444-4444-444444444444");
            var cashierRoleId        = new Guid("55555555-5555-5555-5555-555555555555");
            var warehouseStaffRoleId = new Guid("66666666-6666-6666-6666-666666666666");
            var accountantRoleId     = new Guid("77777777-7777-7777-7777-777777777777");
            var hrManagerRoleId      = new Guid("88888888-8888-8888-8888-888888888888");
            var marketingRoleId      = new Guid("99999999-9999-9999-9999-999999999999");
            var apiDeveloperRoleId   = new Guid("aabbccdd-1111-2222-3333-444455556666");

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = adminRoleId,          Name = "Admin",           Description = "Full system access",                                IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = managerRoleId,        Name = "Manager",         Description = "Full operational access across all modules",         IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = driverRoleId,         Name = "Driver",          Description = "Logistics operations and field tasks",               IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = viewerRoleId,         Name = "Viewer",          Description = "Read-only access across all modules",                IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = cashierRoleId,        Name = "Cashier",         Description = "POS operations, sales, and payment processing",     IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = warehouseStaffRoleId, Name = "Warehouse Staff", Description = "Inventory management, stock movements, receiving",  IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = accountantRoleId,     Name = "Accountant",      Description = "Finance, tax, invoicing, and payment management",   IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = hrManagerRoleId,      Name = "HR Manager",      Description = "Human resources, attendance, and leave management",  IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = marketingRoleId,      Name = "Marketing",       Description = "Promotions, loyalty, coupons, storefront, banners", IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = apiDeveloperRoleId,   Name = "API Developer",   Description = "API keys, webhooks, and integration management",    IsSystem = true, TenantId = DefaultTenantId }
            );

            var perms = new[]
            {
                ("users.create",      "users",      "create"),      // 1
                ("users.read",        "users",      "read"),        // 2
                ("users.update",      "users",      "update"),      // 3
                ("users.delete",      "users",      "delete"),      // 4
                ("roles.assign",      "roles",      "assign"),      // 5
                ("shipments.create",  "shipments",  "create"),      // 6
                ("shipments.read",    "shipments",  "read"),        // 7
                ("shipments.update",  "shipments",  "update"),      // 8
                ("shipments.delete",  "shipments",  "delete"),      // 9
                ("shipments.assign",  "shipments",  "assign"),      // 10
                ("tracking.create",   "tracking",   "create"),      // 11
                ("tracking.read",     "tracking",   "read"),        // 12
                ("drivers.manage",    "drivers",    "manage"),      // 13
                ("vehicles.manage",   "vehicles",   "manage"),      // 14
                ("warehouses.manage", "warehouses", "manage"),      // 15
                ("roles.create",      "roles",      "create"),      // 16
                ("roles.read",        "roles",      "read"),        // 17
                ("roles.update",      "roles",      "update"),      // 18
                ("roles.delete",      "roles",      "delete"),      // 19
                // Catalog
                ("categories.create", "categories", "create"),      // 20
                ("categories.read",   "categories", "read"),        // 21
                ("categories.update", "categories", "update"),      // 22
                ("categories.delete", "categories", "delete"),      // 23
                ("brands.create",     "brands",     "create"),      // 24
                ("brands.read",       "brands",     "read"),        // 25
                ("brands.update",     "brands",     "update"),      // 26
                ("brands.delete",     "brands",     "delete"),      // 27
                ("units.create",      "units",      "create"),      // 28
                ("units.read",        "units",      "read"),        // 29
                ("units.update",      "units",      "update"),      // 30
                ("units.delete",      "units",      "delete"),      // 31
                ("products.create",   "products",   "create"),      // 32
                ("products.read",     "products",   "read"),        // 33
                ("products.update",   "products",   "update"),      // 34
                ("products.delete",   "products",   "delete"),      // 35
                // Inventory
                ("inventory.read",     "inventory",  "read"),       // 36
                ("inventory.create",   "inventory",  "create"),     // 37
                ("inventory.update",   "inventory",  "update"),     // 38
                ("inventory.transfer", "inventory",  "transfer"),   // 39
                // CRM
                ("customer-groups.create", "customer-groups", "create"), // 40
                ("customer-groups.read",   "customer-groups", "read"),   // 41
                ("customer-groups.update", "customer-groups", "update"), // 42
                ("customer-groups.delete", "customer-groups", "delete"), // 43
                ("customers.create",       "customers",       "create"), // 44
                ("customers.read",         "customers",       "read"),   // 45
                ("customers.update",       "customers",       "update"), // 46
                ("customers.delete",       "customers",       "delete"), // 47
                // Sales
                ("sales-orders.create",  "sales-orders", "create"),  // 48
                ("sales-orders.read",    "sales-orders", "read"),    // 49
                ("sales-orders.update",  "sales-orders", "update"),  // 50
                ("sales-orders.delete",  "sales-orders", "delete"),  // 51
                ("sales-orders.confirm", "sales-orders", "confirm"), // 52
                ("sales-orders.cancel",  "sales-orders", "cancel"),  // 53
                ("sales-orders.pay",     "sales-orders", "pay"),     // 54
                // Multi-Branch
                ("branches.create", "branches", "create"), // 55
                ("branches.read",   "branches", "read"),   // 56
                ("branches.update", "branches", "update"), // 57
                ("branches.delete", "branches", "delete"), // 58
                ("branches.assign", "branches", "assign"), // 59
                // Finance
                ("chart-of-accounts.create", "chart-of-accounts", "create"), // 60
                ("chart-of-accounts.read",   "chart-of-accounts", "read"),   // 61
                ("chart-of-accounts.update", "chart-of-accounts", "update"), // 62
                ("chart-of-accounts.delete", "chart-of-accounts", "delete"), // 63
                ("journal-entries.create",   "journal-entries",   "create"), // 64
                ("journal-entries.read",     "journal-entries",   "read"),   // 65
                ("journal-entries.post",     "journal-entries",   "post"),   // 66
                ("journal-entries.void",     "journal-entries",   "void"),   // 67
                ("journal-entries.delete",   "journal-entries",   "delete"), // 68
                ("payment-terms.create",     "payment-terms",    "create"), // 69
                ("payment-terms.read",       "payment-terms",    "read"),   // 70
                ("payment-terms.update",     "payment-terms",    "update"), // 71
                ("payment-terms.delete",     "payment-terms",    "delete"), // 72
                // Tax
                ("tax-rates.create",         "tax-rates",        "create"),           // 73
                ("tax-rates.read",           "tax-rates",        "read"),             // 74
                ("tax-rates.update",         "tax-rates",        "update"),           // 75
                ("tax-rates.delete",         "tax-rates",        "delete"),           // 76
                ("tax-rates.assign",         "tax-rates",        "assign"),           // 77
                ("invoices.create",          "invoices",         "create"),           // 78
                ("invoices.read",            "invoices",         "read"),             // 79
                ("invoices.issue",           "invoices",         "issue"),            // 80
                ("invoices.assign-tax-number","invoices",        "assign-tax-number"),// 81
                ("invoices.pay",             "invoices",         "pay"),              // 82
                ("invoices.cancel",          "invoices",         "cancel"),           // 83
                ("invoices.delete",          "invoices",         "delete"),           // 84
                // Payment Gateway
                ("payment-gateways.create",      "payment-gateways",      "create"), // 85
                ("payment-gateways.read",        "payment-gateways",      "read"),   // 86
                ("payment-gateways.update",      "payment-gateways",      "update"), // 87
                ("payment-gateways.delete",      "payment-gateways",      "delete"), // 88
                ("payment-transactions.create",  "payment-transactions",  "create"), // 89
                ("payment-transactions.read",    "payment-transactions",  "read"),   // 90
                // E-commerce
                ("shopping-carts.read",       "shopping-carts",   "read"),     // 91
                ("shopping-carts.manage",     "shopping-carts",   "manage"),   // 92
                ("wishlists.read",            "wishlists",        "read"),     // 93
                ("wishlists.manage",          "wishlists",        "manage"),   // 94
                ("product-reviews.read",      "product-reviews",  "read"),     // 95
                ("product-reviews.moderate",  "product-reviews",  "moderate"), // 96
                ("product-reviews.delete",    "product-reviews",  "delete"),   // 97
                ("coupons.create",            "coupons",          "create"),   // 98
                ("coupons.read",              "coupons",          "read"),     // 99
                ("coupons.update",            "coupons",          "update"),   // 100
                ("coupons.delete",            "coupons",          "delete"),   // 101
                // Storefront
                ("storefront-config.read",    "storefront-config","read"),     // 102
                ("storefront-config.update",  "storefront-config","update"),   // 103
                ("banners.create",            "banners",          "create"),   // 104
                ("banners.read",              "banners",          "read"),     // 105
                ("banners.update",            "banners",          "update"),   // 106
                ("banners.delete",            "banners",          "delete"),   // 107
                ("pages.create",              "pages",            "create"),   // 108
                ("pages.read",               "pages",            "read"),     // 109
                ("pages.update",              "pages",            "update"),   // 110
                ("pages.delete",              "pages",            "delete"),   // 111
                // Logistics Enhancements
                ("delivery-zones.create",     "delivery-zones",   "create"),   // 112
                ("delivery-zones.read",       "delivery-zones",   "read"),     // 113
                ("delivery-zones.update",     "delivery-zones",   "update"),   // 114
                ("delivery-zones.delete",     "delivery-zones",   "delete"),   // 115
                ("delivery-rates.create",     "delivery-rates",   "create"),   // 116
                ("delivery-rates.read",       "delivery-rates",   "read"),     // 117
                ("delivery-rates.update",     "delivery-rates",   "update"),   // 118
                ("delivery-rates.delete",     "delivery-rates",   "delete"),   // 119
                ("shipment-notes.create",     "shipment-notes",   "create"),   // 120
                ("shipment-notes.read",       "shipment-notes",   "read"),     // 121
                ("shipment-notes.delete",     "shipment-notes",   "delete"),   // 122
                ("sales-orders.ship",         "sales-orders",     "ship"),     // 123
                // Promotions
                ("promotions.create",         "promotions",       "create"),   // 124
                ("promotions.read",           "promotions",       "read"),     // 125
                ("promotions.update",         "promotions",       "update"),   // 126
                ("promotions.delete",         "promotions",       "delete"),   // 127
                ("promotions.activate",       "promotions",       "activate"), // 128
                // Loyalty
                ("loyalty.create",            "loyalty",          "create"),   // 129
                ("loyalty.read",              "loyalty",          "read"),     // 130
                ("loyalty.update",            "loyalty",          "update"),   // 131
                ("loyalty.delete",            "loyalty",          "delete"),   // 132
                ("loyalty.enroll",            "loyalty",          "enroll"),   // 133
                ("loyalty.adjust",            "loyalty",          "adjust"),   // 134
                ("loyalty.redeem",            "loyalty",          "redeem"),   // 135
                // Purchase
                ("suppliers.create",              "suppliers",              "create"),    // 136
                ("suppliers.read",                "suppliers",              "read"),      // 137
                ("suppliers.update",              "suppliers",              "update"),    // 138
                ("suppliers.delete",              "suppliers",              "delete"),    // 139
                ("purchase-orders.create",        "purchase-orders",        "create"),    // 140
                ("purchase-orders.read",          "purchase-orders",        "read"),      // 141
                ("purchase-orders.update",        "purchase-orders",        "update"),    // 142
                ("purchase-orders.delete",        "purchase-orders",        "delete"),    // 143
                ("purchase-orders.submit",        "purchase-orders",        "submit"),    // 144
                ("purchase-orders.approve",       "purchase-orders",        "approve"),   // 145
                ("purchase-orders.cancel",        "purchase-orders",        "cancel"),    // 146
                ("goods-receipts.create",         "goods-receipts",         "create"),    // 147
                ("goods-receipts.read",           "goods-receipts",         "read"),      // 148
                ("goods-receipts.confirm",        "goods-receipts",         "confirm"),   // 149
                ("goods-receipts.delete",         "goods-receipts",         "delete"),    // 150
                // Notification
                ("notification-templates.create", "notification-templates", "create"),    // 151
                ("notification-templates.read",   "notification-templates", "read"),      // 152
                ("notification-templates.update", "notification-templates", "update"),    // 153
                ("notification-templates.delete", "notification-templates", "delete"),    // 154
                ("notifications.read",            "notifications",          "read"),      // 155
                ("notifications.create",          "notifications",          "create"),    // 156
                ("notifications.manage",          "notifications",          "manage"),    // 157
                ("notification-preferences.read", "notification-preferences","read"),     // 158
                ("notification-preferences.update","notification-preferences","update"),  // 159
                // HRM
                ("departments.create",            "departments",            "create"),    // 160
                ("departments.read",              "departments",            "read"),      // 161
                ("departments.update",            "departments",            "update"),    // 162
                ("departments.delete",            "departments",            "delete"),    // 163
                ("employees.create",              "employees",              "create"),    // 164
                ("employees.read",                "employees",              "read"),      // 165
                ("employees.update",              "employees",              "update"),    // 166
                ("employees.delete",              "employees",              "delete"),    // 167
                ("attendance.read",               "attendance",             "read"),      // 168
                ("attendance.manage",             "attendance",             "manage"),    // 169
                ("leave-requests.create",         "leave-requests",         "create"),    // 170
                ("leave-requests.read",           "leave-requests",         "read"),      // 171
                ("leave-requests.approve",        "leave-requests",         "approve"),   // 172
                // Reporting
                ("reports.create",                "reports",                "create"),    // 173
                ("reports.read",                  "reports",                "read"),      // 174
                ("reports.update",                "reports",                "update"),    // 175
                ("reports.delete",                "reports",                "delete"),    // 176
                ("reports.execute",               "reports",                "execute"),   // 177
                ("report-executions.read",        "report-executions",      "read"),      // 178
                ("dashboard-widgets.create",      "dashboard-widgets",      "create"),    // 179
                ("dashboard-widgets.read",        "dashboard-widgets",      "read"),      // 180
                ("dashboard-widgets.update",      "dashboard-widgets",      "update"),    // 181
                ("dashboard-widgets.delete",      "dashboard-widgets",      "delete"),    // 182
                // Audit
                ("audit-logs.read",               "audit-logs",             "read"),      // 183
                ("audit-logs.export",             "audit-logs",             "export"),    // 184
                ("system-logs.read",              "system-logs",            "read"),      // 185
                // Settings
                ("tenant-settings.read",          "tenant-settings",        "read"),      // 186
                ("tenant-settings.update",        "tenant-settings",        "update"),    // 187
                ("system-settings.read",          "system-settings",        "read"),      // 188
                ("system-settings.update",        "system-settings",        "update"),    // 189
                ("api-keys.create",               "api-keys",               "create"),    // 190
                ("api-keys.read",                 "api-keys",               "read"),      // 191
                ("api-keys.update",               "api-keys",               "update"),    // 192
                ("api-keys.delete",               "api-keys",               "delete"),    // 193
                ("api-keys.regenerate",           "api-keys",               "regenerate"),// 194
                // Webhooks
                ("webhooks.create",               "webhooks",               "create"),    // 195
                ("webhooks.read",                 "webhooks",               "read"),      // 196
                ("webhooks.update",               "webhooks",               "update"),    // 197
                ("webhooks.delete",               "webhooks",               "delete"),    // 198
                ("webhooks.test",                 "webhooks",               "test"),      // 199
                ("webhook-deliveries.read",       "webhook-deliveries",     "read"),      // 200
                ("webhook-deliveries.retry",      "webhook-deliveries",     "retry"),     // 201
            };

            var permIds = new Dictionary<string, Guid>();
            var permEntities = new List<Permission>();
            for (int i = 0; i < perms.Length; i++)
            {
                var id = new Guid($"aaaaaaaa-{(i + 1):D4}-aaaa-aaaa-aaaaaaaaaaaa");
                permIds[perms[i].Item1] = id;
                permEntities.Add(new Permission
                {
                    Id = id,
                    Name = perms[i].Item1,
                    Resource = perms[i].Item2,
                    Action = perms[i].Item3,
                    TenantId = DefaultTenantId
                });
            }

            modelBuilder.Entity<Permission>().HasData(permEntities);

            var allRolePerms = new List<RolePermission>();

            // Admin: all
            foreach (var permId in permIds.Values)
                allRolePerms.Add(new RolePermission { RoleId = adminRoleId, PermissionId = permId, TenantId = DefaultTenantId });

            // Manager: full operational access
            foreach (var p in new[] {
                "users.read", "shipments.create", "shipments.read", "shipments.update", "shipments.assign",
                "tracking.create", "tracking.read", "drivers.manage", "vehicles.manage", "warehouses.manage",
                "categories.create", "categories.read", "categories.update", "categories.delete",
                "brands.create", "brands.read", "brands.update", "brands.delete",
                "units.create", "units.read", "units.update", "units.delete",
                "products.create", "products.read", "products.update", "products.delete",
                "inventory.read", "inventory.create", "inventory.update", "inventory.transfer",
                "customer-groups.create", "customer-groups.read", "customer-groups.update", "customer-groups.delete",
                "customers.create", "customers.read", "customers.update", "customers.delete",
                "sales-orders.create", "sales-orders.read", "sales-orders.update", "sales-orders.delete",
                "sales-orders.confirm", "sales-orders.cancel", "sales-orders.pay",
                "branches.read", "branches.assign",
                // Finance (read + invoices)
                "chart-of-accounts.read", "journal-entries.read", "payment-terms.read",
                "invoices.create", "invoices.read", "invoices.issue", "invoices.pay",
                // Tax
                "tax-rates.read", "tax-rates.assign",
                // Payment Gateway
                "payment-gateways.read", "payment-transactions.create", "payment-transactions.read",
                // E-commerce + Storefront
                "shopping-carts.read", "shopping-carts.manage",
                "wishlists.read", "product-reviews.read", "product-reviews.moderate",
                "coupons.create", "coupons.read", "coupons.update",
                "storefront-config.read", "banners.create", "banners.read", "banners.update", "pages.read",
                // Logistics Enhancements
                "delivery-zones.create", "delivery-zones.read", "delivery-zones.update",
                "delivery-rates.create", "delivery-rates.read", "delivery-rates.update",
                "shipment-notes.create", "shipment-notes.read", "sales-orders.ship",
                // Promotions
                "promotions.create", "promotions.read", "promotions.update", "promotions.activate",
                // Loyalty
                "loyalty.read", "loyalty.enroll", "loyalty.adjust", "loyalty.redeem",
                // Purchase
                "suppliers.create", "suppliers.read", "suppliers.update",
                "purchase-orders.create", "purchase-orders.read", "purchase-orders.update",
                "purchase-orders.submit", "purchase-orders.approve", "purchase-orders.cancel",
                "goods-receipts.create", "goods-receipts.read", "goods-receipts.confirm",
                // Notification
                "notification-templates.read", "notifications.read", "notifications.create",
                "notification-preferences.read", "notification-preferences.update",
                // HRM
                "departments.read", "employees.read", "employees.create", "employees.update",
                "attendance.read", "attendance.manage",
                "leave-requests.create", "leave-requests.read", "leave-requests.approve",
                // Reporting
                "reports.create", "reports.read", "reports.update", "reports.execute",
                "report-executions.read",
                "dashboard-widgets.create", "dashboard-widgets.read", "dashboard-widgets.update",
                // Audit
                "audit-logs.read",
                // Settings
                "tenant-settings.read", "tenant-settings.update",
                "api-keys.create", "api-keys.read", "api-keys.update",
                // Webhooks
                "webhooks.create", "webhooks.read", "webhooks.update", "webhooks.test",
                "webhook-deliveries.read"
            })
                allRolePerms.Add(new RolePermission { RoleId = managerRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            // Driver: logistics ops + read access
            foreach (var p in new[] {
                "shipments.read", "tracking.create", "tracking.read",
                "products.read", "inventory.read",
                "customers.read", "sales-orders.read", "branches.read",
                "delivery-zones.read", "delivery-rates.read",
                "shipment-notes.create", "shipment-notes.read", "sales-orders.ship",
                "notifications.read", "notification-preferences.read", "notification-preferences.update",
                "attendance.read", "attendance.manage", "leave-requests.create", "leave-requests.read"
            })
                allRolePerms.Add(new RolePermission { RoleId = driverRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            // Viewer: read-only everything
            foreach (var p in new[] {
                "shipments.read", "tracking.read",
                "categories.read", "brands.read", "units.read", "products.read", "inventory.read",
                "customer-groups.read", "customers.read", "sales-orders.read", "branches.read",
                "chart-of-accounts.read", "journal-entries.read", "payment-terms.read",
                "tax-rates.read", "invoices.read", "payment-gateways.read", "payment-transactions.read",
                "shopping-carts.read", "wishlists.read", "product-reviews.read",
                "coupons.read", "storefront-config.read", "banners.read", "pages.read",
                "delivery-zones.read", "delivery-rates.read", "shipment-notes.read",
                "promotions.read", "loyalty.read",
                "suppliers.read", "purchase-orders.read", "goods-receipts.read",
                "notification-templates.read", "notifications.read", "notification-preferences.read",
                "departments.read", "employees.read", "attendance.read", "leave-requests.read",
                "reports.read", "report-executions.read", "dashboard-widgets.read",
                "audit-logs.read", "tenant-settings.read"
            })
                allRolePerms.Add(new RolePermission { RoleId = viewerRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            // Cashier: POS operations, sales, payments, customers, products read
            foreach (var p in new[] {
                "products.read", "inventory.read", "categories.read", "brands.read", "units.read",
                "customers.create", "customers.read", "customers.update",
                "customer-groups.read",
                "sales-orders.create", "sales-orders.read", "sales-orders.update",
                "sales-orders.confirm", "sales-orders.cancel", "sales-orders.pay",
                "coupons.read",
                "loyalty.read", "loyalty.redeem",
                "branches.read",
                "notifications.read", "notification-preferences.read", "notification-preferences.update",
                "attendance.read", "attendance.manage",
                "dashboard-widgets.read"
            })
                allRolePerms.Add(new RolePermission { RoleId = cashierRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            // Warehouse Staff: inventory, stock movements, goods receipts, shipments
            foreach (var p in new[] {
                "products.read", "categories.read", "brands.read", "units.read",
                "inventory.read", "inventory.create", "inventory.update", "inventory.transfer",
                "warehouses.manage",
                "shipments.create", "shipments.read", "shipments.update", "shipments.assign",
                "tracking.create", "tracking.read",
                "delivery-zones.read", "delivery-rates.read",
                "shipment-notes.create", "shipment-notes.read",
                "purchase-orders.read",
                "goods-receipts.create", "goods-receipts.read", "goods-receipts.confirm",
                "suppliers.read",
                "notifications.read", "notification-preferences.read", "notification-preferences.update",
                "attendance.read", "attendance.manage",
                "dashboard-widgets.read"
            })
                allRolePerms.Add(new RolePermission { RoleId = warehouseStaffRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            // Accountant: finance, tax, invoicing, payment, reporting
            foreach (var p in new[] {
                "chart-of-accounts.create", "chart-of-accounts.read", "chart-of-accounts.update", "chart-of-accounts.delete",
                "journal-entries.create", "journal-entries.read", "journal-entries.post", "journal-entries.void", "journal-entries.delete",
                "payment-terms.create", "payment-terms.read", "payment-terms.update", "payment-terms.delete",
                "tax-rates.create", "tax-rates.read", "tax-rates.update", "tax-rates.delete", "tax-rates.assign",
                "invoices.create", "invoices.read", "invoices.issue", "invoices.assign-tax-number", "invoices.pay", "invoices.cancel", "invoices.delete",
                "payment-gateways.read", "payment-transactions.read",
                "sales-orders.read", "purchase-orders.read", "goods-receipts.read",
                "suppliers.read", "customers.read",
                "reports.create", "reports.read", "reports.update", "reports.execute",
                "report-executions.read",
                "dashboard-widgets.create", "dashboard-widgets.read", "dashboard-widgets.update",
                "audit-logs.read", "audit-logs.export",
                "tenant-settings.read",
                "notifications.read", "notification-preferences.read", "notification-preferences.update",
                "attendance.read", "leave-requests.read"
            })
                allRolePerms.Add(new RolePermission { RoleId = accountantRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            // HR Manager: departments, employees, attendance, leave, notifications
            foreach (var p in new[] {
                "departments.create", "departments.read", "departments.update", "departments.delete",
                "employees.create", "employees.read", "employees.update", "employees.delete",
                "attendance.read", "attendance.manage",
                "leave-requests.create", "leave-requests.read", "leave-requests.approve",
                "users.read",
                "branches.read",
                "reports.create", "reports.read", "reports.execute",
                "report-executions.read",
                "dashboard-widgets.create", "dashboard-widgets.read", "dashboard-widgets.update",
                "notifications.read", "notifications.create",
                "notification-templates.read",
                "notification-preferences.read", "notification-preferences.update",
                "tenant-settings.read"
            })
                allRolePerms.Add(new RolePermission { RoleId = hrManagerRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            // Marketing: promotions, loyalty, coupons, storefront, banners, pages, reviews
            foreach (var p in new[] {
                "promotions.create", "promotions.read", "promotions.update", "promotions.delete", "promotions.activate",
                "loyalty.create", "loyalty.read", "loyalty.update", "loyalty.delete", "loyalty.enroll", "loyalty.adjust",
                "coupons.create", "coupons.read", "coupons.update", "coupons.delete",
                "storefront-config.read", "storefront-config.update",
                "banners.create", "banners.read", "banners.update", "banners.delete",
                "pages.create", "pages.read", "pages.update", "pages.delete",
                "product-reviews.read", "product-reviews.moderate", "product-reviews.delete",
                "products.read", "categories.read", "brands.read",
                "customers.read", "customer-groups.read",
                "sales-orders.read",
                "reports.create", "reports.read", "reports.execute",
                "report-executions.read",
                "dashboard-widgets.create", "dashboard-widgets.read", "dashboard-widgets.update",
                "notifications.read", "notification-preferences.read", "notification-preferences.update"
            })
                allRolePerms.Add(new RolePermission { RoleId = marketingRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            // API Developer: API keys, webhooks, tenant settings, system config
            foreach (var p in new[] {
                "api-keys.create", "api-keys.read", "api-keys.update", "api-keys.delete", "api-keys.regenerate",
                "webhooks.create", "webhooks.read", "webhooks.update", "webhooks.delete", "webhooks.test",
                "webhook-deliveries.read", "webhook-deliveries.retry",
                "tenant-settings.read", "tenant-settings.update",
                "system-settings.read",
                "notification-templates.create", "notification-templates.read", "notification-templates.update", "notification-templates.delete",
                "notifications.read", "notification-preferences.read", "notification-preferences.update",
                "audit-logs.read",
                "system-logs.read",
                "reports.read", "report-executions.read",
                "dashboard-widgets.read"
            })
                allRolePerms.Add(new RolePermission { RoleId = apiDeveloperRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            modelBuilder.Entity<RolePermission>().HasData(allRolePerms);
        }

        // ── Seed: Users ─────────────────────────────────────────────────────
        private static void SeedUsers(ModelBuilder modelBuilder)
        {
            const string passwordHash = "$2a$12$/1CjZqaBIZTbTErAYBktTuw/iK9Y1I.BYKu7J1B9ZWSh5KQhFw9Gy"; // password123
            var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var adminUserId          = new Guid("10000000-0000-0000-0000-000000000001");
            var managerUserId        = new Guid("10000000-0000-0000-0000-000000000002");
            var driverUserId         = new Guid("10000000-0000-0000-0000-000000000003");
            var viewerUserId         = new Guid("10000000-0000-0000-0000-000000000004");
            var cashierUserId        = new Guid("10000000-0000-0000-0000-000000000005");
            var warehouseStaffUserId = new Guid("10000000-0000-0000-0000-000000000006");
            var accountantUserId     = new Guid("10000000-0000-0000-0000-000000000007");
            var hrManagerUserId      = new Guid("10000000-0000-0000-0000-000000000008");
            var marketingUserId      = new Guid("10000000-0000-0000-0000-000000000009");
            var apiDeveloperUserId   = new Guid("10000000-0000-0000-0000-000000000010");

            modelBuilder.Entity<User>().HasData(
                new User { Id = adminUserId,          Name = "Alice Admin",          Email = "admin@niagaone.com",          PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = managerUserId,        Name = "Marcus Manager",       Email = "manager@niagaone.com",        PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = driverUserId,         Name = "Diana Driver",         Email = "driver@niagaone.com",         PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = viewerUserId,         Name = "Victor Viewer",        Email = "viewer@niagaone.com",         PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = cashierUserId,        Name = "Citra Cashier",        Email = "cashier@niagaone.com",        PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = warehouseStaffUserId, Name = "Wira Warehouse",       Email = "warehouse@niagaone.com",      PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = accountantUserId,     Name = "Andi Accountant",      Email = "accountant@niagaone.com",     PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = hrManagerUserId,      Name = "Hana HR",              Email = "hr@niagaone.com",             PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = marketingUserId,      Name = "Maya Marketing",       Email = "marketing@niagaone.com",      PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = apiDeveloperUserId,   Name = "Deva Developer",       Email = "developer@niagaone.com",      PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId }
            );

            var adminRoleId          = new Guid("11111111-1111-1111-1111-111111111111");
            var managerRoleId        = new Guid("22222222-2222-2222-2222-222222222222");
            var driverRoleId         = new Guid("33333333-3333-3333-3333-333333333333");
            var viewerRoleId         = new Guid("44444444-4444-4444-4444-444444444444");
            var cashierRoleId        = new Guid("55555555-5555-5555-5555-555555555555");
            var warehouseStaffRoleId = new Guid("66666666-6666-6666-6666-666666666666");
            var accountantRoleId     = new Guid("77777777-7777-7777-7777-777777777777");
            var hrManagerRoleId      = new Guid("88888888-8888-8888-8888-888888888888");
            var marketingRoleId      = new Guid("99999999-9999-9999-9999-999999999999");
            var apiDeveloperRoleId   = new Guid("aabbccdd-1111-2222-3333-444455556666");

            modelBuilder.Entity<UserRoleAssignment>().HasData(
                new UserRoleAssignment { UserId = adminUserId,          RoleId = adminRoleId,          AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = managerUserId,        RoleId = managerRoleId,        AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = driverUserId,         RoleId = driverRoleId,         AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = viewerUserId,         RoleId = viewerRoleId,         AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = cashierUserId,        RoleId = cashierRoleId,        AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = warehouseStaffUserId, RoleId = warehouseStaffRoleId, AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = accountantUserId,     RoleId = accountantRoleId,     AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = hrManagerUserId,      RoleId = hrManagerRoleId,      AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = marketingUserId,      RoleId = marketingRoleId,      AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = apiDeveloperUserId,   RoleId = apiDeveloperRoleId,   AssignedAt = createdAt, TenantId = DefaultTenantId }
            );
        }
    }
}
