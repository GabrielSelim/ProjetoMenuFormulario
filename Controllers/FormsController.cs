using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormEngineAPI.DTOs;
using FormEngineAPI.Services;

namespace FormEngineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FormsController : ControllerBase
{
    private readonly IFormService _formService;

    public FormsController(IFormService formService)
    {
        _formService = formService;
    }

    /// <summary>
    /// Lista todos os formulários
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FormDto>>> GetAllForms()
    {
        var forms = await _formService.GetAllFormsAsync();
        return Ok(forms);
    }

    /// <summary>
    /// Busca um formulário por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<FormDto>> GetFormById(int id)
    {
        var form = await _formService.GetFormByIdAsync(id);
        if (form == null)
            return NotFound(new { message = "Formulário não encontrado" });

        return Ok(form);
    }

    /// <summary>
    /// Cria um novo formulário
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin,gestor")]
    public async Task<ActionResult<FormDto>> CreateForm([FromBody] CreateFormDto createFormDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var form = await _formService.CreateFormAsync(createFormDto, userId);
            return CreatedAtAction(nameof(GetFormById), new { id = form.Id }, form);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza um formulário existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin,gestor")]
    public async Task<ActionResult<FormDto>> UpdateForm(int id, [FromBody] UpdateFormDto updateFormDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var form = await _formService.UpdateFormAsync(id, updateFormDto, userId);
            return Ok(form);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove um formulário
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteForm(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _formService.DeleteFormAsync(id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
