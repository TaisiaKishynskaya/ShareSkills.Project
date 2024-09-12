using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.RepositoriesTests;

public class StudentRepositoryTests
{
    private readonly StudentRepository _repository;
    private readonly AppDbContext _context;
    private readonly DbContextOptions<AppDbContext> _options;
    
    public StudentRepositoryTests()
    { 
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    
        _context = new AppDbContext(_options);
        _repository = new StudentRepository(_context);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllStudents()
    {
        // Arrange
        var student1 = new StudentEntity { Id = Guid.NewGuid(), Purpose = "Purpose1", UserId = Guid.NewGuid() };
        var student2 = new StudentEntity { Id = Guid.NewGuid(), Purpose = "Purpose2", UserId = Guid.NewGuid() };
        _context.Students.AddRange(student1, student2);
        await _context.SaveChangesAsync();

        // Act
        var students = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, students.Count());
        Assert.Contains(students, s => s.Id == student1.Id);
        Assert.Contains(students, s => s.Id == student2.Id);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllStudentsWithSamePurpose()
    {
        // Arrange
        var purpose = "Common Purpose";
        var student1 = new StudentEntity { Id = Guid.NewGuid(), Purpose = purpose, UserId = Guid.NewGuid() };
        var student2 = new StudentEntity { Id = Guid.NewGuid(), Purpose = purpose, UserId = Guid.NewGuid() };
        _context.Students.AddRange(student1, student2);
        await _context.SaveChangesAsync();

        // Act
        var students = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, students.Count());
        Assert.Contains(students, s => s.Purpose == purpose);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyListWhenDatabaseIsEmpty()
    {
        // Act
        var students = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(students);
    }
    
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectStudent()
    {
        // Arrange
        var student = new StudentEntity { Id = Guid.NewGuid(), Purpose = "Test Purpose", UserId = Guid.NewGuid() };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(student.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(student.Id, result?.Id);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullIfStudentIdDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }


    
    [Fact]
    public async Task Insert_ShouldAddStudentToDatabase()
    {
        // Arrange
        var student = new StudentEntity { Id = Guid.NewGuid(), Purpose = "New Purpose", UserId = Guid.NewGuid() };

        // Act
        _repository.Insert(student);
        await _context.SaveChangesAsync();

        // Assert
        var insertedStudent = await _context.Students.FindAsync(student.Id);
        Assert.NotNull(insertedStudent);
        Assert.Equal(student.Id, insertedStudent.Id);
    }
    
    [Fact]
    public async Task Insert_ShouldNotAddDuplicateStudents()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student1 = new StudentEntity { Id = studentId, Purpose = "Unique Purpose", UserId = Guid.NewGuid() };

        // Insert the first student
        using (var context = new AppDbContext(_options))
        {
            var repository = new StudentRepository(context);
            repository.Insert(student1);
            await context.SaveChangesAsync();
        }

        // Act
        // Attempt to insert a new entity with the same Id
        var student2 = new StudentEntity { Id = studentId, Purpose = "Different Purpose", UserId = Guid.NewGuid() };

        // Use a fresh DbContext instance to insert the duplicate student
        using (var context = new AppDbContext(_options))
        {
            var repository = new StudentRepository(context);
            
            // Insert the duplicate student
            repository.Insert(student2);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => context.SaveChangesAsync());
            Assert.Contains("An item with the same key has already been added.", exception.Message);
        }

        // Assert
        // Verify that only the first student was inserted
        using (var context = new AppDbContext(_options))
        {
            var student = await context.Students.FindAsync(studentId);
            Assert.NotNull(student);
            Assert.Equal(studentId, student.Id);
            Assert.Equal("Unique Purpose", student.Purpose); // Ensure the first inserted purpose is retained
        }
    }
    
    
    [Fact]
    public async Task Remove_ShouldRemoveStudentFromDatabase()
    {
        // Arrange
        var student = new StudentEntity { Id = Guid.NewGuid(), Purpose = "Test Purpose", UserId = Guid.NewGuid() };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(student);
        await _context.SaveChangesAsync();

        // Assert
        var deletedStudent = await _context.Students.FindAsync(student.Id);
        Assert.Null(deletedStudent);
    }
}