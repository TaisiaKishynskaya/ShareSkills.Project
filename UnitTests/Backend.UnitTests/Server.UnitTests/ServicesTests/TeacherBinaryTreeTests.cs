using System.Reflection;
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
            new TeacherEntity { Id = Guid.NewGuid() },
            new TeacherEntity { Id = Guid.NewGuid() }
        };

        _teacherRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teachers);

        // Act
        // The constructor of TeacherBinaryTree is called in the test setup

        // Assert
        Assert.NotNull(_tree.Root); // Ensure that root is set
        // Похоже, что тест не работает, потому что Root равен null, что означает, что метод InitializeTreeAsync может неправильно заполнять дерево или неправильно ожидать его в конструкторе.
        // Вот пошаговое руководство, которое поможет диагностировать и устранить проблему:
        // 1. Проверьте выполнение конструктора: Убедитесь, что конструктор TeacherBinaryTree выполняется и ожидает InitializeTreeAsync. Поскольку InitializeTreeAsync является приватным и вызывается из конструктора, он должен быть правильно вызван и ожидаем.
        // 2. Убедитесь в правильной настройке мока: Убедитесь, что _teacherRepositoryMock настроен правильно и что GetAllAsync возвращает ожидаемый список учителей.
        // 3. Отладьте инициализацию: Если возможно, отладьте конструктор и метод InitializeTreeAsync, чтобы убедиться, что они ведут себя так, как ожидается.
        
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
    public void Insert_ShouldLogInsertionOfRoot()
    {
        // Arrange
        var teacher = new TeacherEntity { Id = Guid.NewGuid() };

        // Act
        _tree.Insert(teacher);

        // Assert
        _loggerMock.Verify(logger => logger.LogInformation("Inserted root teacher: {TeacherId}", teacher.Id), Times.Once); 
        // xUnit и Moq не поддерживают проверку вызовов логгеров с помощью LogInformation или других методов логирования непосредственно в выражениях настройки или проверки из-за того, как эти методы реализованы.
        // Подход к проверке логинга:
        // 1. Создание Custom Logger Capture: Можно создать пользовательский логгер, который будет перехватывать логи, и проверить, что перехвачены ожидаемые логи. Это позволяет избежать прямого использования LogInformation в настройках Moq.
        // 2. Используйте реализацию Custom Logger: Реализуйте простой логгер, который сохраняет логи в списке, который вы можете затем просмотреть в своем тесте.
    }
    
    
    [Fact]
    public void InsertRec_ShouldLogInsertion()
    {
        // Arrange
        var teacher1 = new TeacherEntity { Id = Guid.NewGuid(), SkillId = Guid.NewGuid() };
        var teacher2 = new TeacherEntity { Id = Guid.NewGuid(), SkillId = Guid.NewGuid() };
        var rootNode = new TeacherNode(teacher1);
        var newNode = new TeacherNode(teacher2);

        // Act
        var method = typeof(TeacherBinaryTree).GetMethod("InsertRec", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(_tree, new object[] { rootNode, newNode });

        // Assert - снова проблема с логгером
        _loggerMock.Verify(logger => logger.LogInformation(
            "Inserted teacher {TeacherId} to the right of {RootTeacherId}", teacher2.Id, teacher1.Id), Times.Once);
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
    
    [Fact]
    public void Search_ShouldLogInformation_WhenSearchingForTeacher()
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

        _tree.Insert(teacher);

        // Act
        _tree.Search(skillId, levelId, classTimeId);

        // Assert - снова проблема с логгером
        _loggerMock.Verify(logger => logger.LogInformation(
            "Searching for teacher with SkillId: {SkillId}, LevelId: {LevelId}, ClassTimeId: {ClassTimeId}",
            skillId, levelId, classTimeId), Times.Once);

        _loggerMock.Verify(logger => logger.LogInformation(
            "Found matching teacher: {TeacherId}", teacher.Id), Times.Once);
    }

    [Fact]
    public void Search_ShouldLogInformation_WhenTeacherNotFound()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var levelId = Guid.NewGuid();
        var classTimeId = Guid.NewGuid();

        // Act
        _tree.Search(skillId, levelId, classTimeId);

        // Assert - снова проблема с логгером
        _loggerMock.Verify(logger => logger.LogInformation(
            "Searching for teacher with SkillId: {SkillId}, LevelId: {LevelId}, ClassTimeId: {ClassTimeId}",
            skillId, levelId, classTimeId), Times.Once);

        _loggerMock.Verify(logger => logger.LogInformation(
            "Teacher not found."), Times.Once);
    }
}