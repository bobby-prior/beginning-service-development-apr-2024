
namespace HelpDeskSupportApi.Services;

public class StandardBusinessClock(TimeProvider timeProvider) : IProvideTheBusinessClock
{
    public Task<bool> AreWeCurrentlyOpenAsync()
    {
        var now = timeProvider.GetLocalNow();

        var dayOfTheWeek = now.DayOfWeek;
        var hour = now.Hour;
        var openingTime = new TimeSpan(9, 0, 0);
        var closingTime = new TimeSpan(17, 0, 0);

        var isOpen = dayOfTheWeek switch
        {
            DayOfWeek.Sunday => false,
            DayOfWeek.Saturday => false,
            _ => hour >= openingTime.Hours && hour < closingTime.Hours
        };

        return Task.FromResult(isOpen);
    }
}
