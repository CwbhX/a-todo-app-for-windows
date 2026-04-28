using Linework.Data.Converters;
using Linework.Models;
using Microsoft.EntityFrameworkCore;

namespace Linework.Data;

public sealed class LineworkDbContext(DbContextOptions<LineworkDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<TaskEvent> TaskEvents => Set<TaskEvent>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var dateTimeOffsetConverter = new DateTimeOffsetUnixMillisecondsConverter();
        var nullableDateTimeOffsetConverter = new NullableDateTimeOffsetUnixMillisecondsConverter();
        var nullableDateOnlyConverter = new NullableDateOnlyIsoStringConverter();

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(task => task.Id);
            entity.Property(task => task.Title).HasMaxLength(512).IsRequired();
            entity.Property(task => task.Status).HasConversion<int>();
            entity.Property(task => task.CreatedAt).HasConversion(dateTimeOffsetConverter);
            entity.Property(task => task.UpdatedAt).HasConversion(dateTimeOffsetConverter);
            entity.Property(task => task.CompletedAt).HasConversion(nullableDateTimeOffsetConverter);
            entity.Property(task => task.ArchivedAt).HasConversion(nullableDateTimeOffsetConverter);
            entity.Property(task => task.DueAt).HasConversion(nullableDateTimeOffsetConverter);
            entity.Property(task => task.ReminderAt).HasConversion(nullableDateTimeOffsetConverter);
            entity.Property(task => task.PlannedForDate).HasConversion(nullableDateOnlyConverter);

            entity.HasIndex(task => task.Status);
            entity.HasIndex(task => task.ProjectId);
            entity.HasIndex(task => task.PlannedForDate);
            entity.HasIndex(task => task.DueAt);
            entity.HasIndex(task => task.CompletedAt);
            entity.HasIndex(task => task.ArchivedAt);

            entity.HasOne(task => task.Project)
                .WithMany(project => project.Tasks)
                .HasForeignKey(task => task.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(project => project.Id);
            entity.Property(project => project.Name).HasMaxLength(256).IsRequired();
            entity.Property(project => project.CreatedAt).HasConversion(dateTimeOffsetConverter);
            entity.Property(project => project.ArchivedAt).HasConversion(nullableDateTimeOffsetConverter);
            entity.HasIndex(project => project.Name);
        });

        modelBuilder.Entity<TaskEvent>(entity =>
        {
            entity.HasKey(taskEvent => taskEvent.Id);
            entity.Property(taskEvent => taskEvent.EventType).HasConversion<int>();
            entity.Property(taskEvent => taskEvent.OccurredAt).HasConversion(dateTimeOffsetConverter);
            entity.HasIndex(taskEvent => taskEvent.TaskItemId);
            entity.HasIndex(taskEvent => taskEvent.OccurredAt);
            entity.HasOne(taskEvent => taskEvent.TaskItem)
                .WithMany(task => task.Events)
                .HasForeignKey(taskEvent => taskEvent.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AppSetting>(entity =>
        {
            entity.HasKey(setting => setting.Key);
            entity.Property(setting => setting.Key).HasMaxLength(128);
            entity.Property(setting => setting.ValueJson).IsRequired();
        });
    }
}
