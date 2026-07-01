using SalesforceTest.Application.Common;
using SalesforceTest.Application.Interfaces;
using SalesforceTest.Domain.Interfaces;

namespace SalesforceTest.Application.Features.Salesforce;

public sealed class RefreshObjectCountService
{
    private readonly ISalesforceConnectionRepository _connectionRepository;
    private readonly ISalesforceObjectCacheRepository _cacheRepository;
    private readonly ISalesforceDataService _dataService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshObjectCountService(
        ISalesforceConnectionRepository connectionRepository,
        ISalesforceObjectCacheRepository cacheRepository,
        ISalesforceDataService dataService,
        IUnitOfWork unitOfWork)
    {
        _connectionRepository = connectionRepository;
        _cacheRepository = cacheRepository;
        _dataService = dataService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> ExecuteAsync(Guid userId, string objectApiName, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionRepository.GetByUserIdAsync(userId, cancellationToken);
        if (connection is null)
            return Result.Failure<int>("No Salesforce connection found.");

        var cached = await _cacheRepository.GetByUserAndApiNameAsync(userId, objectApiName, cancellationToken);
        if (cached is null)
            return Result.Failure<int>($"Object '{objectApiName}' not found in cache.");

        try
        {
            var count = await _dataService.GetObjectCountAsync(connection.InstanceUrl, connection.AccessToken, objectApiName, cancellationToken);
            cached.UpdateCount(count);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(count);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<int>(ex.Message);
        }
    }
}
