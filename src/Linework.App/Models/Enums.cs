namespace Linework.Models;

public enum TaskItemStatus
{
    Active = 0,
    Done = 1,
    Archived = 2
}

public enum TaskEventType
{
    Created = 0,
    Edited = 1,
    MarkedDone = 2,
    Reopened = 3,
    Archived = 4,
    PlannedForDateChanged = 5,
    DueDateChanged = 6,
    ReminderChanged = 7,
    ProjectChanged = 8,
    ImportantChanged = 9
}

public enum AppThemeMode
{
    System = 0,
    Light = 1,
    Dark = 2
}

public enum AppDefaultView
{
    Today = 0,
    Active = 1,
    Done = 2,
    Projects = 3,
    Search = 4
}
