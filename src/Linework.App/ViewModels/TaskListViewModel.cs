using CommunityToolkit.Mvvm.ComponentModel;

namespace Linework.ViewModels;

public partial class TaskListViewModel : ObservableObject
{
    [ObservableProperty]
    private string emptyStateText = "Tasks will appear here once task entry is implemented.";
}
