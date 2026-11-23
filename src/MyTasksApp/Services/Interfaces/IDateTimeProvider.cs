namespace MyTasksApp.Services.Interfaces;

/// <summary>
/// Abstraction cung cấp ngày giờ hệ thống nhằm hỗ trợ Dependency Injection và cô lập Unit Test.
/// </summary>
public interface IDateTimeProvider
{
    DateTime Today { get; }
    DateTime Now { get; }
}
