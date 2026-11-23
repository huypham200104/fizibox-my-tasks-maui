using FluentAssertions;
using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services;
using MyTasksApp.Tests.Mocks;
using MyTasksApp.ViewModels;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;
using Xunit;

namespace MyTasksApp.Tests;

public class HistoryViewModelTests
{
    [Fact]
    public async Task LoadHistoryCommand_ShouldPopulateGroupedTasksAndCompletedCount()
    {
        // Arrange
        var service = new MockTaskService();
        var localization = new LocalizationService();
        var viewModel = new HistoryViewModel(service, localization);

        // Act
        await viewModel.LoadHistoryCommand.ExecuteAsync();

        // Assert
        viewModel.IsBusy.Should().BeFalse();
        viewModel.CompletedTasksCount.Should().BeGreaterThan(0);
        viewModel.GroupedTasks.Should().NotBeEmpty();
        viewModel.HasCompletedTasks.Should().BeTrue();
        viewModel.IsEmpty.Should().BeFalse();
        viewModel.IsSearchEmpty.Should().BeFalse();

        // Kiểm tra tất cả công việc trong các nhóm đều đã hoàn thành
        foreach (var group in viewModel.GroupedTasks)
        {
            group.Tasks.Should().NotBeEmpty();
            foreach (var task in group.Tasks)
            {
                task.Status.Should().Be(TaskStatus.Completed);
            }
        }
    }

    [Fact]
    public async Task FilterByPeriod_ShouldCorrectlyFilterTasks()
    {
        // Arrange
        var service = new MockTaskService();
        var localization = new LocalizationService();
        var dateTimeProvider = new MockDateTimeProvider(new DateTime(2026, 11, 21)); // Thứ Bảy (có 2 completed tasks trong seed)
        var viewModel = new HistoryViewModel(service, localization, dateTimeProvider);
        await viewModel.LoadHistoryCommand.ExecuteAsync();

        var initialAllCount = viewModel.CompletedTasksCount;

        // Act & Assert: Lọc Hôm nay
        await viewModel.SelectFilterCommand.ExecuteAsync("Today");
        viewModel.IsFilterToday.Should().BeTrue();
        viewModel.FilterPeriodText.Should().Contain("hôm nay");
        foreach (var group in viewModel.GroupedTasks)
        {
            group.Date.Date.Should().Be(dateTimeProvider.Today);
        }

        // Act & Assert: Lọc Tuần này
        await viewModel.SelectFilterCommand.ExecuteAsync("ThisWeek");
        viewModel.IsFilterWeek.Should().BeTrue();
        DateTime monday = WeekViewModel.GetMondayOfWeek(dateTimeProvider.Today);
        DateTime sunday = monday.AddDays(6);
        foreach (var group in viewModel.GroupedTasks)
        {
            group.Date.Date.Should().BeOnOrAfter(monday).And.BeOnOrBefore(sunday);
        }

        // Act & Assert: Lọc Tháng này
        await viewModel.SelectFilterCommand.ExecuteAsync("ThisMonth");
        viewModel.IsFilterMonth.Should().BeTrue();
        foreach (var group in viewModel.GroupedTasks)
        {
            group.Date.Year.Should().Be(dateTimeProvider.Today.Year);
            group.Date.Month.Should().Be(dateTimeProvider.Today.Month);
        }

        // Act & Assert: Lọc Tất cả
        await viewModel.SelectFilterCommand.ExecuteAsync("All");
        viewModel.IsFilterAll.Should().BeTrue();
        viewModel.CompletedTasksCount.Should().Be(initialAllCount);
    }

