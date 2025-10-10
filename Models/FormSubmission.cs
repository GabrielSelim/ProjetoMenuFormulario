namespace FormEngineAPI.Models;

public class FormSubmission
{
    public int Id { get; set; }
    public int FormId { get; set; }
    public int UserId { get; set; }
    public string DataJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Form Form { get; set; } = null!;
    public User User { get; set; } = null!;
}
