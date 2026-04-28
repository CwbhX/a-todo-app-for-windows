using CommunityToolkit.Mvvm.ComponentModel;

namespace Linework.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string themeMode = "System";
}
