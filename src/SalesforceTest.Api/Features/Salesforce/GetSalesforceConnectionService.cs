using SalesforceTest.Api.Common;
using SalesforceTest.Api.DTOs.Salesforce;
using SalesforceTest.Api.Interfaces;

namespace SalesforceTest.Api.Features.Salesforce;

public sealed class GetSalesforceConnectionService
{
    private readonly ISalesforceConnectionRepository _connectionRepository;

    public GetSalesforceConnectionService(ISalesforceConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository;
    }

    public async Task<Result<SalesforceConnectionDto>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionRepository.GetByUserIdAsync(userId, cancellationToken);

        if (connection is null)
            return Result.Success(new SalesforceConnectionDto("", "", "", "", "", DateTime.MinValue, false));

        return Result.Success(new SalesforceConnectionDto(
            connection.SalesforceUserId,
            connection.OrganizationId,
            connection.DisplayName,
            connection.Email,
            connection.InstanceUrl,
            connection.TokenExpiresAt,
            true));
    }
}
