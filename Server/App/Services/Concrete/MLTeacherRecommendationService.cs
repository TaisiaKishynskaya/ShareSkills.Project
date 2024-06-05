using Microsoft.Extensions.Logging;
using Libraries.Entities.Concrete;

namespace App.Services.Concrete
{
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

        public TeacherBinaryTree(ILogger<TeacherBinaryTree> logger)
        {
            _logger = logger;
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
            _logger.LogInformation("Searching for teacher with SkillId: {SkillId}, LevelId: {LevelId}, ClassTimeId: {ClassTimeId}", skillId, levelId, classTimeId);
            return SearchRec(Root, skillId, levelId, classTimeId);
        }

        private TeacherEntity? SearchRec(TeacherNode? node, Guid skillId, Guid levelId, Guid classTimeId)
        {
            if (node == null)
            {
                _logger.LogInformation("GABELA: Node is null");
                return null; // Возвращаем null только если текущий узел null
            }

            if (node.Teacher != null && node.Teacher.SkillId == skillId && node.Teacher.LevelId == levelId && node.Teacher.ClassTimeId == classTimeId)
            {
                _logger.LogInformation("Found matching teacher: {TeacherId}", node.Teacher.Id);
                return node.Teacher; // Возвращаем учителя только если он удовлетворяет условиям поиска
            }

            var leftResult = SearchRec(node.Left, skillId, levelId, classTimeId);
            if (leftResult != null)
                return leftResult;

            var rightResult = SearchRec(node.Right, skillId, levelId, classTimeId);
            if (rightResult != null)
                return rightResult;

            return null; // Если текущий узел не удовлетворяет условиям поиска, продолжаем поиск в дереве
        }

    }
}
