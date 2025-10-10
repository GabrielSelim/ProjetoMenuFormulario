using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormEngineAPI.DTOs;
using FormEngineAPI.Services;

namespace FormEngineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    /// <summary>
    /// Cria uma nova submissão de formulário
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FormSubmissionDto>> CreateSubmission([FromBody] CreateSubmissionDto createSubmissionDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var submission = await _submissionService.CreateSubmissionAsync(createSubmissionDto, userId);
            return CreatedAtAction(nameof(GetSubmissionsByFormId), new { formId = submission.FormId }, submission);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lista todas as submissões de um formulário específico
    /// </summary>
    [HttpGet("form/{formId}")]
    [Authorize(Roles = "admin,gestor")]
    public async Task<ActionResult<IEnumerable<FormSubmissionDto>>> GetSubmissionsByFormId(int formId)
    {
        var submissions = await _submissionService.GetSubmissionsByFormIdAsync(formId);
        return Ok(submissions);
    }

    /// <summary>
    /// Lista todas as submissões de um usuário específico
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<FormSubmissionDto>>> GetSubmissionsByUserId(int userId)
    {
        // Users can only see their own submissions unless they're admin/gestor
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (currentUserId != userId && userRole != "admin" && userRole != "gestor")
        {
            return Forbid();
        }

        var submissions = await _submissionService.GetSubmissionsByUserIdAsync(userId);
        return Ok(submissions);
    }

    /// <summary>
    /// Lista todas as submissões do usuário atual
    /// </summary>
    [HttpGet("my-submissions")]
    public async Task<ActionResult<IEnumerable<FormSubmissionDto>>> GetMySubmissions()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var submissions = await _submissionService.GetSubmissionsByUserIdAsync(userId);
        return Ok(submissions);
    }
}
