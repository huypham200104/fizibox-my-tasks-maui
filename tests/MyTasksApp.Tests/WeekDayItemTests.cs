using FluentAssertions;
using MyTasksApp.Core.Models;
using Xunit;

namespace MyTasksApp.Tests;

public class WeekDayItemTests
{
    [Fact]
    public void WeekDayItem_PropertiesAndNotifications_ShouldWorkCorrectly()
    {
        // Arrange
        var testDate = new DateTime(2026, 11, 20);
        var item = new WeekDayItem
        {
            Date = testDate,
            DayOfWeekShort = "T6",
            DayNumber = "20",
            TaskCount = 4,
            IsToday = true,
            IsSelected = false
        };

        // Assert initial
        item.Date.Should().Be(testDate);
        item.DayOfWeekShort.Should().Be("T6");
        item.DayNumber.Should().Be("20");
        item.TaskCount.Should().Be(4);
        item.TaskCountText.Should().Be("• 4");
        item.IsToday.Should().BeTrue();
        item.IsSelected.Should().BeFalse();

        // PropertyChanged tracking
        var changedProps = new List<string>();
        item.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != null)
                changedProps.Add(args.PropertyName);
        };

        // Act & Assert changes
        item.TaskCount = 0;
        item.TaskCountText.Should().Be("• 0");
        changedProps.Should().Contain(nameof(WeekDayItem.TaskCount));
        changedProps.Should().Contain(nameof(WeekDayItem.TaskCountText));

        item.IsSelected = true;
        changedProps.Should().Contain(nameof(WeekDayItem.IsSelected));

        item.IsToday = false;
        changedProps.Should().Contain(nameof(WeekDayItem.IsToday));
    }
}
