using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Linework.Infrastructure;
using Linework.Models;
using Linework.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Linework.ViewModels;

public sealed record NavigationItemViewModel(string Name, string Description);
public sealed record TaskRowViewModel(Guid Id, string Title, TaskItemStatus Status, DateOnly? PlannedForDate, string MetaText)
{
    public bool IsDone => Status == TaskItemStatus.Done;
    public bool CanMarkDone => Status == TaskItemStatus.Active;
    public bool CanPlanToday => Status == TaskItemStatus.Active && PlannedForDate is null;
}

public sealed record TaskDetailsDisplayViewModel(
    Guid Id,
    string Title,
    string StatusText,
    string PlannedDateText,
    string DueDateText,
    string CompletedDateText,
    string ProjectNameText);

public partial class MainViewModel(ITaskService taskService, IClock clock, ILogger<MainViewModel> logger) : ObservableObject
{
    private Guid? taskIdToSelectAfterRefresh;
    private string loadedTaskTitle = string.Empty;
    private string loadedTaskNotes = string.Empty;

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

    [ObservableProperty]
    private TaskRowViewModel? selectedTaskRow;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedTask))]
    private TaskDetailsDisplayViewModel? selectedTaskDetails;

    [ObservableProperty]
    private string taskDetailsMessage = "No task selected";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveTaskDetailsCommand))]
    private string editableTaskTitle = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveTaskDetailsCommand))]
    private string editableTaskNotes = string.Empty;

    [ObservableProperty]
    private string taskDetailsStatusMessage = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveTaskDetailsCommand))]
    private bool isSavingTaskDetails;

    public ObservableCollection<TaskRowViewModel> Tasks { get; } = [];

    public string SelectedViewDescription =>
        NavigationItems.FirstOrDefault(item => item.Name == SelectedViewName)?.Description
        ?? "Linework workspace.";

    public bool ShowsTaskList => SelectedViewName is "Today" or "Active" or "Done";
    public bool ShowsQuickAdd => SelectedViewName is "Today" or "Active";
    public bool HasSelectedTask => SelectedTaskDetails is not null;

    [RelayCommand]
    private async Task LoadAsync()
    {
        logger.LogDebug("Loading main view model.");
        await RefreshTasksAsync();
    }

    [RelayCommand]
    private async Task SelectView(string viewName)
    {
        logger.LogDebug("Selecting view {ViewName}.", viewName);
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
            var task = await taskService.CreateTaskAsync(new CreateTaskRequest(title, PlannedForDate: plannedForDate));
            logger.LogDebug(
                "Added task {TaskId} from {ViewName} with title '{TaskTitle}'.",
                task.Id,
                SelectedViewName,
                task.Title);
            QuickAddTitle = string.Empty;
            taskIdToSelectAfterRefresh = task.Id;
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
            logger.LogDebug("Plan today action completed for task {TaskId}.", task.Id);
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
            logger.LogDebug("Mark done action completed for task {TaskId}.", task.Id);
            await RefreshTasksAsync();
        });
    }

    [RelayCommand(CanExecute = nameof(CanSaveTaskDetails))]
    private async Task SaveTaskDetailsAsync()
    {
        var taskId = SelectedTaskDetails?.Id;
        if (taskId is null)
        {
            return;
        }

        IsSavingTaskDetails = true;

        try
        {
            TaskDetailsStatusMessage = string.Empty;
            await taskService.UpdateTaskDetailsAsync(taskId.Value, new UpdateTaskDetailsRequest(EditableTaskTitle, EditableTaskNotes));
            logger.LogDebug("Saved task details for task {TaskId}.", taskId.Value);
            taskIdToSelectAfterRefresh = taskId.Value;
            await RefreshTasksAsync();

            if (SelectedTaskRow is not null)
            {
                await SelectTaskAsync(SelectedTaskRow);
            }

            TaskDetailsStatusMessage = "Saved.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Saving task details failed for task {TaskId}.", taskId);
            TaskDetailsStatusMessage = ex.Message;
        }
        finally
        {
            IsSavingTaskDetails = false;
        }
    }

    [RelayCommand]
    private async Task SelectTaskAsync(TaskRowViewModel? task)
    {
        if (task is null)
        {
            SelectedTaskDetails = null;
            TaskDetailsMessage = "No task selected";
            TaskDetailsStatusMessage = string.Empty;
            loadedTaskTitle = string.Empty;
            loadedTaskNotes = string.Empty;
            EditableTaskTitle = string.Empty;
            EditableTaskNotes = string.Empty;
            return;
        }

        var selectedTask = await taskService.GetTaskAsync(task.Id);
        if (selectedTask is null)
        {
            logger.LogWarning("Selected task {TaskId} was not found.", task.Id);
            SelectedTaskDetails = null;
            TaskDetailsMessage = "Task was not found.";
            TaskDetailsStatusMessage = string.Empty;
            loadedTaskTitle = string.Empty;
            loadedTaskNotes = string.Empty;
            EditableTaskTitle = string.Empty;
            EditableTaskNotes = string.Empty;
            return;
        }

        logger.LogDebug("Selected task {TaskId}.", selectedTask.Id);
        SelectedTaskDetails = ToDetails(selectedTask);
        loadedTaskTitle = selectedTask.Title;
        loadedTaskNotes = selectedTask.MarkdownNotes ?? string.Empty;
        EditableTaskTitle = loadedTaskTitle;
        EditableTaskNotes = loadedTaskNotes;
        TaskDetailsMessage = string.Empty;
    }

    partial void OnSelectedTaskRowChanged(TaskRowViewModel? value)
    {
        SelectTaskCommand.Execute(value);
    }

    private bool CanAddTask()
    {
        return ShowsQuickAdd && !string.IsNullOrWhiteSpace(QuickAddTitle) && !IsBusy;
    }

    partial void OnEditableTaskTitleChanged(string value)
    {
        SaveTaskDetailsCommand.NotifyCanExecuteChanged();
    }

    partial void OnEditableTaskNotesChanged(string value)
    {
        SaveTaskDetailsCommand.NotifyCanExecuteChanged();
    }

    private bool CanSaveTaskDetails()
    {
        return SelectedTaskDetails is not null
            && !IsSavingTaskDetails
            && !string.IsNullOrWhiteSpace(EditableTaskTitle)
            && (EditableTaskTitle != loadedTaskTitle || EditableTaskNotes != loadedTaskNotes);
    }

    private async Task RefreshTasksAsync()
    {
        IsBusy = true;
        AddTaskCommand.NotifyCanExecuteChanged();

        try
        {
            var selectedTaskId = taskIdToSelectAfterRefresh ?? SelectedTaskRow?.Id;
            taskIdToSelectAfterRefresh = null;
            Tasks.Clear();
            var tasks = await LoadSelectedTasksAsync();

            foreach (var task in tasks)
            {
                Tasks.Add(ToRow(task));
            }

            SelectedTaskRow = selectedTaskId is null
                ? null
                : Tasks.FirstOrDefault(task => task.Id == selectedTaskId.Value);

            TaskListMessage = Tasks.Count == 0 ? EmptyMessageForSelectedView() : string.Empty;
            logger.LogDebug("Refreshed {ViewName} task list with {TaskCount} tasks.", SelectedViewName, Tasks.Count);
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
            logger.LogError(ex, "Task list action failed in {ViewName}.", SelectedViewName);
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

    private static TaskDetailsDisplayViewModel ToDetails(TaskItem task)
    {
        return new TaskDetailsDisplayViewModel(
            task.Id,
            task.Title,
            task.Status.ToString(),
            FormatDate(task.PlannedForDate),
            FormatDateTime(task.DueAt),
            FormatDateTime(task.CompletedAt),
            task.Project?.Name ?? "None");
    }

    private static string FormatDate(DateOnly? date)
    {
        return date?.ToString("MMM d, yyyy", CultureInfo.InvariantCulture) ?? "None";
    }

    private static string FormatDateTime(DateTimeOffset? dateTime)
    {
        return dateTime?.ToLocalTime().ToString("MMM d, yyyy h:mm tt", CultureInfo.InvariantCulture) ?? "None";
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
