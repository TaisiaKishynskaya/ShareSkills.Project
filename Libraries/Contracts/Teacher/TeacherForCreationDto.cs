using Newtonsoft.Json;

namespace Libraries.Contracts.Teacher;

public class TeacherForCreationDto
{
    [JsonProperty("userId")]
    public required Guid UserId { get; set; }

    [JsonProperty("rating")]
    public required double Rating { get; set; }

    [JsonProperty("classTime")]
    public required string ClassTime { get; set; } 

    [JsonProperty("level")]
    public required string Level { get; set; }

    [JsonProperty("skill")]
    public required string Skill { get; set; } 
}
