using TaskManagement.Application.DTOs;
using TaskManagement.Application.Validators;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;
using Xunit;

namespace TaskManagement.Tests.Tests;

public class CreateTaskRequestValidatorTests
{
    private readonly CreateTaskRequestValidator _validator;

    public CreateTaskRequestValidatorTests()
    {
        _validator = new CreateTaskRequestValidator();
    }

    [Fact]
    public async Task Validate_WithEmptyTitle_ReturnsValidationError()
    {
        var request = new CreateTaskRequest { Title = "", Status = DomainTaskStatus.Pending };
        var result = await _validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Title is required.", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task Validate_WithWhiteSpaceTitle_ReturnsValidationError()
    {
        var request = new CreateTaskRequest { Title = "   ", Status = DomainTaskStatus.Pending };
        var result = await _validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Title is required.", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task Validate_WithTitleExceeding200Chars_ReturnsValidationError()
    {
        var request = new CreateTaskRequest { Title = new string('a', 201), Status = DomainTaskStatus.Pending };
        var result = await _validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Title must not exceed 200 characters.", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task Validate_WithDueDateInThePast_ReturnsValidationError()
    {
        var request = new CreateTaskRequest 
        { 
            Title = "Valid Title", 
            DueDate = DateTime.UtcNow.AddDays(-1), 
            Status = DomainTaskStatus.Pending 
        };
        var result = await _validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Due date cannot be in the past.", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task Validate_WithValidRequest_PassesValidation()
    {
        var request = new CreateTaskRequest 
        { 
            Title = "Valid Title", 
            DueDate = DateTime.UtcNow.AddDays(1), 
            Status = DomainTaskStatus.Pending 
        };
        var result = await _validator.ValidateAsync(request);
        
        Assert.True(result.IsValid);
    }
}
