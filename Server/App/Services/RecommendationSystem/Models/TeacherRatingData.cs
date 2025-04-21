using Microsoft.ML.Data;

namespace App.Services.RecommendationSystem.Models;

public class TeacherRatingData
{
    [LoadColumn(0)]
    //[ColumnName(@"StudentId")]
    public uint StudentId { get; set; }

    [LoadColumn(1)] 
    //[ColumnName(@"TeacherId")]
    public uint TeacherId { get; set; }
    [LoadColumn(2)]
    //[ColumnName(@"Label")]
    public float Label { get; set; }
}