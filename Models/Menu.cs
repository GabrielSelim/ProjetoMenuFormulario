namespace FormEngineAPI.Models;

public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string UrlOrPath { get; set; } = string.Empty;
    public string RolesAllowed { get; set; } = string.Empty;
    public int Order { get; set; } = 0;
    public string Icon { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int? OriginalFormId { get; set; }
    public string? FormVersion { get; set; }
    public bool UseLatestVersion { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } = new List<Menu>();
    public Form? OriginalForm { get; set; }
}
