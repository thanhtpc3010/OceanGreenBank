using ProjectService.Application.Common.Interfaces;

namespace ProjectService.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime NowUtc => DateTime.Now;
}
