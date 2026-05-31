using FluentValidation;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;
using TaskEntity = TaskManagement.Domain.Entities.Task;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IValidator<CreateTaskRequest> _createValidator;
    private readonly IValidator<UpdateTaskRequest> _updateValidator;

    public TaskService(
        ITaskRepository taskRepository,
        IValidator<CreateTaskRequest> createValidator,
        IValidator<UpdateTaskRequest> updateValidator)
    {
        _taskRepository = taskRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResponse<TaskResponse>> GetAllAsync(TaskStatus? status, DateTime? dueDate, PagedRequest paging, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _taskRepository.GetAllAsync(status, dueDate, paging.Skip(), paging.PageSize, cancellationToken);
        
        return new PagedResponse<TaskResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = totalCount,
            PageNumber = paging.PageNumber,
            PageSize = paging.PageSize
        };
    }

    public async Task<TaskResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            throw new TaskNotFoundException(id);
        }
        return MapToResponse(task);
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new TaskValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var now = DateTime.UtcNow;
        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            DueDate = request.DueDate,
            Status = request.Status,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _taskRepository.AddAsync(task, cancellationToken);
        return MapToResponse(task);
    }

    public async Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new TaskValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            throw new TaskNotFoundException(id);
        }

        if (request.Title != null) task.Title = request.Title.Trim();
        if (request.Description != null) task.Description = request.Description.Trim();
        if (request.DueDate != null) task.DueDate = request.DueDate;
        if (request.Status != null) task.Status = request.Status.Value;

        task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task, cancellationToken);
        return MapToResponse(task);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            throw new TaskNotFoundException(id);
        }

        await _taskRepository.DeleteAsync(id, cancellationToken);
    }

    private static TaskResponse MapToResponse(TaskEntity entity)
    {
        return new TaskResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            DueDate = entity.DueDate,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
