namespace FormEngineAPI.DTOs;

public class MenuDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string UrlOrPath { get; set; } = string.Empty;
    public string RolesAllowed { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Icon { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int? OriginalFormId { get; set; }
    public string? FormVersion { get; set; }
    public bool UseLatestVersion { get; set; }
    public List<MenuDto> Children { get; set; } = new List<MenuDto>();
}

public class CreateMenuDto
{
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
}

public class UpdateMenuDto
{
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string UrlOrPath { get; set; } = string.Empty;
    public string RolesAllowed { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Icon { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int? OriginalFormId { get; set; }
    public string? FormVersion { get; set; }
    public bool UseLatestVersion { get; set; } = true;
}
