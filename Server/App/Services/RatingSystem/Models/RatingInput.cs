namespace App.Services.RatingSystem.Models;

public class RatingInput
{
    public string UserId { get; set; } = null!;      // Guid в текстовому форматі
    public string ItemId { get; set; } = null!;
    public float Label { get; set; }
}