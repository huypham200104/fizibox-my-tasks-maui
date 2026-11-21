using FluentAssertions;
using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services;
using MyTasksApp.ViewModels;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;
using Xunit;

namespace MyTasksApp.Tests;

public class TodayViewModelTests
{
    [Fact]
    public async Task LoadTasksCommand_ShouldPopulateGroupedCollectionsAndMetrics()
    {
        // Arrange
        var service = new MockTaskService();
        var viewModel = new TodayViewModel(service);

        // Act
        await viewModel.LoadTasksCommand.ExecuteAsync();

        // Assert
        viewModel.IsBusy.Should().BeFalse();
        viewModel.TotalTasksCount.Should().BeGreaterThan(0);
        viewModel.ProgressRatio.Should().BeInRange(0.0, 1.0);
        viewModel.ProgressPercentageText.Should().EndWith("%");
        viewModel.ProgressRatioText.Should().Contain("việc hoàn thành");

        // Các task quá hạn phải đúng điều kiện DueDate < Today
        foreach (var task in viewModel.OverdueTasks)
        {
            task.DueDate.Date.Should().BeBefore(DateTime.Today);
            task.Status.Should().NotBe(TaskStatus.Completed);
        }

        // Các task hôm nay phải đúng điều kiện DueDate == Today
        foreach (var task in viewModel.TodayTasks)
        {
            task.DueDate.Date.Should().Be(DateTime.Today);
            task.Status.Should().NotBe(TaskStatus.Completed);
        }

        // Các task xong phải có status Completed
        foreach (var task in viewModel.CompletedTasks)
        {
            task.Status.Should().Be(TaskStatus.Completed);
        }
    }

    [Fact]
    public async Task ToggleTaskStatusCommand_ShouldOptimisticallyUpdateTaskAndCollections()
    {
        // Arrange
        var service = new MockTaskService();
        var viewModel = new TodayViewModel(service);
        await viewModel.LoadTasksCommand.ExecuteAsync();

        var initialCompletedCount = viewModel.CompletedTasks.Count;
        var taskToComplete = viewModel.TodayTasks.FirstOrDefault();

        if (taskToComplete != null)
        {
            // Act: Hoàn thành task
            await viewModel.ToggleTaskStatusCommand.ExecuteAsync(taskToComplete);

            // Assert
            taskToComplete.Status.Should().Be(TaskStatus.Completed);
            viewModel.CompletedTasks.Should().Contain(taskToComplete);
            viewModel.TodayTasks.Should().NotContain(taskToComplete);
            viewModel.CompletedTasks.Count.Should().Be(initialCompletedCount + 1);
            viewModel.SnackbarMessage.Should().Be("Đã hoàn thành công việc");

            // Act: Mở lại task
            await viewModel.ToggleTaskStatusCommand.ExecuteAsync(taskToComplete);

            // Assert
            taskToComplete.Status.Should().Be(TaskStatus.Pending);
            viewModel.CompletedTasks.Should().NotContain(taskToComplete);
            viewModel.TodayTasks.Should().Contain(taskToComplete);
            viewModel.CompletedTasks.Count.Should().Be(initialCompletedCount);
            viewModel.SnackbarMessage.Should().Be("Đã mở lại công việc");
        }
    }

    [Fact]
    public async Task DeleteTaskCommand_ShouldRemoveTaskAndEnableUndo()
    {
        // Arrange
        var service = new MockTaskService();
        var viewModel = new TodayViewModel(service);
        await viewModel.LoadTasksCommand.ExecuteAsync();

        var taskToDelete = viewModel.TodayTasks.FirstOrDefault() ?? viewModel.OverdueTasks.First();
        var initialTotal = viewModel.TotalTasksCount;

        // Act
        await viewModel.DeleteTaskCommand.ExecuteAsync(taskToDelete);

        // Assert
        viewModel.TodayTasks.Should().NotContain(taskToDelete);
        viewModel.OverdueTasks.Should().NotContain(taskToDelete);
        viewModel.TotalTasksCount.Should().Be(initialTotal - 1);
        viewModel.IsSnackbarVisible.Should().BeTrue();
        viewModel.CanUndo.Should().BeTrue();
        viewModel.SnackbarMessage.Should().Be("Đã xóa công việc");
    }

    [Fact]
    public async Task SaveNewTaskCommand_WithEmptyTitle_ShouldShowWarningSnackbar()
    {
        // Arrange
        var service = new MockTaskService();
        var viewModel = new TodayViewModel(service);
        viewModel.NewTaskTitle = "   "; // Khoảng trắng

        // Act
        await viewModel.SaveNewTaskCommand.ExecuteAsync();

        // Assert
        viewModel.SnackbarMessage.Should().Be("Vui lòng nhập tên công việc");
    }

    [Fact]
    public async Task ReopenCompletedTask_ShouldMoveTaskBackToProperGroup_AndMaintainVisualProgress()
    {
        // Arrange
        var service = new MockTaskService();
        var viewModel = new TodayViewModel(service);
        await viewModel.LoadTasksCommand.ExecuteAsync();

        var completedTask = viewModel.CompletedTasks.FirstOrDefault();
        completedTask.Should().NotBeNull();
        int initialCompletedCount = viewModel.CompletedTasks.Count;
        int initialTotalCount = viewModel.TotalTasksCount;

        // Act: Người dùng mở lại công việc đã xong
        await viewModel.ToggleTaskStatusCommand.ExecuteAsync(completedTask);

        // Assert: Trạng thái và vị trí danh sách
        completedTask!.Status.Should().Be(TaskStatus.Pending);
        completedTask.CompletedAt.Should().BeNull();
        viewModel.CompletedTasks.Should().NotContain(completedTask);
        viewModel.TodayTasks.Should().Contain(completedTask);
        viewModel.CompletedTasks.Count.Should().Be(initialCompletedCount - 1);
        viewModel.TotalTasksCount.Should().Be(initialTotalCount);

        // Assert: Tiến độ tính toán chính xác
        viewModel.ProgressRatio.Should().Be((double)(initialCompletedCount - 1) / initialTotalCount);
        viewModel.SnackbarMessage.Should().Be("Đã mở lại công việc");
    }
}
