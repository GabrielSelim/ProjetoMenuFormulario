using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FormEngineAPI.DTOs;
using FormEngineAPI.Services;

namespace FormEngineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// Lista todos os menus (filtrados por role do usuário)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuDto>>> GetAllMenus()
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var menus = await _menuService.GetAllMenusAsync(userRole);
        return Ok(menus);
    }

    /// <summary>
    /// Busca um menu por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MenuDto>> GetMenuById(int id)
    {
        var menu = await _menuService.GetMenuByIdAsync(id);
        if (menu == null)
            return NotFound(new { message = "Menu não encontrado" });

        return Ok(menu);
    }

    /// <summary>
    /// Cria um novo menu
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<MenuDto>> CreateMenu([FromBody] CreateMenuDto createMenuDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var menu = await _menuService.CreateMenuAsync(createMenuDto, userId);
            return CreatedAtAction(nameof(GetMenuById), new { id = menu.Id }, menu);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza um menu existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<MenuDto>> UpdateMenu(int id, [FromBody] UpdateMenuDto updateMenuDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var menu = await _menuService.UpdateMenuAsync(id, updateMenuDto, userId);
            return Ok(menu);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove um menu
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteMenu(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _menuService.DeleteMenuAsync(id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
