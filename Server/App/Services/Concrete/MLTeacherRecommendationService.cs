using Microsoft.Extensions.Logging;
using Libraries.Entities.Concrete;
using Libraries.Data.UnitOfWork.Abstract;
using System.Threading.Tasks;
using System.Linq;

namespace App.Services.Concrete;

public class TeacherNode
{
    public TeacherEntity Teacher { get; set; }
    public TeacherNode? Left { get; set; }
    public TeacherNode? Right { get; set; }

    public TeacherNode(TeacherEntity teacher)
    {
        Teacher = teacher;
        Left = null;
        Right = null;
    }
}

public class TeacherBinaryTree
{
    public TeacherNode? Root { get; set; }
    private readonly ILogger<TeacherBinaryTree> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TeacherBinaryTree(ILogger<TeacherBinaryTree> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        InitializeTreeAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeTreeAsync()
    {
        var teachers = await _unitOfWork.TeacherRepository.GetAllAsync();
        foreach (var teacher in teachers)
        {
            Insert(teacher);
        }
    }

    public void Insert(TeacherEntity teacher)
    {
        var newNode = new TeacherNode(teacher);

        if (Root == null)
        {
            Root = newNode;
            _logger.LogInformation("Inserted root teacher: {TeacherId}", teacher.Id);
            return;
        }

        InsertRec(Root, newNode);
    }

    private void InsertRec(TeacherNode root, TeacherNode newNode)
    {
        if (newNode.Teacher.SkillId.CompareTo(root.Teacher.SkillId) < 0)
        {
            if (root.Left == null)
            {
                root.Left = newNode;
                _logger.LogInformation("Inserted teacher {TeacherId} to the left of {RootTeacherId}", newNode.Teacher.Id, root.Teacher.Id);
            }
            else
            {
                InsertRec(root.Left, newNode);
            }
        }
        else
        {
            if (root.Right == null)
            {
                root.Right = newNode;
                _logger.LogInformation("Inserted teacher {TeacherId} to the right of {RootTeacherId}", newNode.Teacher.Id, root.Teacher.Id);
            }
            else
            {
                InsertRec(root.Right, newNode);
            }
        }
    }

    public TeacherEntity? Search(Guid skillId, Guid levelId, Guid classTimeId)
    {
        _logger.LogInformation(
            "Searching for teacher with SkillId: {SkillId}, LevelId: {LevelId}, ClassTimeId: {ClassTimeId}",
            skillId, levelId, classTimeId);

        var currentNode = Root;
        while (currentNode != null)
        {
            if (currentNode.Teacher != null && currentNode.Teacher.SkillId == skillId &&
                currentNode.Teacher.LevelId == levelId &&
                currentNode.Teacher.ClassTimeId == classTimeId)
            {
                _logger.LogInformation("Found matching teacher: {TeacherId}", currentNode.Teacher.Id);
                return currentNode.Teacher;
            }
            else if (skillId.CompareTo(currentNode.Teacher.SkillId) < 0)
            {
                currentNode = currentNode.Left;
            }
            else
            {
                currentNode = currentNode.Right;
            }
        }

        _logger.LogInformation("Teacher not found.");
        return null;
    }
}
