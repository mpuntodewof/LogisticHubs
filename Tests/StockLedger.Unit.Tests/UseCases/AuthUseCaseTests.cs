using Application.DTOs.Auth;
using Application.Interfaces;
using Application.UseCases.Auth;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace StockLedger.Unit.Tests.UseCases;

public class AuthUseCaseTests
{
    private readonly IAuthRepository _authRepo = Substitute.For<IAuthRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly ITenantContext _tenantContext = Substitute.For<ITenantContext>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly AuthUseCase _sut;

    public AuthUseCaseTests()
    {
        _sut = new AuthUseCase(_authRepo, _passwordHasher, _tokenService, _tenantContext, _unitOfWork);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsLoginResponse()
    {
        var tenantId = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            PasswordHash = "hashed",
            IsActive = true,
            TenantId = tenantId
        };

        _authRepo.GetUserByEmailUnfilteredAsync("test@example.com").Returns(user);
        _passwordHasher.Verify("Password1!", "hashed").Returns(true);
        _authRepo.GetUserRoleNamesAsync(user.Id).Returns(new List<string> { "Admin" });
        _authRepo.GetUserPermissionNamesAsync(user.Id).Returns(new List<string> { "products.read" });
        _tokenService.GenerateAccessToken(user, Arg.Any<List<string>>(), Arg.Any<List<string>>(), tenantId)
            .Returns("access-token");
        _tokenService.GenerateRefreshToken().Returns(("raw-refresh", "hashed-refresh"));

        var result = await _sut.LoginAsync(new LoginRequest { Email = "test@example.com", Password = "Password1!" }, "127.0.0.1");

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("raw-refresh");
        result.User.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsUnauthorized()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hashed",
            IsActive = true,
            TenantId = Guid.NewGuid()
        };

        _authRepo.GetUserByEmailUnfilteredAsync("test@example.com").Returns(user);
        _passwordHasher.Verify("wrong", "hashed").Returns(false);

        var act = () => _sut.LoginAsync(new LoginRequest { Email = "test@example.com", Password = "wrong" }, null);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_NonexistentUser_StillCallsVerify_PreventsTimingAttack()
    {
        _authRepo.GetUserByEmailUnfilteredAsync("nobody@example.com").ReturnsNull();
        // The dummy hash is generated lazily from _passwordHasher.Hash()
        _passwordHasher.Hash("timing-safe-dummy").Returns("$2a$12$dummyhash");
        _passwordHasher.Verify("anypass", "$2a$12$dummyhash").Returns(false);

        var act = () => _sut.LoginAsync(new LoginRequest { Email = "nobody@example.com", Password = "anypass" }, null);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        // Verify that password hash was still checked (timing attack mitigation)
        _passwordHasher.Received(1).Verify("anypass", Arg.Any<string>());
    }

    [Fact]
    public async Task LoginAsync_DisabledAccount_ThrowsUnauthorized()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "disabled@example.com",
            PasswordHash = "hashed",
            IsActive = false,
            TenantId = Guid.NewGuid()
        };

        _authRepo.GetUserByEmailUnfilteredAsync("disabled@example.com").Returns(user);
        _passwordHasher.Verify("Password1!", "hashed").Returns(true);

        var act = () => _sut.LoginAsync(new LoginRequest { Email = "disabled@example.com", Password = "Password1!" }, null);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Account is disabled.");
    }

    [Fact]
    public async Task RegisterAsync_NewTenant_CreatesAdminRole()
    {
        var adminRole = new Role { Id = Guid.NewGuid(), Name = "Admin", TenantId = Guid.NewGuid() };

        _authRepo.CreateTenantAsync(Arg.Any<Tenant>()).Returns(ci => ci.Arg<Tenant>());
        _authRepo.CreateUserAsync(Arg.Any<User>()).Returns(ci => ci.Arg<User>());
        _authRepo.GetRoleByNameAndTenantAsync("Admin", Arg.Any<Guid>()).Returns(adminRole);
        _passwordHasher.Hash("Password1!").Returns("hashed");

        var request = new RegisterRequest
        {
            Name = "Owner",
            Email = "owner@company.com",
            Password = "Password1!",
            CompanyName = "Test Company"
        };

        var userId = await _sut.RegisterAsync(request);

        userId.Should().NotBeEmpty();
        await _authRepo.Received(1).CreateTenantAsync(Arg.Any<Tenant>());
        await _authRepo.Received(1).AssignRoleToUserAsync(Arg.Any<Guid>(), adminRole.Id);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsConflict()
    {
        var existingUser = new User { Id = Guid.NewGuid(), Email = "existing@example.com", TenantId = Guid.NewGuid() };

        _authRepo.GetUserByEmailAsync("existing@example.com").Returns(existingUser);

        var request = new RegisterRequest
        {
            Name = "Dup",
            Email = "existing@example.com",
            Password = "Password1!",
            TenantId = Guid.NewGuid()
        };

        _authRepo.GetTenantByIdAsync(request.TenantId!.Value).Returns(new Tenant { Id = request.TenantId.Value, IsActive = true });

        var act = () => _sut.RegisterAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }
}
