using Libraries.Entities.Concrete;

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

    public void Insert(TeacherEntity teacher)
    {
        var newNode = new TeacherNode(teacher);

        if (Root == null)
        {
            Root = newNode;
            return;
        }

        InsertRec(Root, newNode);
    }

    private void InsertRec(TeacherNode root, TeacherNode newNode)
    {
        if (newNode.Teacher.SkillId.CompareTo(root.Teacher.SkillId) < 0)
        {
            if (root.Left == null)
                root.Left = newNode;
            else
                InsertRec(root.Left, newNode);
        }
        else
        {
            if (root.Right == null)
                root.Right = newNode;
            else
                InsertRec(root.Right, newNode);
        }
    }

    public List<TeacherEntity> Search(Guid skillId, Guid levelId, Guid classTimeId)
    {
        var result = new List<TeacherEntity>();
        SearchRec(Root, skillId, levelId, classTimeId, result);
        return result;
    }

    private void SearchRec(TeacherNode? node, Guid skillId, Guid levelId, Guid classTimeId, List<TeacherEntity> result)
    {
        if (node == null)
            return;

        if (node.Teacher.SkillId == skillId && node.Teacher.LevelId == levelId && node.Teacher.ClassTimeId == classTimeId)
        {
            result.Add(node.Teacher);
        }

        SearchRec(node.Left, skillId, levelId, classTimeId, result);
        SearchRec(node.Right, skillId, levelId, classTimeId, result);
    }
}