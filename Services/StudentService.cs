using WebApiPractice.DTOs;
using WebApiPractice.Models;
using WebApiPractice.Repositories;

namespace WebApiPractice.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly ILogger<StudentService> _logger;

    public StudentService(
        IStudentRepository repository,
        ILogger<StudentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<StudentDto>> GetAllStudentsAsync()
    {
        var students = await _repository.GetAllAsync();

        return students.Select(s => new StudentDto
        {
            Id = s.Id,
            Name = s.Name,
            Email = s.Email,
            Age = s.Age,
            DepartmentName = s.Department?.DepartmentName
        }).ToList();
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        var student = await _repository.GetByIdAsync(id);

        if (student is null)
            return null;

        return MapToDto(student);
    }

    public async Task<StudentDto> AddStudentAsync(CreateStudentDto studentDto)
    {
        var student = new Student
        {
            Name = studentDto.Name,
            Email = studentDto.Email,
            Age = studentDto.Age
        };

        await _repository.AddAsync(student);

        _logger.LogInformation(
            "Student created successfully with ID {StudentId}",
            student.Id);

        return MapToDto(student);
    }

    public async Task<StudentDto?> UpdateStudentAsync(
        int id,
        UpdateStudentDto studentDto)
    {
        var student = await _repository.GetByIdAsync(id);

        if (student is null)
            return null;

        student.Name = studentDto.Name;
        student.Email = studentDto.Email;
        student.Age = studentDto.Age;

        await _repository.UpdateAsync(student);

        return MapToDto(student);
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await _repository.GetByIdAsync(id);

        if (student is null)
            return false;

        await _repository.DeleteAsync(student);

        return true;
    }

    public Task<bool> EmailExistsAsync(
        string email,
        int? excludeStudentId = null)
    {
        return _repository.EmailExistsAsync(email, excludeStudentId);
    }

    private static StudentDto MapToDto(Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Age = student.Age,
            DepartmentName = student.Department?.DepartmentName
        };
    }

    public async Task<List<StudentDto>> SearchStudentsByNameAsync(string search)
    {
        var students = await _repository.SearchByNameAsync(search);

        return students.Select(s => new StudentDto
        {
            Id = s.Id,
            Name = s.Name,
            Email = s.Email,
            Age = s.Age,
            DepartmentName = s.Department?.DepartmentName
        }).ToList();
    }

    public async Task<List<StudentDto>> GetStudentsByDepartmentIdAsync(int departmentId)
    {
        var students = await _repository.GetByDepartmentIdAsync(departmentId);

        return students.Select(s => new StudentDto
        {
            Id = s.Id,
            Name = s.Name,
            Email = s.Email,
            Age = s.Age,
            DepartmentName = s.Department?.DepartmentName
        }).ToList();
    }

    public async Task<PagedResult<StudentDto>> GetPagedStudentsAsync(
    int pageNumber,
    int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentException("Page number must be at least 1.");

        if (pageSize < 1 || pageSize > 100)
            throw new ArgumentException(
                "Page size must be between 1 and 100.");

        var students = await _repository.GetPagedAsync(
            pageNumber,
            pageSize);

        var totalCount = await _repository.GetTotalCountAsync();

        var studentDtos = students.Select(s => new StudentDto
        {
            Id = s.Id,
            Name = s.Name,
            Email = s.Email,
            Age = s.Age,
            DepartmentName = s.Department?.DepartmentName
        }).ToList();

        return new PagedResult<StudentDto>
        {
            Items = studentDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}