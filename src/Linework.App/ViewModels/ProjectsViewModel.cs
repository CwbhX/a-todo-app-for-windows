using CommunityToolkit.Mvvm.ComponentModel;

namespace Linework.ViewModels;

public partial class ProjectsViewModel : ObservableObject
{
    [ObservableProperty]
    private string emptyStateText = "Projects are optional and will be added after core task flow.";
}
