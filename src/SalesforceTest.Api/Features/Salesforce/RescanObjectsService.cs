using SalesforceTest.Api.Common;
using SalesforceTest.Api.DTOs.Salesforce;
using SalesforceTest.Api.Entities;
using SalesforceTest.Api.Interfaces;
using System.Text.Json;

namespace SalesforceTest.Api.Features.Salesforce;

public sealed class RescanObjectsService
{
    private readonly ISalesforceConnectionRepository _connectionRepository;
    private readonly ISalesforceObjectCacheRepository _cacheRepository;
    private readonly ISalesforceDataService _dataService;
    private readonly IUnitOfWork _unitOfWork;

    public RescanObjectsService(
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

    public async Task<Result<CachedObjectsResult>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionRepository.GetByUserIdAsync(userId, cancellationToken);
        if (connection is null)
            return Result.Failure<CachedObjectsResult>("No Salesforce connection found. Please connect your account first.");

        try
        {
            var scanned = await _dataService.GetAvailableObjectsAsync(connection.InstanceUrl, connection.AccessToken, cancellationToken);
            var now = DateTime.UtcNow;

            await _cacheRepository.DeleteByUserIdAsync(userId, cancellationToken);

            var rows = scanned.Select(o => SalesforceObjectCache.Create(
                userId, o.ApiName, o.Label, o.LabelPlural, o.RecordCount,
                JsonSerializer.Serialize(o.Fields), now)).ToList();

            await _cacheRepository.AddRangeAsync(rows, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new CachedObjectsResult(scanned, now, FromCache: false));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<CachedObjectsResult>(ex.Message);
        }
    }
}
