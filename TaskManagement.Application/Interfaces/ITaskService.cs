using TaskManagement.Application.DTOs;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<PagedResponse<TaskResponse>> GetAllAsync(TaskStatus? status, DateTime? dueDate, PagedRequest paging, CancellationToken cancellationToken = default);
    Task<TaskResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
