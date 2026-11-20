using MyTasksApp.Views;

namespace MyTasksApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Đăng ký route điều hướng trang Chi tiết công việc
		Routing.RegisterRoute(nameof(TaskDetailPage), typeof(TaskDetailPage));
	}
}
