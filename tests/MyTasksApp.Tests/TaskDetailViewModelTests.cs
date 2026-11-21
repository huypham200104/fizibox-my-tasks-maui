using FluentAssertions;
using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services;
using MyTasksApp.ViewModels;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;
using Xunit;

namespace MyTasksApp.Tests;

public class TaskDetailViewModelTests
{
    [Fact]
    public async Task ApplyQueryAttributes_WithValidId_ShouldLoadTask()
    {
        // Arrange
        var service = new MockTaskService();
        var loc = new LocalizationService();
        var vm = new TaskDetailViewModel(service, loc);

        // Act
        vm.ApplyQueryAttributes(new Dictionary<string, object> { { "id", "1" } });
        await Task.Delay(100); // Wait for async load

        // Assert
        vm.IsNotFound.Should().BeFalse();
        vm.CurrentTask.Should().NotBeNull();
        vm.CurrentTask!.Id.Should().Be(1);
    }

    [Fact]
    public async Task ApplyQueryAttributes_WithNegativeOrInvalidId_ShouldSetIsNotFoundTrue()
    {
        // Arrange
        var service = new MockTaskService();
        var loc = new LocalizationService();
        var vm = new TaskDetailViewModel(service, loc);

        // Act
        vm.ApplyQueryAttributes(new Dictionary<string, object> { { "id", "-1" } });
        await Task.Delay(50);

        // Assert
        vm.IsNotFound.Should().BeTrue();
        vm.CurrentTask.Should().BeNull();
    }

    [Fact]
    public async Task ToggleMainStatus_WhenPending_ShouldChangeToCompleted()
    {
        // Arrange
        var service = new MockTaskService();
        var loc = new LocalizationService();
        var vm = new TaskDetailViewModel(service, loc);
        await vm.LoadTaskByIdAsync(1); // Task 1 is Pending or InProgress in mock

        // Ensure task is pending
        vm.CurrentTask!.Status = TaskStatus.Pending;

        // Act
        await vm.ToggleMainStatusCommand.ExecuteAsync();

        // Assert
        vm.CurrentTask.Status.Should().Be(TaskStatus.Completed);
        vm.CurrentTask.CompletedAt.Should().NotBeNull();
        vm.IsReopenConfirmVisible.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleMainStatus_WhenCompleted_ShouldShowReopenConfirmDialog()
    {
        // Arrange
        var service = new MockTaskService();
        var loc = new LocalizationService();
        var vm = new TaskDetailViewModel(service, loc);
        await vm.LoadTaskByIdAsync(1);
        vm.CurrentTask!.Status = TaskStatus.Completed;

        // Act
        await vm.ToggleMainStatusCommand.ExecuteAsync();

        // Assert: Không được tự tiện đổi trạng thái ngay, phải mở popup xác nhận
        vm.IsReopenConfirmVisible.Should().BeTrue();
        vm.CurrentTask.Status.Should().Be(TaskStatus.Completed);

        // Act 2: Bấm xác nhận mở lại
        await vm.ConfirmReopenCommand.ExecuteAsync();

        // Assert 2
        vm.IsReopenConfirmVisible.Should().BeFalse();
        vm.CurrentTask.Status.Should().Be(TaskStatus.Pending);
        vm.CurrentTask.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task SaveEditCommand_ShouldUpdateTaskPropertiesAndPersist()
    {
        // Arrange
        var service = new MockTaskService();
        var loc = new LocalizationService();
        var vm = new TaskDetailViewModel(service, loc);
        await vm.LoadTaskByIdAsync(1);

        // Act: Open edit modal and update fields
        vm.OpenEditModalCommand.Execute(null);
        vm.IsEditModalVisible.Should().BeTrue();

        vm.EditTitle = "Updated Title Test";
        vm.EditDescription = "Updated Description Test";
        vm.EditNote = "Updated Note Test";
        vm.EditProjectName = "Dự án Mobile App";

        await vm.SaveEditCommand.ExecuteAsync();

        // Assert
        vm.IsEditModalVisible.Should().BeFalse();
        vm.CurrentTask!.Title.Should().Be("Updated Title Test");
        vm.CurrentTask.Description.Should().Be("Updated Description Test");
        vm.CurrentTask.Note.Should().Be("Updated Note Test");
        vm.CurrentTask.ProjectName.Should().Be("Dự án Mobile App");

        // Verify in service
        var persisted = await service.GetTaskByIdAsync(1);
        persisted!.Title.Should().Be("Updated Title Test");
    }

    [Fact]
    public async Task ChangeStatusSheet_ShouldUpdateTaskStatus()
    {
        // Arrange
        var service = new MockTaskService();
        var loc = new LocalizationService();
        var vm = new TaskDetailViewModel(service, loc);
        await vm.LoadTaskByIdAsync(1);

        // Act
        await vm.SelectStatusOptionCommand.ExecuteAsync(TaskStatus.InProgress);

        // Assert
        vm.CurrentTask!.Status.Should().Be(TaskStatus.InProgress);
        vm.IsStatusSheetVisible.Should().BeFalse();
    }

    [Fact]
    public async Task ApplyDatePicker_ShouldUpdateDueDate()
    {
        // Arrange
        var service = new MockTaskService();
        var loc = new LocalizationService();
        var vm = new TaskDetailViewModel(service, loc);
        await vm.LoadTaskByIdAsync(1);

        var newDate = new DateTime(2026, 11, 25);
        vm.OpenDatePickerSheetCommand.Execute(null);
        vm.PickerSelectedDate = newDate;

        // Act
        await vm.ApplyDatePickerCommand.ExecuteAsync();

        // Assert
        vm.IsDatePickerSheetVisible.Should().BeFalse();
        vm.CurrentTask!.DueDate.Date.Should().Be(newDate.Date);
    }

    [Fact]
    public async Task ApplyTimePicker_ShouldUpdateDueTime()
    {
        // Arrange
        var service = new MockTaskService();
        var loc = new LocalizationService();
        var vm = new TaskDetailViewModel(service, loc);
        await vm.LoadTaskByIdAsync(1);

        vm.OpenTimePickerSheetCommand.Execute(null);
        vm.SelectQuickTimePresetCommand.Execute("17:00");

        // Act
        await vm.ApplyTimePickerCommand.ExecuteAsync();

        // Assert
        vm.IsTimePickerSheetVisible.Should().BeFalse();
        vm.CurrentTask!.DueTime.Should().Be(new TimeSpan(17, 0, 0));
    }
}