    [Fact]
    public async Task SearchText_ShouldFilterTasksRealtime()
    {
        // Arrange
        var service = new MockTaskService();
        var localization = new LocalizationService();
        var viewModel = new HistoryViewModel(service, localization);
        await viewModel.LoadHistoryCommand.ExecuteAsync();

        var totalCount = viewModel.CompletedTasksCount;

        // Act: Tìm kiếm từ khóa có trong dữ liệu
        viewModel.SearchText = "MyTasks";

        // Assert
        viewModel.HasSearchText.Should().BeTrue();
        viewModel.CompletedTasksCount.Should().BeLessThanOrEqualTo(totalCount);
        foreach (var group in viewModel.GroupedTasks)
        {
            foreach (var task in group.Tasks)
            {
                bool matches = task.Title.Contains("MyTasks", StringComparison.OrdinalIgnoreCase) ||
                               task.ProjectName.Contains("MyTasks", StringComparison.OrdinalIgnoreCase);
                matches.Should().BeTrue();
            }
        }

        // Act: Xóa tìm kiếm
        viewModel.ClearSearchCommand.Execute(null);

        // Assert
        viewModel.SearchText.Should().BeEmpty();
        viewModel.HasSearchText.Should().BeFalse();
        viewModel.CompletedTasksCount.Should().Be(totalCount);
    }

    [Fact]
    public async Task ToggleSortOrder_ShouldReverseGroupAndTaskDates()
    {
        // Arrange
        var service = new MockTaskService();
        var localization = new LocalizationService();
        var dateTimeProvider = new MockDateTimeProvider(new DateTime(2026, 11, 21));
        var viewModel = new HistoryViewModel(service, localization, dateTimeProvider);
        await viewModel.LoadHistoryCommand.ExecuteAsync();

        // Ban đầu sắp xếp mới nhất lên đầu
        viewModel.IsSortDescending.Should().BeTrue();
        if (viewModel.GroupedTasks.Count > 1)
        {
            viewModel.GroupedTasks[0].Date.Should().BeOnOrAfter(viewModel.GroupedTasks[^1].Date);
        }

        // Act: Đổi chiều sắp xếp (cũ nhất lên đầu)
        await viewModel.ToggleSortOrderCommand.ExecuteAsync(null);

        // Assert
        viewModel.IsSortDescending.Should().BeFalse();
        if (viewModel.GroupedTasks.Count > 1)
        {
            viewModel.GroupedTasks[0].Date.Should().BeOnOrBefore(viewModel.GroupedTasks[^1].Date);
        }
    }

    [Fact]
    public async Task EmptyStates_ShouldBehaveCorrectly()
    {
        // Arrange
        var service = new MockTaskService();
        var localization = new LocalizationService();
        var viewModel = new HistoryViewModel(service, localization);
        await viewModel.LoadHistoryCommand.ExecuteAsync();

        // 1. Khi tìm kiếm từ khóa không tồn tại
        viewModel.SearchText = "NonExistentTaskXYZ123";
        viewModel.CompletedTasksCount.Should().Be(0);
        viewModel.HasCompletedTasks.Should().BeFalse();
        viewModel.IsSearchEmpty.Should().BeTrue();
        viewModel.IsEmpty.Should().BeFalse();

        // 2. Khi SimulateEmpty được kích hoạt
        service.SimulateEmpty = true;
        await viewModel.RefreshCommand.ExecuteAsync();
        viewModel.SearchText = string.Empty;
        viewModel.CompletedTasksCount.Should().Be(0);
        viewModel.HasCompletedTasks.Should().BeFalse();
        viewModel.IsEmpty.Should().BeTrue();
        viewModel.IsSearchEmpty.Should().BeFalse();
    }

    [Fact]
    public async Task OnLanguageChanged_ShouldUpdateAllLabelsInstantly()
    {
        // Arrange
        var service = new MockTaskService();
        var localization = new LocalizationService();
        var viewModel = new HistoryViewModel(service, localization);
        await viewModel.LoadHistoryCommand.ExecuteAsync();

        // Tiếng Việt mặc định
        viewModel.PageTitle.Should().Be("Lịch sử");
        viewModel.CompletedBadge.Should().Be("Đã hoàn thành");

        // Act: Đổi sang English
        localization.SetLanguage(AppLanguage.English);

        // Assert
        viewModel.PageTitle.Should().Be("History");
        viewModel.CompletedBadge.Should().Be("Completed");
        viewModel.FilterAllText.Should().Be("All");
        viewModel.FilterTodayText.Should().Be("Today");
    }
}
