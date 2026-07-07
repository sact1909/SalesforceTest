using SalesforceTest.Api.Common;
using SalesforceTest.Api.DTOs.Salesforce;
using SalesforceTest.Api.Interfaces;

namespace SalesforceTest.Api.Features.Salesforce;

public sealed class GetContactsService
{
    private readonly ISalesforceConnectionRepository _connectionRepository;
    private readonly ISalesforceDataService _dataService;

    public GetContactsService(ISalesforceConnectionRepository connectionRepository, ISalesforceDataService dataService)
    {
        _connectionRepository = connectionRepository;
        _dataService = dataService;
    }

    public async Task<Result<IReadOnlyList<SalesforceContactDto>>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionRepository.GetByUserIdAsync(userId, cancellationToken);

        if (connection is null)
            return Result.Failure<IReadOnlyList<SalesforceContactDto>>("No Salesforce connection found. Please connect your account first.");

        try
        {
            var contacts = await _dataService.GetContactsAsync(connection.InstanceUrl, connection.AccessToken, cancellationToken);
            return Result.Success(contacts);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<IReadOnlyList<SalesforceContactDto>>(ex.Message);
        }
    }
}
