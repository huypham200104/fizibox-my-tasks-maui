using Microsoft.Maui.Controls;
using MyTasksApp.ViewModels;

namespace MyTasksApp.Views;

public partial class TaskDetailPage : ContentPage, IQueryAttributable
{
    public TaskDetailPage(TaskDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is IQueryAttributable vm)
        {
            vm.ApplyQueryAttributes(query);
        }
    }
}
