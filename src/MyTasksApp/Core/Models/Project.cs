namespace MyTasksApp.Core.Models;

/// <summary>
/// Dự án chứa các công việc
/// </summary>
public class Project
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#4F46E5";
}
