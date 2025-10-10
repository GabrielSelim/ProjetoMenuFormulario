namespace FormEngineAPI.DTOs;

public class FormDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SchemaJson { get; set; } = string.Empty;
    public string RolesAllowed { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateFormDto
{
    public string Name { get; set; } = string.Empty;
    public string SchemaJson { get; set; } = string.Empty;
    public string RolesAllowed { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
}

public class UpdateFormDto
{
    public string Name { get; set; } = string.Empty;
    public string SchemaJson { get; set; } = string.Empty;
    public string RolesAllowed { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}
