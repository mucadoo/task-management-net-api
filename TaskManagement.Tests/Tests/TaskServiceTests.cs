using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;
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
}
