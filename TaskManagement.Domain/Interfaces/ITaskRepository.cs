using TaskEntity = TaskManagement.Domain.Entities.Task;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Interfaces;

public interface ITaskRepository
{
    Task<(IEnumerable<TaskEntity> Items, int TotalCount)> GetAllAsync(TaskStatus? status, DateTime? dueDate, int skip, int take, CancellationToken cancellationToken = default);
    Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
