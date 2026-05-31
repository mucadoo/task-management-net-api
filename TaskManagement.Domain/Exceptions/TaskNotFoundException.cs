namespace TaskManagement.Domain.Exceptions;

public class TaskNotFoundException : Exception
{
    public TaskNotFoundException(Guid id) : base($"Task with id '{id}' was not found.")
    {
    }
}
