using SalesforceTest.Application.Common;
using SalesforceTest.Application.DTOs.Salesforce;
using SalesforceTest.Application.Interfaces;
using SalesforceTest.Domain.Interfaces;

namespace SalesforceTest.Application.Features.Salesforce;

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
