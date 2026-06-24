using SalesforceTest.Application.Common;
using SalesforceTest.Application.DTOs.Salesforce;
using SalesforceTest.Application.Interfaces;
using SalesforceTest.Domain.Interfaces;

namespace SalesforceTest.Application.Features.Salesforce;

public sealed class GetInvoicesService
{
    private readonly ISalesforceConnectionRepository _connectionRepository;
    private readonly ISalesforceDataService _dataService;

    public GetInvoicesService(ISalesforceConnectionRepository connectionRepository, ISalesforceDataService dataService)
    {
        _connectionRepository = connectionRepository;
        _dataService = dataService;
    }

    public async Task<Result<IReadOnlyList<SalesforceInvoiceDto>>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionRepository.GetByUserIdAsync(userId, cancellationToken);

        if (connection is null)
            return Result.Failure<IReadOnlyList<SalesforceInvoiceDto>>("No Salesforce connection found. Please connect your account first.");

        try
        {
            var invoices = await _dataService.GetInvoicesAsync(connection.InstanceUrl, connection.AccessToken, cancellationToken);
            return Result.Success(invoices);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<IReadOnlyList<SalesforceInvoiceDto>>(ex.Message);
        }
    }
}
