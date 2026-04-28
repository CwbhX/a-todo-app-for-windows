using CommunityToolkit.Mvvm.ComponentModel;

namespace Linework.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    [ObservableProperty]
    private string query = string.Empty;
}
