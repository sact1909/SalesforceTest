using SalesforceTest.Application.Common;
using SalesforceTest.Application.DTOs.Salesforce;
using SalesforceTest.Application.Interfaces;
using SalesforceTest.Domain.Interfaces;

namespace SalesforceTest.Application.Features.Salesforce;

public sealed class GetAccountsService
{
    private readonly ISalesforceConnectionRepository _connectionRepository;
    private readonly ISalesforceDataService _dataService;

    public GetAccountsService(ISalesforceConnectionRepository connectionRepository, ISalesforceDataService dataService)
    {
        _connectionRepository = connectionRepository;
        _dataService = dataService;
    }

    public async Task<Result<IReadOnlyList<SalesforceAccountDto>>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionRepository.GetByUserIdAsync(userId, cancellationToken);

        if (connection is null)
            return Result.Failure<IReadOnlyList<SalesforceAccountDto>>("No Salesforce connection found. Please connect your account first.");

        try
        {
            var accounts = await _dataService.GetAccountsAsync(connection.InstanceUrl, connection.AccessToken, cancellationToken);
            return Result.Success(accounts);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<IReadOnlyList<SalesforceAccountDto>>(ex.Message);
        }
    }
}
