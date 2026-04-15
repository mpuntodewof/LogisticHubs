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

        // Auth & RBAC
        public DbSet<User> Users => Set<User>();
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

        // Warehouses
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();

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

        // Settings
        public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();
        public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

        // Audit
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<SystemLog> SystemLogs => Set<SystemLog>();

        // Idempotency
        public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

        // Import
        public DbSet<SalesChannel> SalesChannels => Set<SalesChannel>();
        public DbSet<CsvImportBatch> CsvImportBatches => Set<CsvImportBatch>();
        public DbSet<CsvImportRow> CsvImportRows => Set<CsvImportRow>();

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

            // ── Warehouses ──────────────────────────────────────────────────────
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => !e.IsDeleted && (_tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId));
            });

            // ── WarehouseStocks ─────────────────────────────────────────────────
            modelBuilder.Entity<WarehouseStock>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.WarehouseId, e.ProductVariantId }).IsUnique();
                entity.Ignore(e => e.QuantityAvailable);
                entity.Property(e => e.RowVersion).IsRowVersion();
                entity.HasOne(e => e.Warehouse).WithMany(w => w.WarehouseStocks).HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ProductVariant).WithMany(v => v.WarehouseStocks).HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenantId);
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
                entity.ToTable(t => t.HasCheckConstraint("CK_WarehouseStock_NonNegative", "QuantityOnHand >= 0"));
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
                entity.HasOne(e => e.TaxRateEntity).WithMany().HasForeignKey(e => e.TaxRateId).OnDelete(DeleteBehavior.Restrict);
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

            // ── Idempotency ────────────────────────────────────────────────────
            modelBuilder.Entity<IdempotencyRecord>(entity =>
            {
                entity.HasKey(e => e.IdempotencyKey);
                entity.HasIndex(e => e.ExpiresAt);
            });

            // ── Import ─────────────────────────────────────────────────────────
            modelBuilder.Entity<SalesChannel>(entity =>
            {
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
                entity.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique();
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CsvImportBatch>(entity =>
            {
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.SalesChannel).WithMany(c => c.ImportBatches).HasForeignKey(e => e.SalesChannelId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Warehouse).WithMany().HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CsvImportRow>(entity =>
            {
                entity.HasQueryFilter(e => _tenantContext == null || _tenantContext.TenantId == null || e.TenantId == _tenantContext.TenantId);
                entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Batch).WithMany(b => b.Rows).HasForeignKey(e => e.CsvImportBatchId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.MatchedProductVariant).WithMany().HasForeignKey(e => e.MatchedProductVariantId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.StockMovement).WithMany().HasForeignKey(e => e.StockMovementId).OnDelete(DeleteBehavior.SetNull);
                entity.HasIndex(e => new { e.CsvImportBatchId, e.RowNumber });
                entity.Property(e => e.RawRowJson).HasColumnType("text");
            });

            // ── Seed Data ───────────────────────────────────────────────────────
            SeedTenants(modelBuilder);
            SeedRolesAndPermissions(modelBuilder);
            SeedUsers(modelBuilder);
            SeedProductCatalog(modelBuilder);
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
                CompanyName = "StockLedger Default",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }

        // ── Seed: Roles & Permissions ───────────────────────────────────────
        private static void SeedRolesAndPermissions(ModelBuilder modelBuilder)
        {
            var adminRoleId          = new Guid("11111111-1111-1111-1111-111111111111");
            var managerRoleId        = new Guid("22222222-2222-2222-2222-222222222222");
            var viewerRoleId         = new Guid("44444444-4444-4444-4444-444444444444");
            var warehouseStaffRoleId = new Guid("66666666-6666-6666-6666-666666666666");
            var accountantRoleId     = new Guid("77777777-7777-7777-7777-777777777777");

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = adminRoleId,          Name = "Admin",           Description = "Full system access",                                IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = managerRoleId,        Name = "Manager",         Description = "Full operational access across all modules",         IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = viewerRoleId,         Name = "Viewer",          Description = "Read-only access across all modules",                IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = warehouseStaffRoleId, Name = "Warehouse Staff", Description = "Inventory management, stock movements, receiving",  IsSystem = true, TenantId = DefaultTenantId },
                new Role { Id = accountantRoleId,     Name = "Accountant",      Description = "Finance, tax, invoicing, and payment management",   IsSystem = true, TenantId = DefaultTenantId }
            );

            var perms = new[]
            {
                // Auth
                ("users.create",      "users",      "create"),      // 1
                ("users.read",        "users",      "read"),        // 2
                ("users.update",      "users",      "update"),      // 3
                ("users.delete",      "users",      "delete"),      // 4
                ("roles.assign",      "roles",      "assign"),      // 5
                ("roles.create",      "roles",      "create"),      // 6
                ("roles.read",        "roles",      "read"),        // 7
                ("roles.update",      "roles",      "update"),      // 8
                ("roles.delete",      "roles",      "delete"),      // 9
                // Catalog
                ("categories.create", "categories", "create"),      // 10
                ("categories.read",   "categories", "read"),        // 11
                ("categories.update", "categories", "update"),      // 12
                ("categories.delete", "categories", "delete"),      // 13
                ("brands.create",     "brands",     "create"),      // 14
                ("brands.read",       "brands",     "read"),        // 15
                ("brands.update",     "brands",     "update"),      // 16
                ("brands.delete",     "brands",     "delete"),      // 17
                ("units.create",      "units",      "create"),      // 18
                ("units.read",        "units",      "read"),        // 19
                ("units.update",      "units",      "update"),      // 20
                ("units.delete",      "units",      "delete"),      // 21
                ("products.create",   "products",   "create"),      // 22
                ("products.read",     "products",   "read"),        // 23
                ("products.update",   "products",   "update"),      // 24
                ("products.delete",   "products",   "delete"),      // 25
                // Inventory
                ("inventory.read",     "inventory",  "read"),       // 26
                ("inventory.create",   "inventory",  "create"),     // 27
                ("inventory.update",   "inventory",  "update"),     // 28
                ("inventory.transfer", "inventory",  "transfer"),   // 29
                // Warehouses
                ("warehouses.manage", "warehouses", "manage"),      // 30
                // Finance
                ("chart-of-accounts.create", "chart-of-accounts", "create"), // 31
                ("chart-of-accounts.read",   "chart-of-accounts", "read"),   // 32
                ("chart-of-accounts.update", "chart-of-accounts", "update"), // 33
                ("chart-of-accounts.delete", "chart-of-accounts", "delete"), // 34
                ("journal-entries.create",   "journal-entries",   "create"), // 35
                ("journal-entries.read",     "journal-entries",   "read"),   // 36
                ("journal-entries.post",     "journal-entries",   "post"),   // 37
                ("journal-entries.void",     "journal-entries",   "void"),   // 38
                ("journal-entries.delete",   "journal-entries",   "delete"), // 39
                ("payment-terms.create",     "payment-terms",    "create"), // 40
                ("payment-terms.read",       "payment-terms",    "read"),   // 41
                ("payment-terms.update",     "payment-terms",    "update"), // 42
                ("payment-terms.delete",     "payment-terms",    "delete"), // 43
                // Tax
                ("tax-rates.create",              "tax-rates",    "create"),           // 44
                ("tax-rates.read",                "tax-rates",    "read"),             // 45
                ("tax-rates.update",              "tax-rates",    "update"),           // 46
                ("tax-rates.delete",              "tax-rates",    "delete"),           // 47
                ("tax-rates.assign",              "tax-rates",    "assign"),           // 48
                ("invoices.create",               "invoices",     "create"),           // 49
                ("invoices.read",                 "invoices",     "read"),             // 50
                ("invoices.issue",                "invoices",     "issue"),            // 51
                ("invoices.assign-tax-number",    "invoices",     "assign-tax-number"),// 52
                ("invoices.pay",                  "invoices",     "pay"),              // 53
                ("invoices.cancel",               "invoices",     "cancel"),           // 54
                ("invoices.delete",               "invoices",     "delete"),           // 55
                // Audit
                ("audit-logs.read",               "audit-logs",   "read"),             // 56
                ("audit-logs.export",             "audit-logs",   "export"),           // 57
                ("system-logs.read",              "system-logs",  "read"),             // 58
                // Settings
                ("tenant-settings.read",          "tenant-settings",  "read"),         // 59
                ("tenant-settings.update",        "tenant-settings",  "update"),       // 60
                ("system-settings.read",          "system-settings",  "read"),         // 61
                ("system-settings.update",        "system-settings",  "update"),       // 62
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

            // Manager: full operational access (kept domains only)
            foreach (var p in new[] {
                "users.read",
                "categories.create", "categories.read", "categories.update", "categories.delete",
                "brands.create", "brands.read", "brands.update", "brands.delete",
                "units.create", "units.read", "units.update", "units.delete",
                "products.create", "products.read", "products.update", "products.delete",
                "inventory.read", "inventory.create", "inventory.update", "inventory.transfer",
                "warehouses.manage",
                // Finance (read + invoices)
                "chart-of-accounts.read", "journal-entries.read", "payment-terms.read",
                "invoices.create", "invoices.read", "invoices.issue", "invoices.pay",
                // Tax
                "tax-rates.read", "tax-rates.assign",
                // Audit
                "audit-logs.read",
                // Settings
                "tenant-settings.read", "tenant-settings.update"
            })
                allRolePerms.Add(new RolePermission { RoleId = managerRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            // Viewer: read-only everything (kept domains only)
            foreach (var p in new[] {
                "categories.read", "brands.read", "units.read", "products.read", "inventory.read",
                "warehouses.manage",
                "chart-of-accounts.read", "journal-entries.read", "payment-terms.read",
                "tax-rates.read", "invoices.read",
                "audit-logs.read",
                "tenant-settings.read"
            })
                allRolePerms.Add(new RolePermission { RoleId = viewerRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            // Warehouse Staff: inventory, stock movements, warehouses
            foreach (var p in new[] {
                "products.read", "categories.read", "brands.read", "units.read",
                "inventory.read", "inventory.create", "inventory.update", "inventory.transfer",
                "warehouses.manage"
            })
                allRolePerms.Add(new RolePermission { RoleId = warehouseStaffRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            // Accountant: finance, tax, invoicing
            foreach (var p in new[] {
                "chart-of-accounts.create", "chart-of-accounts.read", "chart-of-accounts.update", "chart-of-accounts.delete",
                "journal-entries.create", "journal-entries.read", "journal-entries.post", "journal-entries.void", "journal-entries.delete",
                "payment-terms.create", "payment-terms.read", "payment-terms.update", "payment-terms.delete",
                "tax-rates.create", "tax-rates.read", "tax-rates.update", "tax-rates.delete", "tax-rates.assign",
                "invoices.create", "invoices.read", "invoices.issue", "invoices.assign-tax-number", "invoices.pay", "invoices.cancel", "invoices.delete",
                "products.read",
                "audit-logs.read", "audit-logs.export",
                "tenant-settings.read"
            })
                allRolePerms.Add(new RolePermission { RoleId = accountantRoleId, PermissionId = permIds[p], TenantId = DefaultTenantId });

            modelBuilder.Entity<RolePermission>().HasData(allRolePerms);
        }

        // ── Seed: Users ─────────────────────────────────────────────────────
        private static void SeedUsers(ModelBuilder modelBuilder)
        {
            const string passwordHash = "$2a$12$/1CjZqaBIZTbTErAYBktTuw/iK9Y1I.BYKu7J1B9ZWSh5KQhFw9Gy"; // password123
            var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var adminUserId          = new Guid("10000000-0000-0000-0000-000000000001");
            var managerUserId        = new Guid("10000000-0000-0000-0000-000000000002");
            var viewerUserId         = new Guid("10000000-0000-0000-0000-000000000004");
            var warehouseStaffUserId = new Guid("10000000-0000-0000-0000-000000000006");
            var accountantUserId     = new Guid("10000000-0000-0000-0000-000000000007");

            modelBuilder.Entity<User>().HasData(
                new User { Id = adminUserId,          Name = "Alice Admin",          Email = "admin@stockledger.io",        PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = managerUserId,        Name = "Marcus Manager",       Email = "manager@stockledger.io",      PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = viewerUserId,         Name = "Victor Viewer",        Email = "viewer@stockledger.io",       PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = warehouseStaffUserId, Name = "Wira Warehouse",       Email = "warehouse@stockledger.io",    PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId },
                new User { Id = accountantUserId,     Name = "Andi Accountant",      Email = "accountant@stockledger.io",   PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt, TenantId = DefaultTenantId }
            );

            var adminRoleId          = new Guid("11111111-1111-1111-1111-111111111111");
            var managerRoleId        = new Guid("22222222-2222-2222-2222-222222222222");
            var viewerRoleId         = new Guid("44444444-4444-4444-4444-444444444444");
            var warehouseStaffRoleId = new Guid("66666666-6666-6666-6666-666666666666");
            var accountantRoleId     = new Guid("77777777-7777-7777-7777-777777777777");

            modelBuilder.Entity<UserRoleAssignment>().HasData(
                new UserRoleAssignment { UserId = adminUserId,          RoleId = adminRoleId,          AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = managerUserId,        RoleId = managerRoleId,        AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = viewerUserId,         RoleId = viewerRoleId,         AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = warehouseStaffUserId, RoleId = warehouseStaffRoleId, AssignedAt = createdAt, TenantId = DefaultTenantId },
                new UserRoleAssignment { UserId = accountantUserId,     RoleId = accountantRoleId,     AssignedAt = createdAt, TenantId = DefaultTenantId }
            );
        }

        // ── Seed: Product Catalog (Categories, Brands, UoM, Products, Variants, Images, Tax) ──
        private static void SeedProductCatalog(ModelBuilder modelBuilder)
        {
            var t = DefaultTenantId;
            var d = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // ── Categories ──────────────────────────────────────────────────
            var catBeverage   = new Guid("c0000000-0000-0000-0000-000000000001");
            var catSnack      = new Guid("c0000000-0000-0000-0000-000000000002");
            var catDairy      = new Guid("c0000000-0000-0000-0000-000000000003");
            var catRice       = new Guid("c0000000-0000-0000-0000-000000000004");
            var catPersonal   = new Guid("c0000000-0000-0000-0000-000000000005");
            var catCoffee     = new Guid("c0000000-0000-0000-0000-000000000006"); // child of Beverage

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = catBeverage, Name = "Beverages",      Slug = "beverages",      Description = "Drinks, juices, water, and ready-to-drink beverages", SortOrder = 1, IsActive = true, TenantId = t, CreatedAt = d },
                new Category { Id = catSnack,    Name = "Snacks",         Slug = "snacks",         Description = "Chips, crackers, biscuits, and packaged snacks",      SortOrder = 2, IsActive = true, TenantId = t, CreatedAt = d },
                new Category { Id = catDairy,    Name = "Dairy",          Slug = "dairy",          Description = "Milk, yogurt, cheese, and dairy products",            SortOrder = 3, IsActive = true, TenantId = t, CreatedAt = d },
                new Category { Id = catRice,     Name = "Rice & Staples", Slug = "rice-and-staples", Description = "Rice, flour, cooking oil, and pantry essentials",   SortOrder = 4, IsActive = true, TenantId = t, CreatedAt = d },
                new Category { Id = catPersonal, Name = "Personal Care",  Slug = "personal-care",  Description = "Soap, shampoo, toothpaste, and hygiene products",     SortOrder = 5, IsActive = true, TenantId = t, CreatedAt = d },
                new Category { Id = catCoffee,   Name = "Coffee & Tea",   Slug = "coffee-and-tea", Description = "Ground coffee, instant coffee, and tea",              SortOrder = 1, IsActive = true, TenantId = t, CreatedAt = d, ParentCategoryId = catBeverage }
            );

            // ── Brands ──────────────────────────────────────────────────────
            var brIndofood  = new Guid("b0000000-0000-0000-0000-000000000001");
            var brWings     = new Guid("b0000000-0000-0000-0000-000000000002");
            var brUnilever  = new Guid("b0000000-0000-0000-0000-000000000003");
            var brKapalApi  = new Guid("b0000000-0000-0000-0000-000000000004");
            var brUltraJaya = new Guid("b0000000-0000-0000-0000-000000000005");
            var brMayora    = new Guid("b0000000-0000-0000-0000-000000000006");
            var brAqua      = new Guid("b0000000-0000-0000-0000-000000000007");

            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = brIndofood,  Name = "Indofood",     Slug = "indofood",     Description = "PT Indofood Sukses Makmur — Indonesia's largest food company",   IsActive = true, TenantId = t, CreatedAt = d },
                new Brand { Id = brWings,     Name = "Wings",        Slug = "wings",        Description = "Wings Group — FMCG household and personal care",                 IsActive = true, TenantId = t, CreatedAt = d },
                new Brand { Id = brUnilever,  Name = "Unilever",     Slug = "unilever",     Description = "Unilever Indonesia — global consumer goods",                     IsActive = true, TenantId = t, CreatedAt = d },
                new Brand { Id = brKapalApi,  Name = "Kapal Api",    Slug = "kapal-api",    Description = "PT Santos Jaya Abadi — Indonesia's #1 coffee brand",             IsActive = true, TenantId = t, CreatedAt = d },
                new Brand { Id = brUltraJaya, Name = "Ultra Jaya",   Slug = "ultra-jaya",   Description = "PT Ultra Jaya Milk — UHT milk and dairy products",               IsActive = true, TenantId = t, CreatedAt = d },
                new Brand { Id = brMayora,    Name = "Mayora",       Slug = "mayora",       Description = "PT Mayora Indah — biscuits, confectionery, beverages",           IsActive = true, TenantId = t, CreatedAt = d },
                new Brand { Id = brAqua,      Name = "Aqua",         Slug = "aqua",         Description = "Danone-Aqua — Indonesia's leading bottled water",                IsActive = true, TenantId = t, CreatedAt = d }
            );

            // ── Units of Measure ────────────────────────────────────────────
            var uPcs   = new Guid("a0000000-0000-0000-0000-000000000001");
            var uBox   = new Guid("a0000000-0000-0000-0000-000000000002");
            var uKg    = new Guid("a0000000-0000-0000-0000-000000000003");
            var uLiter = new Guid("a0000000-0000-0000-0000-000000000004");
            var uDzn   = new Guid("a0000000-0000-0000-0000-000000000005");

            modelBuilder.Entity<UnitOfMeasure>().HasData(
                new UnitOfMeasure { Id = uPcs,   Name = "Piece",  Abbreviation = "pcs",  TenantId = t, CreatedAt = d },
                new UnitOfMeasure { Id = uBox,   Name = "Box",    Abbreviation = "box",  TenantId = t, CreatedAt = d },
                new UnitOfMeasure { Id = uKg,    Name = "Kilogram", Abbreviation = "kg", TenantId = t, CreatedAt = d },
                new UnitOfMeasure { Id = uLiter, Name = "Liter",  Abbreviation = "L",    TenantId = t, CreatedAt = d },
                new UnitOfMeasure { Id = uDzn,   Name = "Dozen",  Abbreviation = "dzn",  TenantId = t, CreatedAt = d }
            );

            // ── Unit Conversions ────────────────────────────────────────────
            modelBuilder.Entity<UnitConversion>().HasData(
                new UnitConversion { Id = new Guid("ac000000-0000-0000-0000-000000000001"), FromUnitId = uBox, ToUnitId = uPcs,  ConversionFactor = 12m,    TenantId = t, CreatedAt = d },  // 1 box = 12 pcs
                new UnitConversion { Id = new Guid("ac000000-0000-0000-0000-000000000002"), FromUnitId = uDzn, ToUnitId = uPcs,  ConversionFactor = 12m,    TenantId = t, CreatedAt = d },  // 1 dozen = 12 pcs
                new UnitConversion { Id = new Guid("ac000000-0000-0000-0000-000000000003"), FromUnitId = uKg,  ToUnitId = uKg,   ConversionFactor = 1000m,  TenantId = t, CreatedAt = d }   // placeholder: 1 kg = 1000 g (gram not seeded, so kg→kg as reference)
            );

            // ── Tax Rates ───────────────────────────────────────────────────
            var taxPPN11 = new Guid("ae000000-0000-0000-0000-000000000001");
            var taxExempt = new Guid("ae000000-0000-0000-0000-000000000002");

            modelBuilder.Entity<TaxRate>().HasData(
                new TaxRate { Id = taxPPN11,  Name = "PPN 11%",  Code = "PPN-11",  TaxType = "PPN",    Rate = 0.1100m, Description = "Pajak Pertambahan Nilai 11%", EffectiveFrom = new DateTime(2022, 4, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TenantId = t, CreatedAt = d },
                new TaxRate { Id = taxExempt, Name = "Exempt",   Code = "TAX-EX",  TaxType = "Exempt", Rate = 0.0000m, Description = "Tax exempt (basic necessities)", EffectiveFrom = new DateTime(2022, 4, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TenantId = t, CreatedAt = d }
            );

            // ── Products (10 items) ─────────────────────────────────────────
            var p01 = new Guid("d0000000-0000-0000-0000-000000000001");
            var p02 = new Guid("d0000000-0000-0000-0000-000000000002");
            var p03 = new Guid("d0000000-0000-0000-0000-000000000003");
            var p04 = new Guid("d0000000-0000-0000-0000-000000000004");
            var p05 = new Guid("d0000000-0000-0000-0000-000000000005");
            var p06 = new Guid("d0000000-0000-0000-0000-000000000006");
            var p07 = new Guid("d0000000-0000-0000-0000-000000000007");
            var p08 = new Guid("d0000000-0000-0000-0000-000000000008");
            var p09 = new Guid("d0000000-0000-0000-0000-000000000009");
            var p10 = new Guid("d0000000-0000-0000-0000-000000000010");

            modelBuilder.Entity<Product>().HasData(
                // 1. Indomie Goreng — Indofood, Snacks (instant noodle = snack category in warung)
                new Product { Id = p01, Name = "Indomie Mi Goreng",           Slug = "indomie-mi-goreng",           Description = "Indonesia's iconic instant fried noodle, original flavor",                 CategoryId = catSnack,    BrandId = brIndofood,  BaseUnitOfMeasureId = uPcs,   Status = "Active", IsActive = true, TenantId = t, CreatedAt = d },
                // 2. Aqua Mineral Water
                new Product { Id = p02, Name = "Aqua Mineral Water",          Slug = "aqua-mineral-water",          Description = "Natural mineral water from mountain springs",                              CategoryId = catBeverage, BrandId = brAqua,      BaseUnitOfMeasureId = uPcs,   Status = "Active", IsActive = true, TenantId = t, CreatedAt = d },
                // 3. Kapal Api Special
                new Product { Id = p03, Name = "Kapal Api Kopi Spesial",      Slug = "kapal-api-kopi-spesial",      Description = "Premium ground coffee blend, rich and aromatic",                           CategoryId = catCoffee,   BrandId = brKapalApi,  BaseUnitOfMeasureId = uPcs,   Status = "Active", IsActive = true, TenantId = t, CreatedAt = d },
                // 4. Ultra Milk UHT
                new Product { Id = p04, Name = "Ultra Milk UHT Full Cream",   Slug = "ultra-milk-uht-full-cream",   Description = "UHT processed full cream milk, rich in calcium and vitamin D",             CategoryId = catDairy,    BrandId = brUltraJaya, BaseUnitOfMeasureId = uPcs,   Status = "Active", IsActive = true, TenantId = t, CreatedAt = d },
                // 5. Beras Premium
                new Product { Id = p05, Name = "Beras Premium Pandan Wangi",  Slug = "beras-premium-pandan-wangi",  Description = "Premium fragrant pandan wangi rice, locally sourced from Central Java",    CategoryId = catRice,     BrandId = null,        BaseUnitOfMeasureId = uKg,    Status = "Active", IsActive = true, TenantId = t, CreatedAt = d },
                // 6. Minyak Goreng Bimoli
                new Product { Id = p06, Name = "Bimoli Minyak Goreng",        Slug = "bimoli-minyak-goreng",        Description = "Premium cooking oil, twice-filtered for clarity and health",                CategoryId = catRice,     BrandId = brIndofood,  BaseUnitOfMeasureId = uLiter, Status = "Active", IsActive = true, TenantId = t, CreatedAt = d },
                // 7. Roma Kelapa
                new Product { Id = p07, Name = "Roma Kelapa Biscuit",         Slug = "roma-kelapa-biscuit",         Description = "Classic coconut-flavored cream biscuit, a household favorite",              CategoryId = catSnack,    BrandId = brMayora,    BaseUnitOfMeasureId = uPcs,   Status = "Active", IsActive = true, TenantId = t, CreatedAt = d },
                // 8. Lifebuoy Sabun Mandi
                new Product { Id = p08, Name = "Lifebuoy Body Wash",          Slug = "lifebuoy-body-wash",          Description = "Antibacterial body wash with ActiveSilver formula",                        CategoryId = catPersonal, BrandId = brUnilever,  BaseUnitOfMeasureId = uPcs,   Status = "Active", IsActive = true, TenantId = t, CreatedAt = d },
                // 9. So Klin Deterjen
                new Product { Id = p09, Name = "So Klin Liquid Detergent",    Slug = "so-klin-liquid-detergent",     Description = "Concentrated liquid laundry detergent with softener",                      CategoryId = catPersonal, BrandId = brWings,     BaseUnitOfMeasureId = uPcs,   Status = "Active", IsActive = true, TenantId = t, CreatedAt = d },
                // 10. Teh Pucuk Harum
                new Product { Id = p10, Name = "Teh Pucuk Harum",            Slug = "teh-pucuk-harum",             Description = "Ready-to-drink jasmine green tea brewed from young tea leaves",             CategoryId = catBeverage, BrandId = brMayora,    BaseUnitOfMeasureId = uPcs,   Status = "Active", IsActive = true, TenantId = t, CreatedAt = d }
            );

            // ── Product Variants ────────────────────────────────────────────
            var v01a = new Guid("f0000000-0000-0000-0000-000000000001");
            var v01b = new Guid("f0000000-0000-0000-0000-000000000002");
            var v02a = new Guid("f0000000-0000-0000-0000-000000000003");
            var v02b = new Guid("f0000000-0000-0000-0000-000000000004");
            var v02c = new Guid("f0000000-0000-0000-0000-000000000005");
            var v03a = new Guid("f0000000-0000-0000-0000-000000000006");
            var v03b = new Guid("f0000000-0000-0000-0000-000000000007");
            var v04a = new Guid("f0000000-0000-0000-0000-000000000008");
            var v04b = new Guid("f0000000-0000-0000-0000-000000000009");
            var v05a = new Guid("f0000000-0000-0000-0000-000000000010");
            var v05b = new Guid("f0000000-0000-0000-0000-000000000011");
            var v06a = new Guid("f0000000-0000-0000-0000-000000000012");
            var v06b = new Guid("f0000000-0000-0000-0000-000000000013");
            var v07a = new Guid("f0000000-0000-0000-0000-000000000014");
            var v08a = new Guid("f0000000-0000-0000-0000-000000000015");
            var v08b = new Guid("f0000000-0000-0000-0000-000000000016");
            var v09a = new Guid("f0000000-0000-0000-0000-000000000017");
            var v09b = new Guid("f0000000-0000-0000-0000-000000000018");
            var v10a = new Guid("f0000000-0000-0000-0000-000000000019");
            var v10b = new Guid("f0000000-0000-0000-0000-000000000020");

            modelBuilder.Entity<ProductVariant>().HasData(
                // Indomie Mi Goreng
                new ProductVariant { Id = v01a, ProductId = p01, Name = "Indomie Mi Goreng - Single Pack",    Sku = "IDM-GRG-001", Barcode = "8901234560001", CostPrice =  2200m, SellingPrice =  3000m, Weight = 0.085m,  IsActive = true, TenantId = t, CreatedAt = d },
                new ProductVariant { Id = v01b, ProductId = p01, Name = "Indomie Mi Goreng - Box (40 pcs)",   Sku = "IDM-GRG-040", Barcode = "8901234560002", CostPrice = 82000m, SellingPrice = 110000m, Weight = 3.4m,    IsActive = true, TenantId = t, CreatedAt = d },
                // Aqua Mineral Water
                new ProductVariant { Id = v02a, ProductId = p02, Name = "Aqua 330ml",                        Sku = "AQU-330-001", Barcode = "8901234560003", CostPrice =  2000m, SellingPrice =  3500m, Weight = 0.33m,   IsActive = true, TenantId = t, CreatedAt = d },
                new ProductVariant { Id = v02b, ProductId = p02, Name = "Aqua 600ml",                        Sku = "AQU-600-001", Barcode = "8901234560004", CostPrice =  2800m, SellingPrice =  4500m, Weight = 0.6m,    IsActive = true, TenantId = t, CreatedAt = d },
                new ProductVariant { Id = v02c, ProductId = p02, Name = "Aqua 1500ml (Gallon-ette)",         Sku = "AQU-1500-01", Barcode = "8901234560005", CostPrice =  4500m, SellingPrice =  7500m, Weight = 1.5m,    IsActive = true, TenantId = t, CreatedAt = d },
                // Kapal Api Kopi Spesial
                new ProductVariant { Id = v03a, ProductId = p03, Name = "Kapal Api Special 165g",             Sku = "KPA-SPL-165", Barcode = "8901234560006", CostPrice = 12000m, SellingPrice = 17500m, Weight = 0.165m,  IsActive = true, TenantId = t, CreatedAt = d },
                new ProductVariant { Id = v03b, ProductId = p03, Name = "Kapal Api Special 380g",             Sku = "KPA-SPL-380", Barcode = "8901234560007", CostPrice = 25000m, SellingPrice = 35000m, Weight = 0.38m,   IsActive = true, TenantId = t, CreatedAt = d },
                // Ultra Milk UHT Full Cream
                new ProductVariant { Id = v04a, ProductId = p04, Name = "Ultra Milk Full Cream 250ml",        Sku = "ULT-FC-0250", Barcode = "8901234560008", CostPrice =  4500m, SellingPrice =  6500m, Weight = 0.26m,   IsActive = true, TenantId = t, CreatedAt = d },
                new ProductVariant { Id = v04b, ProductId = p04, Name = "Ultra Milk Full Cream 1000ml",       Sku = "ULT-FC-1000", Barcode = "8901234560009", CostPrice = 15000m, SellingPrice = 19500m, Weight = 1.03m,   IsActive = true, TenantId = t, CreatedAt = d },
                // Beras Premium Pandan Wangi
                new ProductVariant { Id = v05a, ProductId = p05, Name = "Beras Pandan Wangi 5kg",             Sku = "BRS-PW-005K", Barcode = "8901234560010", CostPrice = 62000m, SellingPrice = 75000m, Weight = 5.0m,    IsActive = true, TenantId = t, CreatedAt = d },
                new ProductVariant { Id = v05b, ProductId = p05, Name = "Beras Pandan Wangi 25kg",            Sku = "BRS-PW-025K", Barcode = "8901234560011", CostPrice = 290000m, SellingPrice = 350000m, Weight = 25.0m, IsActive = true, TenantId = t, CreatedAt = d },
                // Bimoli Minyak Goreng
                new ProductVariant { Id = v06a, ProductId = p06, Name = "Bimoli Minyak Goreng 1L",            Sku = "BMI-MG-001L", Barcode = "8901234560012", CostPrice = 17000m, SellingPrice = 22000m, Weight = 0.92m,   IsActive = true, TenantId = t, CreatedAt = d },
                new ProductVariant { Id = v06b, ProductId = p06, Name = "Bimoli Minyak Goreng 2L",            Sku = "BMI-MG-002L", Barcode = "8901234560013", CostPrice = 32000m, SellingPrice = 42000m, Weight = 1.84m,   IsActive = true, TenantId = t, CreatedAt = d },
                // Roma Kelapa Biscuit
                new ProductVariant { Id = v07a, ProductId = p07, Name = "Roma Kelapa 300g",                   Sku = "ROM-KLP-300", Barcode = "8901234560014", CostPrice =  8500m, SellingPrice = 12500m, Weight = 0.3m,    IsActive = true, TenantId = t, CreatedAt = d },
                // Lifebuoy Body Wash
                new ProductVariant { Id = v08a, ProductId = p08, Name = "Lifebuoy Body Wash 400ml - Cool",    Sku = "LFB-BW-400C", Barcode = "8901234560015", CostPrice = 22000m, SellingPrice = 32000m, Weight = 0.42m,   IsActive = true, TenantId = t, CreatedAt = d },
                new ProductVariant { Id = v08b, ProductId = p08, Name = "Lifebuoy Body Wash 400ml - Total",   Sku = "LFB-BW-400T", Barcode = "8901234560016", CostPrice = 22000m, SellingPrice = 32000m, Weight = 0.42m,   IsActive = true, TenantId = t, CreatedAt = d },
                // So Klin Liquid Detergent
                new ProductVariant { Id = v09a, ProductId = p09, Name = "So Klin Liquid 800ml",               Sku = "SKL-LQ-0800", Barcode = "8901234560017", CostPrice = 14000m, SellingPrice = 19500m, Weight = 0.82m,   IsActive = true, TenantId = t, CreatedAt = d },
                new ProductVariant { Id = v09b, ProductId = p09, Name = "So Klin Liquid 1600ml Refill",       Sku = "SKL-LQ-1600", Barcode = "8901234560018", CostPrice = 24000m, SellingPrice = 34000m, Weight = 1.62m,   IsActive = true, TenantId = t, CreatedAt = d },
                // Teh Pucuk Harum
                new ProductVariant { Id = v10a, ProductId = p10, Name = "Teh Pucuk Harum 350ml",              Sku = "TPH-350-001", Barcode = "8901234560019", CostPrice =  2500m, SellingPrice =  4000m, Weight = 0.36m,   IsActive = true, TenantId = t, CreatedAt = d },
                new ProductVariant { Id = v10b, ProductId = p10, Name = "Teh Pucuk Harum 480ml",              Sku = "TPH-480-001", Barcode = "8901234560020", CostPrice =  3200m, SellingPrice =  5500m, Weight = 0.49m,   IsActive = true, TenantId = t, CreatedAt = d }
            );

            // ── Product Images ──────────────────────────────────────────────
            modelBuilder.Entity<ProductImage>().HasData(
                new ProductImage { Id = new Guid("e1000000-0000-0000-0000-000000000001"), ProductId = p01, ImageUrl = "/images/products/indomie-goreng.jpg",       AltText = "Indomie Mi Goreng pack",       IsPrimary = true,  SortOrder = 1, TenantId = t, CreatedAt = d },
                new ProductImage { Id = new Guid("e1000000-0000-0000-0000-000000000002"), ProductId = p02, ImageUrl = "/images/products/aqua-mineral.jpg",         AltText = "Aqua mineral water bottle",    IsPrimary = true,  SortOrder = 1, TenantId = t, CreatedAt = d },
                new ProductImage { Id = new Guid("e1000000-0000-0000-0000-000000000003"), ProductId = p03, ImageUrl = "/images/products/kapal-api-special.jpg",    AltText = "Kapal Api Kopi Spesial pack",  IsPrimary = true,  SortOrder = 1, TenantId = t, CreatedAt = d },
                new ProductImage { Id = new Guid("e1000000-0000-0000-0000-000000000004"), ProductId = p04, ImageUrl = "/images/products/ultra-milk-fc.jpg",        AltText = "Ultra Milk Full Cream carton", IsPrimary = true,  SortOrder = 1, TenantId = t, CreatedAt = d },
                new ProductImage { Id = new Guid("e1000000-0000-0000-0000-000000000005"), ProductId = p05, ImageUrl = "/images/products/beras-pandan-wangi.jpg",   AltText = "Beras Pandan Wangi sack",      IsPrimary = true,  SortOrder = 1, TenantId = t, CreatedAt = d },
                new ProductImage { Id = new Guid("e1000000-0000-0000-0000-000000000006"), ProductId = p06, ImageUrl = "/images/products/bimoli-cooking-oil.jpg",   AltText = "Bimoli cooking oil bottle",    IsPrimary = true,  SortOrder = 1, TenantId = t, CreatedAt = d },
                new ProductImage { Id = new Guid("e1000000-0000-0000-0000-000000000007"), ProductId = p07, ImageUrl = "/images/products/roma-kelapa.jpg",          AltText = "Roma Kelapa biscuit pack",     IsPrimary = true,  SortOrder = 1, TenantId = t, CreatedAt = d },
                new ProductImage { Id = new Guid("e1000000-0000-0000-0000-000000000008"), ProductId = p08, ImageUrl = "/images/products/lifebuoy-body-wash.jpg",   AltText = "Lifebuoy body wash bottle",    IsPrimary = true,  SortOrder = 1, TenantId = t, CreatedAt = d },
                new ProductImage { Id = new Guid("e1000000-0000-0000-0000-000000000009"), ProductId = p09, ImageUrl = "/images/products/so-klin-liquid.jpg",       AltText = "So Klin liquid detergent",     IsPrimary = true,  SortOrder = 1, TenantId = t, CreatedAt = d },
                new ProductImage { Id = new Guid("e1000000-0000-0000-0000-000000000010"), ProductId = p10, ImageUrl = "/images/products/teh-pucuk-harum.jpg",      AltText = "Teh Pucuk Harum bottle",       IsPrimary = true,  SortOrder = 1, TenantId = t, CreatedAt = d }
            );

            // ── Product ↔ Tax Rate assignments ──────────────────────────────
            // Basic necessities (rice) are tax-exempt; everything else gets PPN 11%
            modelBuilder.Entity<ProductTaxRate>().HasData(
                new ProductTaxRate { ProductId = p01, TaxRateId = taxPPN11,  TenantId = t },  // Indomie
                new ProductTaxRate { ProductId = p02, TaxRateId = taxPPN11,  TenantId = t },  // Aqua
                new ProductTaxRate { ProductId = p03, TaxRateId = taxPPN11,  TenantId = t },  // Kapal Api
                new ProductTaxRate { ProductId = p04, TaxRateId = taxPPN11,  TenantId = t },  // Ultra Milk
                new ProductTaxRate { ProductId = p05, TaxRateId = taxExempt, TenantId = t },  // Beras — tax exempt (basic food)
                new ProductTaxRate { ProductId = p06, TaxRateId = taxExempt, TenantId = t },  // Minyak Goreng — tax exempt (basic food)
                new ProductTaxRate { ProductId = p07, TaxRateId = taxPPN11,  TenantId = t },  // Roma Kelapa
                new ProductTaxRate { ProductId = p08, TaxRateId = taxPPN11,  TenantId = t },  // Lifebuoy
                new ProductTaxRate { ProductId = p09, TaxRateId = taxPPN11,  TenantId = t },  // So Klin
                new ProductTaxRate { ProductId = p10, TaxRateId = taxPPN11,  TenantId = t }   // Teh Pucuk
            );
        }
    }
}
