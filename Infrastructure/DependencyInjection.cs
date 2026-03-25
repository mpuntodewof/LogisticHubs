using Application.Interfaces;
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

            // Use cases
            services.AddScoped<Application.UseCases.Auth.AuthUseCase>();
            services.AddScoped<Application.UseCases.Users.UserUseCase>();
            services.AddScoped<Application.UseCases.Shipments.ShipmentUseCase>();
            services.AddScoped<Application.UseCases.Drivers.DriverUseCase>();
            services.AddScoped<Application.UseCases.Vehicles.VehicleUseCase>();
            services.AddScoped<Application.UseCases.Warehouses.WarehouseUseCase>();
            services.AddScoped<Application.UseCases.Roles.RoleUseCase>();

            return services;
        }
    }
}
