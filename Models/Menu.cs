namespace FormEngineAPI.Models;

public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty; // form, page, link, etc.
    public string UrlOrPath { get; set; } = string.Empty;
    public string RolesAllowed { get; set; } = string.Empty; // JSON array de roles
    public int Order { get; set; } = 0;
    public string Icon { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } = new List<Menu>();
}
