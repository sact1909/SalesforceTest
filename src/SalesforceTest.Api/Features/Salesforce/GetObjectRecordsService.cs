using SalesforceTest.Api.Common;
using SalesforceTest.Api.DTOs.Salesforce;
using SalesforceTest.Api.Interfaces;

namespace SalesforceTest.Api.Features.Salesforce;

public sealed class GetObjectRecordsService
{
    private readonly ISalesforceConnectionRepository _connectionRepository;
    private readonly ISalesforceDataService _dataService;

    public GetObjectRecordsService(ISalesforceConnectionRepository connectionRepository, ISalesforceDataService dataService)
    {
        _connectionRepository = connectionRepository;
        _dataService = dataService;
    }

    public async Task<Result<SalesforceObjectRecordsDto>> ExecuteAsync(Guid userId, string objectApiName, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionRepository.GetByUserIdAsync(userId, cancellationToken);

        if (connection is null)
            return Result.Failure<SalesforceObjectRecordsDto>("No Salesforce connection found. Please connect your account first.");

        try
        {
            var records = await _dataService.GetObjectRecordsAsync(connection.InstanceUrl, connection.AccessToken, objectApiName, from, to, cancellationToken);
            return Result.Success(records);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<SalesforceObjectRecordsDto>(ex.Message);
        }
    }
}
