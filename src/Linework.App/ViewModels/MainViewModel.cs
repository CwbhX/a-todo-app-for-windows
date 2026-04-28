using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Linework.ViewModels;

public sealed record NavigationItemViewModel(string Name, string Description);

public partial class MainViewModel : ObservableObject
{
    public IReadOnlyList<NavigationItemViewModel> NavigationItems { get; } =
    [
        new("Today", "Planned work and today's completions."),
        new("Active", "Open work plus recently completed tasks."),
        new("Done", "Completed work history."),
        new("Projects", "Optional task grouping."),
        new("Search", "Find current and completed work."),
        new("Settings", "Local preferences.")
    ];

    [ObservableProperty]
    private string selectedViewName = "Today";

    public string SelectedViewDescription =>
        NavigationItems.FirstOrDefault(item => item.Name == SelectedViewName)?.Description
        ?? "Linework workspace.";

    [RelayCommand]
    private void SelectView(string viewName)
    {
        SelectedViewName = viewName;
        OnPropertyChanged(nameof(SelectedViewDescription));
    }
}
