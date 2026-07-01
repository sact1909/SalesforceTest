using SalesforceTest.Application.Common;
using SalesforceTest.Application.DTOs.Salesforce;
using SalesforceTest.Application.Interfaces;
using SalesforceTest.Domain.Interfaces;

namespace SalesforceTest.Application.Features.Salesforce;

public sealed class GetObjectRecordsService
{
    private readonly ISalesforceConnectionRepository _connectionRepository;
    private readonly ISalesforceDataService _dataService;

    public GetObjectRecordsService(ISalesforceConnectionRepository connectionRepository, ISalesforceDataService dataService)
    {
        _connectionRepository = connectionRepository;
        _dataService = dataService;
    }

    public async Task<Result<SalesforceObjectRecordsDto>> ExecuteAsync(Guid userId, string objectApiName, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionRepository.GetByUserIdAsync(userId, cancellationToken);

        if (connection is null)
            return Result.Failure<SalesforceObjectRecordsDto>("No Salesforce connection found. Please connect your account first.");

        try
        {
            var records = await _dataService.GetObjectRecordsAsync(connection.InstanceUrl, connection.AccessToken, objectApiName, cancellationToken);
            return Result.Success(records);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<SalesforceObjectRecordsDto>(ex.Message);
        }
    }
}
