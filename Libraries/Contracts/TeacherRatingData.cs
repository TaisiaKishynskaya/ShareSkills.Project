using Microsoft.ML.Data;

namespace Libraries.Contracts;

public class TeacherRatingData
{
    //public string UserId { get; set; }
    //public string TeacherId { get; set; }
    //public float Label { get; set; }

    [LoadColumn(0)] public uint UserId;
    [LoadColumn(1)] public uint TeacherId;
    [LoadColumn(2)] public float Rating;
}
