using FluentAssertions;
using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services;
using MyTasksApp.ViewModels;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;
using Xunit;

namespace MyTasksApp.Tests;

public class WeekViewModelTests
{
    [Fact]
    public async Task LoadWeekData_ShouldGenerate7DaysFromMondayToSunday()
    {
        // Arrange
        var service = new MockTaskService();
        var vm = new WeekViewModel(service);

        // Act
        await vm.LoadWeekDataCommand.ExecuteAsync();

        // Assert
        vm.WeekDays.Should().HaveCount(7);
        vm.WeekDays[0].DayOfWeekShort.Should().Be("T2");
        vm.WeekDays[0].Date.DayOfWeek.Should().Be(DayOfWeek.Monday);
        vm.WeekDays[6].DayOfWeekShort.Should().Be("CN");
        vm.WeekDays[6].Date.DayOfWeek.Should().Be(DayOfWeek.Sunday);

        vm.WeekRangeText.Should().NotBeNullOrWhiteSpace();
        vm.WeekRangeText.Should().Contain("–");

        // Selected day must be marked
        vm.WeekDays.Should().ContainSingle(d => d.IsSelected);
    }

    [Fact]
    public async Task PreviousWeek_And_NextWeek_ShouldShift7DaysAndPreserveDayOfWeek()
    {
        // Arrange
        var service = new MockTaskService();
        var vm = new WeekViewModel(service);
        await vm.LoadWeekDataCommand.ExecuteAsync();

        var initialWeekStart = vm.CurrentWeekStart;
        var initialSelectedDayOfWeek = vm.SelectedDate.DayOfWeek;

        // Act: Next week
        await vm.NextWeekCommand.ExecuteAsync();

        // Assert
        vm.CurrentWeekStart.Should().Be(initialWeekStart.AddDays(7));
        vm.SelectedDate.DayOfWeek.Should().Be(initialSelectedDayOfWeek);
        vm.SelectedDate.Should().Be(initialWeekStart.AddDays(7).AddDays((int)(initialSelectedDayOfWeek == DayOfWeek.Sunday ? 6 : (int)initialSelectedDayOfWeek - 1)));

        // Act: Previous week twice
        await vm.PreviousWeekCommand.ExecuteAsync();
        await vm.PreviousWeekCommand.ExecuteAsync();

        // Assert
        vm.CurrentWeekStart.Should().Be(initialWeekStart.AddDays(-7));
        vm.SelectedDate.DayOfWeek.Should().Be(initialSelectedDayOfWeek);
    }

    [Fact]
    public async Task SelectDay_ShouldUpdateSelectedDateAndFilterDayTasksInChronologicalOrder()
    {
        // Arrange
        var service = new MockTaskService();
        var vm = new WeekViewModel(service);
        await vm.LoadWeekDataCommand.ExecuteAsync();

        // Chọn ngày Thứ Tư trong tuần (index 2)
        var targetDay = vm.WeekDays[2];

        // Act
        await vm.SelectDayCommand.ExecuteAsync(targetDay);

        // Assert
        vm.SelectedDate.Date.Should().Be(targetDay.Date.Date);
        targetDay.IsSelected.Should().BeTrue();
        vm.WeekDays.Where(d => d != targetDay).All(d => !d.IsSelected).Should().BeTrue();

        // Kiểm tra danh sách tasks sắp xếp tăng dần theo DueTime
        for (int i = 0; i < vm.DayTasks.Count - 1; i++)
        {
            vm.DayTasks[i].DueTime.Should().BeLessThanOrEqualTo(vm.DayTasks[i + 1].DueTime);
        }
    }

    [Fact]
    public async Task DayProgress_ShouldCalculateAccurateMetrics()
    {
        // Arrange
        var service = new MockTaskService();
        var vm = new WeekViewModel(service);
        await vm.LoadWeekDataCommand.ExecuteAsync();

        // Assert
        vm.DayTotalTasksCount.Should().Be(vm.DayTasks.Count);
        if (vm.DayTotalTasksCount > 0)
        {
            vm.DayProgressRatio.Should().BeInRange(0.0, 1.0);
            vm.DayProgressPercentageText.Should().EndWith("%");
            vm.DayProgressRatioText.Should().Contain("hoàn thành");
        }
        else
        {
            vm.DayProgressRatio.Should().Be(0.0);
            vm.DayProgressPercentageText.Should().Be("0%");
        }
    }

