using SalesforceTest.Application.Common;
using SalesforceTest.Application.Interfaces;
using SalesforceTest.Domain.Entities;
using SalesforceTest.Domain.Interfaces;

namespace SalesforceTest.Application.Features.Salesforce;

public sealed class HandleOAuthCallbackService
{
    private readonly ISalesforceOAuthService _oauthService;
    private readonly ISalesforceConnectionRepository _connectionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public HandleOAuthCallbackService(
        ISalesforceOAuthService oauthService,
        ISalesforceConnectionRepository connectionRepository,
        IUnitOfWork unitOfWork)
    {
        _oauthService = oauthService;
        _connectionRepository = connectionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(string code, string state, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(state, out var userId))
            return Result.Failure("Invalid state parameter.");

        var tokens = await _oauthService.ExchangeCodeAsync(code, cancellationToken);
        var userInfo = await _oauthService.GetUserInfoAsync(tokens.InstanceUrl, tokens.AccessToken, cancellationToken);

        var tokenExpiresAt = DateTimeOffset.FromUnixTimeMilliseconds(tokens.IssuedAt).UtcDateTime.AddHours(1);

        var existing = await _connectionRepository.GetByUserIdAsync(userId, cancellationToken);
        if (existing is not null)
        {
            existing.UpdateTokens(tokens.AccessToken, tokens.RefreshToken, tokenExpiresAt);
        }
        else
        {
            var connection = SalesforceConnection.Create(
                userId,
                tokens.InstanceUrl,
                tokens.AccessToken,
                tokens.RefreshToken,
                userInfo.UserId,
                userInfo.OrganizationId,
                userInfo.DisplayName,
                userInfo.Email,
                tokenExpiresAt);

            await _connectionRepository.AddAsync(connection, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
