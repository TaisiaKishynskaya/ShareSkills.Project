namespace Libraries.Contracts.Report;

public class ReportDto
{
    public string TeacherName { get; set; } 
    public string TeacherSurname { get; set; }
    public string SkillName { get; set; }
    public int TotalTime { get; set; } 
    public List<string> Themes { get; set; }
}

