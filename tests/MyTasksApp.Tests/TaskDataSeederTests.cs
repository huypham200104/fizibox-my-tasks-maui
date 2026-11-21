using FluentAssertions;
using MyTasksApp.Core.Enums;
using MyTasksApp.Services;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;
using Xunit;

namespace MyTasksApp.Tests;

public class TaskDataSeederTests
{
    [Fact]
    public void GenerateDefaultTasks_ShouldCoverAllDaysFrom19To28NovemberWith4To12TasksPerDay()
    {
        // Act
        var tasks = TaskDataSeeder.GenerateDefaultTasks();

        // Assert
        tasks.Should().NotBeNull();
        tasks.Should().HaveCount(74);

        // Đảm bảo không trùng Id và tất cả Id đều dương
        tasks.Select(t => t.Id).Should().OnlyHaveUniqueItems();
        tasks.All(t => t.Id > 0).Should().BeTrue();

        // Đảm bảo dữ liệu cơ bản hợp lệ
        foreach (var task in tasks)
        {
            task.Title.Should().NotBeNullOrWhiteSpace();
            task.Description.Should().NotBeNullOrWhiteSpace();
            task.ProjectName.Should().NotBeNullOrWhiteSpace();
            task.DueDate.Year.Should().Be(2026);
            task.DueDate.Month.Should().Be(11);
            task.DueDate.Day.Should().BeInRange(19, 28);

            if (task.Status == TaskStatus.Completed)
            {
                task.CompletedAt.Should().NotBeNull();
            }
        }

        // Kiểm tra phân bổ từng ngày từ 19/11/2026 đến 28/11/2026
        for (int day = 19; day <= 28; day++)
        {
            DateTime targetDate = new(2026, 11, day);
            var dayTasks = tasks.Where(t => t.DueDate.Date == targetDate.Date).ToList();

            // Mỗi ngày đều khoảng từ 4 - 12 task
            dayTasks.Count.Should().BeInRange(4, 12, $"Ngày {targetDate:dd/MM/yyyy} phải có từ 4 đến 12 tasks");
        }
    }

    [Fact]
    public void GenerateDefaultTasks_ShouldContainRealisticProjectNames()
    {
        // Act
        var tasks = TaskDataSeeder.GenerateDefaultTasks();

        // Assert
        var projects = tasks.Select(t => t.ProjectName).Distinct().ToList();
        projects.Should().Contain(new[]
        {
            "MyTasks App",
            "Dự án Mobile App",
            "Dự án Backend",
            "UI/UX Design",
            "DevOps",
            "Core Platform",
            "Công việc cá nhân"
        });
    }
}
