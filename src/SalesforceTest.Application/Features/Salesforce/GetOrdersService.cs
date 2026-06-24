using SalesforceTest.Application.Common;
using SalesforceTest.Application.DTOs.Salesforce;
using SalesforceTest.Application.Interfaces;
using SalesforceTest.Domain.Interfaces;

namespace SalesforceTest.Application.Features.Salesforce;

public sealed class GetOrdersService
{
    private readonly ISalesforceConnectionRepository _connectionRepository;
    private readonly ISalesforceDataService _dataService;

    public GetOrdersService(ISalesforceConnectionRepository connectionRepository, ISalesforceDataService dataService)
    {
        _connectionRepository = connectionRepository;
        _dataService = dataService;
    }

    public async Task<Result<IReadOnlyList<SalesforceOrderDto>>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionRepository.GetByUserIdAsync(userId, cancellationToken);

        if (connection is null)
            return Result.Failure<IReadOnlyList<SalesforceOrderDto>>("No Salesforce connection found. Please connect your account first.");

        try
        {
            var orders = await _dataService.GetOrdersAsync(connection.InstanceUrl, connection.AccessToken, cancellationToken);
            return Result.Success(orders);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<IReadOnlyList<SalesforceOrderDto>>(ex.Message);
        }
    }
}
