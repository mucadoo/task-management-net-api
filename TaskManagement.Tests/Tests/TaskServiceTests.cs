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
}
