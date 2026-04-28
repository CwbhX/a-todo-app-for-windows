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
