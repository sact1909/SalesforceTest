using SalesforceTest.Application.Interfaces;

namespace SalesforceTest.Infrastructure.Services;

internal sealed class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
