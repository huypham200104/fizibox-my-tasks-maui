using MyTasksApp.Services.Interfaces;

namespace MyTasksApp.Tests.Mocks;

/// <summary>
/// Mock DateTimeProvider trả về ngày cố định phục vụ kiểm thử đơn vị độc lập.
/// </summary>
public class MockDateTimeProvider : IDateTimeProvider
{
    public DateTime MockDate { get; set; }

    public MockDateTimeProvider(DateTime? mockDate = null)
    {
        MockDate = mockDate ?? new DateTime(2026, 11, 23);
    }

    public DateTime Today => MockDate.Date;
    public DateTime Now => MockDate;
}
