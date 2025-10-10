namespace FormEngineAPI.Models;

public class ActivityLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; } = string.Empty; // CREATE, UPDATE, DELETE, VIEW
    public string Entity { get; set; } = string.Empty; // Form, Menu, User, etc.
    public int EntityId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Details { get; set; } = string.Empty; // JSON com detalhes adicionais
    
    // Navigation properties
    public User User { get; set; } = null!;
}
