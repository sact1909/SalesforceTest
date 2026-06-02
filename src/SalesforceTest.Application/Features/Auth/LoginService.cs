using SalesforceTest.Application.Common;
using SalesforceTest.Application.DTOs.Auth;
using SalesforceTest.Application.Interfaces;
using SalesforceTest.Domain.Interfaces;

namespace SalesforceTest.Application.Features.Auth;

public sealed class LoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<LoginResponse>("Invalid username or password.");

        if (!user.IsActive)
            return Result.Failure<LoginResponse>("Account is disabled.");

        user.RecordLogin();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var (token, expiresAt) = _tokenService.GenerateToken(user);

        return Result.Success(new LoginResponse(
            Token: token,
            Username: user.Username,
            Email: user.Email,
            FullName: $"{user.FirstName} {user.LastName}".Trim(),
            ExpiresAt: expiresAt
        ));
    }
}
