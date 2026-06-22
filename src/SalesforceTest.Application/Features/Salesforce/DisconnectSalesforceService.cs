using SalesforceTest.Application.Common;
using SalesforceTest.Domain.Interfaces;

namespace SalesforceTest.Application.Features.Salesforce;

public sealed class DisconnectSalesforceService
{
    private readonly ISalesforceConnectionRepository _connectionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DisconnectSalesforceService(
        ISalesforceConnectionRepository connectionRepository,
        IUnitOfWork unitOfWork)
    {
        _connectionRepository = connectionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionRepository.GetByUserIdAsync(userId, cancellationToken);
        if (connection is null)
            return Result.Failure("No Salesforce connection found.");

        await _connectionRepository.RemoveAsync(connection, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
