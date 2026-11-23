using MyTasksApp.Services.Interfaces;

namespace MyTasksApp.Services;

/// <summary>
/// Cung cấp thời gian thực tế của hệ điều hành.
/// </summary>
public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime Today => DateTime.Today;
    public DateTime Now => DateTime.Now;
}
