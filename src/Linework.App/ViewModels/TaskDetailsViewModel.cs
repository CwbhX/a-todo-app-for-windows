using CommunityToolkit.Mvvm.ComponentModel;

namespace Linework.ViewModels;

public partial class TaskDetailsViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string markdownNotes = string.Empty;
}
