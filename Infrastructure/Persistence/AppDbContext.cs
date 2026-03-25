using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Users ────────────────────────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // ── Drivers ──────────────────────────────────────────────────────────
            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Drivers)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Warehouses ───────────────────────────────────────────────────────
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            // ── Vehicles ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PlateNumber).IsUnique();
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            // ── Shipments ────────────────────────────────────────────────────────
            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TrackingNumber).IsUnique();
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.OriginWarehouse)
                      .WithMany(w => w.Shipments)
                      .HasForeignKey(e => e.OriginWarehouseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── ShipmentAssignments ──────────────────────────────────────────────
            modelBuilder.Entity<ShipmentAssignment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.Shipment)
                      .WithMany(s => s.ShipmentAssignments)
                      .HasForeignKey(e => e.ShipmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Vehicle)
                      .WithMany(v => v.ShipmentAssignments)
                      .HasForeignKey(e => e.VehicleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Driver)
                      .WithMany(d => d.ShipmentAssignments)
                      .HasForeignKey(e => e.DriverId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── ShipmentTracking ─────────────────────────────────────────────────
            modelBuilder.Entity<ShipmentTracking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.Shipment)
                      .WithMany(s => s.TrackingHistory)
                      .HasForeignKey(e => e.ShipmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Roles ─────────────────────────────────────────────────────────────
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IsSystem).HasDefaultValue(false);
            });

            // ── Permissions ───────────────────────────────────────────────────────
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Resource).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
                entity.HasIndex(e => new { e.Resource, e.Action }).IsUnique();
            });

            // ── UserRoleAssignments ───────────────────────────────────────────────
            modelBuilder.Entity<UserRoleAssignment>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(e => e.User)
                      .WithMany(u => u.UserRoleAssignments)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.UserRoleAssignments)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.AssignedAt).IsRequired();
                entity.Property(e => e.AssignedBy).IsRequired(false);
            });

            // ── RolePermissions ───────────────────────────────────────────────────
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.PermissionId });

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(e => e.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── RefreshTokens ─────────────────────────────────────────────────────
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.TokenHash).IsUnique();
                entity.Property(e => e.TokenHash).HasMaxLength(500).IsRequired();
                entity.Property(e => e.CreatedByIp).HasMaxLength(45);
                entity.Property(e => e.RevokedByIp).HasMaxLength(45);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
                entity.HasIndex(e => new { e.UserId, e.RevokedAt });

                entity.HasOne(e => e.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Ignore(e => e.IsActive);
            });

            // ── Seed Data ─────────────────────────────────────────────────────────
            SeedRolesAndPermissions(modelBuilder);
            SeedUsers(modelBuilder);
        }

        private static void SeedRolesAndPermissions(ModelBuilder modelBuilder)
        {
            var adminRoleId   = new Guid("11111111-1111-1111-1111-111111111111");
            var managerRoleId = new Guid("22222222-2222-2222-2222-222222222222");
            var driverRoleId  = new Guid("33333333-3333-3333-3333-333333333333");
            var viewerRoleId  = new Guid("44444444-4444-4444-4444-444444444444");

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = adminRoleId,   Name = "Admin",   Description = "Full system access",                  IsSystem = true },
                new Role { Id = managerRoleId, Name = "Manager", Description = "Manage shipments, drivers, vehicles", IsSystem = true },
                new Role { Id = driverRoleId,  Name = "Driver",  Description = "View assignments, update tracking",   IsSystem = true },
                new Role { Id = viewerRoleId,  Name = "Viewer",  Description = "Read-only access",                    IsSystem = true }
            );

            var perms = new[]
            {
                ("users.create",      "users",      "create"),
                ("users.read",        "users",      "read"),
                ("users.update",      "users",      "update"),
                ("users.delete",      "users",      "delete"),
                ("roles.assign",      "roles",      "assign"),
                ("shipments.create",  "shipments",  "create"),
                ("shipments.read",    "shipments",  "read"),
                ("shipments.update",  "shipments",  "update"),
                ("shipments.delete",  "shipments",  "delete"),
                ("shipments.assign",  "shipments",  "assign"),
                ("tracking.create",   "tracking",   "create"),
                ("tracking.read",     "tracking",   "read"),
                ("drivers.manage",    "drivers",    "manage"),
                ("vehicles.manage",   "vehicles",   "manage"),
                ("warehouses.manage", "warehouses", "manage"),
                ("roles.create",     "roles",      "create"),
                ("roles.read",       "roles",      "read"),
                ("roles.update",     "roles",      "update"),
                ("roles.delete",     "roles",      "delete"),
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
                    Action = perms[i].Item3
                });
            }

            modelBuilder.Entity<Permission>().HasData(permEntities);

            var allRolePerms = new List<RolePermission>();

            // Admin: all
            foreach (var permId in permIds.Values)
                allRolePerms.Add(new RolePermission { RoleId = adminRoleId, PermissionId = permId });

            // Manager
            foreach (var p in new[] { "users.read", "shipments.create", "shipments.read", "shipments.update", "shipments.assign", "tracking.create", "tracking.read", "drivers.manage", "vehicles.manage", "warehouses.manage" })
                allRolePerms.Add(new RolePermission { RoleId = managerRoleId, PermissionId = permIds[p] });

            // Driver
            foreach (var p in new[] { "shipments.read", "tracking.create", "tracking.read" })
                allRolePerms.Add(new RolePermission { RoleId = driverRoleId, PermissionId = permIds[p] });

            // Viewer
            foreach (var p in new[] { "shipments.read", "tracking.read" })
                allRolePerms.Add(new RolePermission { RoleId = viewerRoleId, PermissionId = permIds[p] });

            modelBuilder.Entity<RolePermission>().HasData(allRolePerms);
        }

        private static void SeedUsers(ModelBuilder modelBuilder)
        {
            // All seed users share the same password: P@ssw0rd123
            // BCrypt hash (work factor 12)
            const string passwordHash = "$2a$12$MpUTWj3KydCUJrXo0qo7Ley3zJJs7kn3.a717.gmxvNgrg63.lnhm";
            var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var adminUserId   = new Guid("10000000-0000-0000-0000-000000000001");
            var managerUserId = new Guid("10000000-0000-0000-0000-000000000002");
            var driverUserId  = new Guid("10000000-0000-0000-0000-000000000003");
            var viewerUserId  = new Guid("10000000-0000-0000-0000-000000000004");

            modelBuilder.Entity<User>().HasData(
                new User { Id = adminUserId,   Name = "Alice Admin",     Email = "admin@logistichub.com",   PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt },
                new User { Id = managerUserId, Name = "Marcus Manager",  Email = "manager@logistichub.com", PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt },
                new User { Id = driverUserId,  Name = "Diana Driver",    Email = "driver@logistichub.com",  PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt },
                new User { Id = viewerUserId,  Name = "Victor Viewer",   Email = "viewer@logistichub.com",  PasswordHash = passwordHash, IsActive = true, CreatedAt = createdAt }
            );

            var adminRoleId   = new Guid("11111111-1111-1111-1111-111111111111");
            var managerRoleId = new Guid("22222222-2222-2222-2222-222222222222");
            var driverRoleId  = new Guid("33333333-3333-3333-3333-333333333333");
            var viewerRoleId  = new Guid("44444444-4444-4444-4444-444444444444");

            modelBuilder.Entity<UserRoleAssignment>().HasData(
                new UserRoleAssignment { UserId = adminUserId,   RoleId = adminRoleId,   AssignedAt = createdAt },
                new UserRoleAssignment { UserId = managerUserId, RoleId = managerRoleId, AssignedAt = createdAt },
                new UserRoleAssignment { UserId = driverUserId,  RoleId = driverRoleId,  AssignedAt = createdAt },
                new UserRoleAssignment { UserId = viewerUserId,  RoleId = viewerRoleId,  AssignedAt = createdAt }
            );
        }
    }
}