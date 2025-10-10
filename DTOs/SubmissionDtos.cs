namespace FormEngineAPI.DTOs;

public class FormSubmissionDto
{
    public int Id { get; set; }
    public int FormId { get; set; }
    public int UserId { get; set; }
    public string DataJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? FormName { get; set; }
    public string? UserName { get; set; }
}

public class CreateSubmissionDto
{
    public int FormId { get; set; }
    public string DataJson { get; set; } = string.Empty;
}
