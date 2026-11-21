using FluentAssertions;
using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services;
using Xunit;

namespace MyTasksApp.Tests;

public class SnackbarServiceTests
{
    [Fact]
    public void ShowSuccess_ShouldTriggerShowRequested_WithSuccessType()
    {
        // Arrange
        var service = new SnackbarService();
        SnackbarModel? receivedModel = null;
        service.ShowRequested += (s, e) => receivedModel = e;

        // Act
        service.ShowSuccess("Thành công");

        // Assert
        receivedModel.Should().NotBeNull();
        receivedModel!.Message.Should().Be("Thành công");
        receivedModel.Type.Should().Be(SnackbarType.Success);
        receivedModel.OnUndoAction.Should().BeNull();
        receivedModel.DurationMs.Should().Be(5000);
    }

    [Fact]
    public void ShowDanger_ShouldTriggerShowRequested_WithDangerType()
    {
        // Arrange
        var service = new SnackbarService();
        SnackbarModel? receivedModel = null;
        service.ShowRequested += (s, e) => receivedModel = e;

        // Act
        service.ShowDanger("Báo lỗi!");

        // Assert
        receivedModel.Should().NotBeNull();
        receivedModel!.Message.Should().Be("Báo lỗi!");
        receivedModel.Type.Should().Be(SnackbarType.Danger);
        receivedModel.OnUndoAction.Should().BeNull();
    }

    [Fact]
    public void ShowWarning_ShouldTriggerShowRequested_WithWarningType()
    {
        // Arrange
        var service = new SnackbarService();
        SnackbarModel? receivedModel = null;
        service.ShowRequested += (s, e) => receivedModel = e;

        // Act
        service.ShowWarning("Cảnh báo!");

        // Assert
        receivedModel.Should().NotBeNull();
        receivedModel!.Message.Should().Be("Cảnh báo!");
        receivedModel.Type.Should().Be(SnackbarType.Warning);
        receivedModel.OnUndoAction.Should().BeNull();
    }

    [Fact]
    public async Task ShowUndo_ShouldTriggerShowRequested_WithUndoTypeAndCallableAction()
    {
        // Arrange
        var service = new SnackbarService();
        SnackbarModel? receivedModel = null;
        service.ShowRequested += (s, e) => receivedModel = e;
        bool undoExecuted = false;

        // Act
        service.ShowUndo("Đã xóa việc", () =>
        {
            undoExecuted = true;
            return Task.CompletedTask;
        });

        // Assert
        receivedModel.Should().NotBeNull();
        receivedModel!.Message.Should().Be("Đã xóa việc");
        receivedModel.Type.Should().Be(SnackbarType.Undo);
        receivedModel.OnUndoAction.Should().NotBeNull();

        await receivedModel.OnUndoAction!();
        undoExecuted.Should().BeTrue();
    }

    [Fact]
    public void Dismiss_ShouldTriggerDismissRequested()
    {
        // Arrange
        var service = new SnackbarService();
        bool dismissCalled = false;
        service.DismissRequested += (s, e) => dismissCalled = true;

        // Act
        service.Dismiss();

        // Assert
        dismissCalled.Should().BeTrue();
    }
}
