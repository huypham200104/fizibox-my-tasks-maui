using FluentAssertions;
using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;
using Xunit;

namespace MyTasksApp.Tests;

public class MockTaskServiceTests
{
    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnDefaultMockData()
    {
        // Arrange
        var service = new MockTaskService();

        // Act
        var tasks = await service.GetAllTasksAsync();

        // Assert
        tasks.Should().NotBeNull();
        tasks.Should().NotBeEmpty();
        tasks.Count.Should().BeGreaterThan(5);
    }

    [Fact]
    public async Task GetTodayTasksAsync_ShouldReturnRelevantTasksForToday()
    {
        // Arrange
        var service = new MockTaskService();

        // Act
        var todayTasks = await service.GetTodayTasksAsync();

        // Assert
        todayTasks.Should().NotBeNull();
        todayTasks.Should().NotBeEmpty();
        
        // Tất cả các task thuộc Today phải là: quá hạn, hạn hôm nay, hoặc đã xong hôm nay
        DateTime today = DateTime.Today;
        todayTasks.Should().OnlyContain(t =>
            (t.DueDate.Date < today && t.Status != TaskStatus.Completed) ||
            (t.DueDate.Date == today && t.Status != TaskStatus.Completed) ||
            (t.Status == TaskStatus.Completed && (t.DueDate.Date == today || (t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today)))
        );
    }

    [Fact]
    public async Task ToggleTaskStatusAsync_ShouldToggleStatusAndFireTasksChanged()
    {
        // Arrange
        var service = new MockTaskService();
        var allTasks = await service.GetAllTasksAsync();
        var taskToToggle = allTasks.First(t => t.Status == TaskStatus.Pending);
        bool eventFired = false;
        service.TasksChanged += (s, e) => eventFired = true;

        // Act
        bool result = await service.ToggleTaskStatusAsync(taskToToggle.Id);
        var updated = await service.GetTaskByIdAsync(taskToToggle.Id);

        // Assert
        result.Should().BeTrue();
        eventFired.Should().BeTrue();
        updated.Should().NotBeNull();
        updated!.Status.Should().Be(TaskStatus.Completed);
        updated.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteTaskAsync_And_UndoDeleteAsync_ShouldWorkCorrectly()
    {
        // Arrange
        var service = new MockTaskService();
        var allTasks = await service.GetAllTasksAsync();
        var taskToDelete = allTasks.First();
        int initialCount = allTasks.Count;

        // Act 1: Xóa
        bool deleteSuccess = await service.DeleteTaskAsync(taskToDelete.Id);
        var afterDelete = await service.GetAllTasksAsync();

        // Assert 1
        deleteSuccess.Should().BeTrue();
        afterDelete.Count.Should().Be(initialCount - 1);
        afterDelete.Should().NotContain(t => t.Id == taskToDelete.Id);

        // Act 2: Hoàn tác
        var restored = await service.UndoDeleteAsync();
        var afterUndo = await service.GetAllTasksAsync();

        // Assert 2
        restored.Should().NotBeNull();
        restored!.Id.Should().Be(taskToDelete.Id);
        afterUndo.Count.Should().Be(initialCount);
        afterUndo.Should().Contain(t => t.Id == taskToDelete.Id);
    }

    [Fact]
    public async Task AddTaskAsync_ShouldAddNewTaskWithGeneratedId()
    {
        // Arrange
        var service = new MockTaskService();
        var newTask = new TaskItem
        {
            Title = "Unit Test Task",
            Description = "Description for test",
            ProjectName = "Testing",
            DueDate = DateTime.Today,
            DueTime = new TimeSpan(10, 0, 0),
            Status = TaskStatus.Pending
        };

        // Act
        bool success = await service.AddTaskAsync(newTask);
        var allTasks = await service.GetAllTasksAsync();
        var addedTask = allTasks.FirstOrDefault(t => t.Title == "Unit Test Task");

        // Assert
        success.Should().BeTrue();
        addedTask.Should().NotBeNull();
        addedTask!.Id.Should().BeGreaterThan(0);
        addedTask.ProjectName.Should().Be("Testing");
    }
}
