using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Linework.Infrastructure;
using Linework.Models;
using Linework.Services;
using System.Collections.ObjectModel;

namespace Linework.ViewModels;

public sealed record NavigationItemViewModel(string Name, string Description);
public sealed record TaskRowViewModel(Guid Id, string Title, TaskItemStatus Status, DateOnly? PlannedForDate, string MetaText)
{
    public bool IsDone => Status == TaskItemStatus.Done;
    public bool CanMarkDone => Status == TaskItemStatus.Active;
    public bool CanPlanToday => Status == TaskItemStatus.Active && PlannedForDate is null;
}

public partial class MainViewModel(ITaskService taskService, IClock clock) : ObservableObject
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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTaskCommand))]
    private string quickAddTitle = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string taskListMessage = "Loading tasks...";

    public ObservableCollection<TaskRowViewModel> Tasks { get; } = [];

    public string SelectedViewDescription =>
        NavigationItems.FirstOrDefault(item => item.Name == SelectedViewName)?.Description
        ?? "Linework workspace.";

    public bool ShowsTaskList => SelectedViewName is "Today" or "Active" or "Done";
    public bool ShowsQuickAdd => SelectedViewName is "Today" or "Active";

    [RelayCommand]
    private async Task LoadAsync()
    {
        await RefreshTasksAsync();
    }

    [RelayCommand]
    private async Task SelectView(string viewName)
    {
        SelectedViewName = viewName;
        OnPropertyChanged(nameof(SelectedViewDescription));
        OnPropertyChanged(nameof(ShowsTaskList));
        OnPropertyChanged(nameof(ShowsQuickAdd));
        await RefreshTasksAsync();
    }

    [RelayCommand(CanExecute = nameof(CanAddTask))]
    private async Task AddTaskAsync()
    {
        var title = QuickAddTitle.Trim();
        if (title.Length == 0)
        {
            return;
        }

        await RunTaskListActionAsync(async () =>
        {
            var plannedForDate = SelectedViewName == "Today" ? clock.Today : (DateOnly?)null;
            await taskService.CreateTaskAsync(new CreateTaskRequest(title, PlannedForDate: plannedForDate));
            QuickAddTitle = string.Empty;
            await RefreshTasksAsync();
        });
    }

    [RelayCommand]
    private async Task PlanTodayAsync(TaskRowViewModel task)
    {
        if (!task.CanPlanToday)
        {
            return;
        }

        await RunTaskListActionAsync(async () =>
        {
            await taskService.PlanForTodayAsync(task.Id);
            await RefreshTasksAsync();
        });
    }

    [RelayCommand]
    private async Task MarkDoneAsync(TaskRowViewModel task)
    {
        if (!task.CanMarkDone)
        {
            return;
        }

        await RunTaskListActionAsync(async () =>
        {
            await taskService.MarkDoneAsync(task.Id);
            await RefreshTasksAsync();
        });
    }

    private bool CanAddTask()
    {
        return ShowsQuickAdd && !string.IsNullOrWhiteSpace(QuickAddTitle) && !IsBusy;
    }

    private async Task RefreshTasksAsync()
    {
        IsBusy = true;
        AddTaskCommand.NotifyCanExecuteChanged();

        try
        {
            Tasks.Clear();
            var tasks = await LoadSelectedTasksAsync();

            foreach (var task in tasks)
            {
                Tasks.Add(ToRow(task));
            }

            TaskListMessage = Tasks.Count == 0 ? EmptyMessageForSelectedView() : string.Empty;
        }
        finally
        {
            IsBusy = false;
            AddTaskCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task RunTaskListActionAsync(Func<Task> action)
    {
        try
        {
            TaskListMessage = string.Empty;
            await action();
        }
        catch (Exception ex)
        {
            TaskListMessage = ex.Message;
        }
    }

    private Task<IReadOnlyList<TaskItem>> LoadSelectedTasksAsync()
    {
        return SelectedViewName switch
        {
            "Today" => taskService.GetTodayTasksAsync(),
            "Active" => taskService.GetActiveTasksAsync(),
            "Done" => taskService.GetDoneTasksAsync(new DoneQuery()),
            _ => Task.FromResult<IReadOnlyList<TaskItem>>([])
        };
    }

    private TaskRowViewModel ToRow(TaskItem task)
    {
        var metaText = task.Status switch
        {
            TaskItemStatus.Done when task.CompletedAt is not null => $"Done {task.CompletedAt.Value.LocalDateTime:g}",
            TaskItemStatus.Done => "Done",
            _ when task.PlannedForDate == clock.Today => "Planned today",
            _ when task.PlannedForDate is not null => $"Planned {task.PlannedForDate:MMM d}",
            _ => "Unplanned"
        };

        return new TaskRowViewModel(task.Id, task.Title, task.Status, task.PlannedForDate, metaText);
    }

    private string EmptyMessageForSelectedView()
    {
        return SelectedViewName switch
        {
            "Today" => "No tasks for today yet.",
            "Active" => "No active tasks yet.",
            "Done" => "No completed tasks yet.",
            _ => "This view is still a placeholder."
        };
    }
}
