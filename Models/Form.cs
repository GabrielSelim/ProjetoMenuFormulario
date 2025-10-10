namespace FormEngineAPI.Models;

public class Form
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SchemaJson { get; set; } = string.Empty;
    public string RolesAllowed { get; set; } = string.Empty; // JSON array de roles
    public string Version { get; set; } = "1.0.0";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<FormSubmission> FormSubmissions { get; set; } = new List<FormSubmission>();
}
