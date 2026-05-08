using Linework.Models;
using Linework.Services;
using Linework.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Linework.Tests;

public sealed class MainViewModelTests
{
    [Fact]
    public async Task Selecting_task_row_loads_read_only_task_details()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock, new SettingsService(dbContext));
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Client Work",
            CreatedAt = clock.Now
        };
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(ct);
        var task = await service.CreateTaskAsync(
            new CreateTaskRequest(
                "Prepare weekly review",
                MarkdownNotes: "## Notes\n\nSummarize finished work.",
                ProjectId: project.Id,
                PlannedForDate: clock.Today,
                DueAt: clock.Now.AddHours(2)),
            ct);
        var viewModel = new MainViewModel(service, clock);

        await viewModel.LoadCommand.ExecuteAsync(null);
        var row = Assert.Single(viewModel.Tasks);
        await viewModel.SelectTaskCommand.ExecuteAsync(row);

        Assert.Equal(task.Id, viewModel.SelectedTaskDetails?.Id);
        Assert.Equal("Prepare weekly review", viewModel.SelectedTaskDetails?.Title);
        Assert.Equal("Active", viewModel.SelectedTaskDetails?.StatusText);
        Assert.Equal("Apr 27, 2026", viewModel.SelectedTaskDetails?.PlannedDateText);
        Assert.NotEqual("None", viewModel.SelectedTaskDetails?.DueDateText);
        Assert.Equal("None", viewModel.SelectedTaskDetails?.CompletedDateText);
        Assert.Equal("Client Work", viewModel.SelectedTaskDetails?.ProjectNameText);
        Assert.Equal("Prepare weekly review", viewModel.EditableTaskTitle);
        Assert.Equal("## Notes\n\nSummarize finished work.", viewModel.EditableTaskNotes);
        Assert.True(viewModel.HasSelectedTask);
    }

    [Fact]
    public async Task Saving_task_details_persists_changes_and_refreshes_selected_row()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock, new SettingsService(dbContext));
        var task = await service.CreateTaskAsync(
            new CreateTaskRequest("Original title", MarkdownNotes: "Original notes", PlannedForDate: clock.Today),
            ct);
        var viewModel = new MainViewModel(service, clock);

        await viewModel.LoadCommand.ExecuteAsync(null);
        var row = Assert.Single(viewModel.Tasks);
        await viewModel.SelectTaskCommand.ExecuteAsync(row);

        viewModel.EditableTaskTitle = "Updated title";
        viewModel.EditableTaskNotes = "Updated notes";
        await viewModel.SaveTaskDetailsCommand.ExecuteAsync(null);

        Assert.Equal("Updated title", viewModel.SelectedTaskDetails?.Title);
        Assert.Equal("Updated title", Assert.Single(viewModel.Tasks).Title);
        Assert.Equal("Updated title", viewModel.EditableTaskTitle);
        Assert.Equal("Updated notes", viewModel.EditableTaskNotes);
        Assert.Equal("Saved.", viewModel.TaskDetailsStatusMessage);

        var saved = await dbContext.TaskItems.SingleAsync(item => item.Id == task.Id, ct);
        Assert.Equal("Updated title", saved.Title);
        Assert.Equal("Updated notes", saved.MarkdownNotes);
        Assert.Contains(await dbContext.TaskEvents.Where(item => item.TaskItemId == task.Id).ToListAsync(ct), item => item.EventType == TaskEventType.Edited);
    }
}