    [Fact]
    public async Task ToggleTaskStatus_ShouldKeepTaskInPlaceAndToggleStatus()
    {
        // Arrange
        var service = new MockTaskService();
        var vm = new WeekViewModel(service);
        await vm.LoadWeekDataCommand.ExecuteAsync();

        // Chọn ngày có sẵn công việc trong mock data
        var targetDay = vm.WeekDays.First(d => d.TaskCount > 0);
        await vm.SelectDayCommand.ExecuteAsync(targetDay);

        vm.DayTasks.Should().NotBeEmpty();
        var task = vm.DayTasks[1]; // Việc thứ 2
        var originalIndex = vm.DayTasks.IndexOf(task);
        var originalStatus = task.Status;

        // Act: Toggle status
        await vm.ToggleTaskStatusCommand.ExecuteAsync(task);

        // Assert: Vị trí trong danh sách không đổi, status đổi
        vm.DayTasks.IndexOf(task).Should().Be(originalIndex);
        task.Status.Should().NotBe(originalStatus);
    }

    [Fact]
    public async Task DeleteTask_ShouldRemoveFromDayTasksAndUpdateProgress()
    {
        // Arrange
        var service = new MockTaskService();
        var vm = new WeekViewModel(service);
        await vm.LoadWeekDataCommand.ExecuteAsync();

        var targetDay = vm.WeekDays.First(d => d.TaskCount > 0);
        await vm.SelectDayCommand.ExecuteAsync(targetDay);

        var taskToDelete = vm.DayTasks.First();
        var initialCount = vm.DayTasks.Count;

        // Act: Delete
        await vm.DeleteTaskCommand.ExecuteAsync(taskToDelete);

        // Assert
        vm.DayTasks.Should().HaveCount(initialCount - 1);
        vm.DayTasks.Should().NotContain(taskToDelete);
        vm.IsSnackbarVisible.Should().BeTrue();
        vm.CanUndo.Should().BeTrue();
    }

    [Fact]
    public async Task SaveNewTask_WithDifferentDate_ShouldKeepCurrentDayAndIncrementTargetDayBadge()
    {
        // Arrange
        var service = new MockTaskService();
        var vm = new WeekViewModel(service);
        await vm.LoadWeekDataCommand.ExecuteAsync();

        // Đang xem ngày Thứ Hai 16/11
        var monday = vm.WeekDays.First(d => d.Date.Day == 16);
        await vm.SelectDayCommand.ExecuteAsync(monday);

        var initialMondayCount = vm.DayTasks.Count;
        var tuesday = vm.WeekDays.First(d => d.Date.Day == 17);
        var initialTuesdayBadgeCount = tuesday.TaskCount;

        // Act: Tạo việc cho ngày Thứ Ba 17/11
        vm.OpenCreateModalCommand.Execute(null);
        vm.NewTaskTitle = "Việc mới Thứ Ba";
        vm.NewTaskDueDate = tuesday.Date;
        await vm.SaveNewTaskCommand.ExecuteAsync();

        // Assert: Thứ Hai vẫn giữ nguyên số việc, Thứ Ba badge tăng 1
        vm.SelectedDate.Date.Should().Be(monday.Date.Date);
        vm.DayTasks.Should().HaveCount(initialMondayCount);
        tuesday.TaskCount.Should().Be(initialTuesdayBadgeCount + 1);
    }

    [Fact]
    public async Task OnTasksChanged_ShouldRefreshWeekDaysAndSelectedDayTasks()
    {
        // Arrange
        var service = new MockTaskService();
        var vm = new WeekViewModel(service);
        await vm.LoadWeekDataCommand.ExecuteAsync();

        var selected = vm.SelectedDate;

        // Act: Thêm 1 task trực tiếp từ service (mô phỏng màn khác thay đổi dữ liệu)
        await service.AddTaskAsync(new TaskItem
        {
            Title = "Task từ màn khác",
            DueDate = selected.Date,
            DueTime = new TimeSpan(10, 0, 0)
        });

        // Assert
        vm.DayTasks.Should().Contain(t => t.Title == "Task từ màn khác");
    }
}
