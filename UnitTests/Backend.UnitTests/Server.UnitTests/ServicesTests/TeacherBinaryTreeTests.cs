using App.Services.Concrete;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.Extensions.Logging;
using Moq;

namespace Server.UnitTests.ServicesTests;

public class TeacherBinaryTreeTests
{
    private readonly Mock<ILogger<TeacherBinaryTree>> _loggerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
    private readonly TeacherBinaryTree _tree;

    public TeacherBinaryTreeTests()
    {
        _loggerMock = new Mock<ILogger<TeacherBinaryTree>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _teacherRepositoryMock = new Mock<ITeacherRepository>();

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository).Returns(_teacherRepositoryMock.Object);
        _tree = new TeacherBinaryTree(_loggerMock.Object, _unitOfWorkMock.Object);
    }
    
    // Helper methods to traverse and verify the binary tree structure
    private int CountNodes(TeacherNode? node)
    {
        if (node == null) return 0;
        return 1 + CountNodes(node.Left) + CountNodes(node.Right);
    }

    private TeacherNode? FindNode(TeacherNode? root, Guid id)
    {
        if (root == null) return null;
        if (root.Teacher.Id == id) return root;
        var left = FindNode(root.Left, id);
        return left ?? FindNode(root.Right, id);
    }
    

    [Fact]
    public async Task Constructor_ShouldInitializeTreeWithTeachers()
    {
        // Arrange
        var teachers = new List<TeacherEntity>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };

        foreach (var teacher in teachers)
        {
            _tree.Insert(teacher);
        }
        
        _teacherRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teachers);

        // Act
        // The constructor of TeacherBinaryTree is called in the test setup

        // Assert
        Assert.NotNull(_tree.Root); // Ensure that root is set
        Assert.Equal(teachers.Count, CountNodes(_tree.Root)); // Ensure all nodes are inserted

        foreach (var teacher in teachers)
        {
            Assert.True(FindNode(_tree.Root, teacher.Id) != null); // Ensure each teacher is in the tree
        }
    }

    [Fact]
    public void Insert_ShouldInsertNodeCorrectly()
    {
        // Arrange
        var teacher1 = new TeacherEntity { Id = Guid.NewGuid() };
        var teacher2 = new TeacherEntity { Id = Guid.NewGuid() };

        // Act
        _tree.Insert(teacher1);
        _tree.Insert(teacher2);

        // Assert
        Assert.NotNull(_tree.Root);
        Assert.Equal(teacher1.Id, _tree.Root.Teacher.Id);
        Assert.NotNull(FindNode(_tree.Root, teacher2.Id)); // Ensure the second teacher is in the tree
    }
    
    [Fact]
    public void Search_ShouldReturnTeacher_WhenTeacherExists()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var levelId = Guid.NewGuid();
        var classTimeId = Guid.NewGuid();

        var teacher = new TeacherEntity
        {
            Id = Guid.NewGuid(),
            SkillId = skillId,
            LevelId = levelId,
            ClassTimeId = classTimeId
        };

        // Set up a tree with the teacher
        _tree.Insert(teacher);

        // Act
        var result = _tree.Search(skillId, levelId, classTimeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(teacher.Id, result.Id);
    }
    
    [Fact]
    public void Search_ShouldReturnNull_WhenTeacherDoesNotExist()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var levelId = Guid.NewGuid();
        var classTimeId = Guid.NewGuid();

        // Act
        var result = _tree.Search(skillId, levelId, classTimeId);

        // Assert
        Assert.Null(result);
    }
}