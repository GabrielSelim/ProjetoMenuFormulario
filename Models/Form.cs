namespace FormEngineAPI.Models;

public class Form
{
    public int Id { get; set; }
    public int? OriginalFormId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SchemaJson { get; set; } = string.Empty;
    public string RolesAllowed { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public bool IsLatest { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public Form? OriginalForm { get; set; }
    public ICollection<Form> Versions { get; set; } = new List<Form>();
    public ICollection<FormSubmission> FormSubmissions { get; set; } = new List<FormSubmission>();
}
