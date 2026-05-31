using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;
using TaskEntity = TaskManagement.Domain.Entities.Task;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;
using Xunit;

namespace TaskManagement.Tests.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _sut = new TaskService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsResponseWithGeneratedId()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Test Task",
            Status = TaskStatus.Pending
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal(TaskStatus.Pending, result.Status);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_SetsCreatedAtAndUpdatedAt()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Test Task",
            Status = TaskStatus.Pending
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        Assert.True(result.CreatedAt > DateTime.UtcNow.AddSeconds(-5));
        Assert.Equal(result.CreatedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskExists_ReturnsTaskResponse()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var entity = new TaskEntity
        {
            Id = taskId,
            Title = "Existing Task",
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _sut.GetByIdAsync(taskId);

        // Assert
        Assert.Equal("Existing Task", result.Title);
        Assert.Equal(taskId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskDoesNotExist_ThrowsTaskNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity?)null);

        // Act + Assert
        await Assert.ThrowsAsync<TaskNotFoundException>(() => _sut.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAsync_FilteredByStatus_ReturnsOnlyMatchingTasks()
    {
        // Arrange
        var pendingTask = new TaskEntity { Title = "Task 1", Status = TaskStatus.Pending };
        var completedTask = new TaskEntity { Title = "Task 2", Status = TaskStatus.Completed };

        _repositoryMock.Setup(r => r.GetAllAsync(TaskStatus.Pending, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskEntity> { pendingTask });

        // Act
        var result = await _sut.GetAllAsync(TaskStatus.Pending, null);

        // Assert
        Assert.Single(result);
        Assert.All(result, r => Assert.Equal(TaskStatus.Pending, r.Status));
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskExists_UpdatesOnlyProvidedFields()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskEntity
        {
            Id = taskId,
            Title = "Old Title",
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var request = new UpdateTaskRequest { Status = TaskStatus.InProgress };

        _repositoryMock.Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateAsync(taskId, request);

        // Assert
        Assert.Equal(TaskStatus.InProgress, result.Status);
        Assert.Equal("Old Title", result.Title);
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskDoesNotExist_ThrowsTaskNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity?)null);

        // Act + Assert
        await Assert.ThrowsAsync<TaskNotFoundException>(() => _sut.DeleteAsync(Guid.NewGuid()));
    }
}
