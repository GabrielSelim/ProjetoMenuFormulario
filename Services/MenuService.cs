using FormEngineAPI.Models;
using FormEngineAPI.DTOs;
using FormEngineAPI.Repositories;
using AutoMapper;
using System.Text.Json;

namespace FormEngineAPI.Services;

public interface IMenuService
{
    Task<IEnumerable<MenuDto>> GetAllMenusAsync(string? userRole = null);
    Task<MenuDto?> GetMenuByIdAsync(int id);
    Task<MenuDto> CreateMenuAsync(CreateMenuDto createMenuDto, int userId);
    Task<MenuDto> UpdateMenuAsync(int id, UpdateMenuDto updateMenuDto, int userId);
    Task DeleteMenuAsync(int id, int userId);
}

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IMapper _mapper;

    public MenuService(
        IMenuRepository menuRepository,
        IActivityLogRepository activityLogRepository,
        IMapper mapper)
    {
        _menuRepository = menuRepository;
        _activityLogRepository = activityLogRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MenuDto>> GetAllMenusAsync(string? userRole = null)
    {
        var menus = await _menuRepository.GetMenusWithChildrenAsync();
        var menuDtos = _mapper.Map<IEnumerable<MenuDto>>(menus);

        // Filter by role if provided
        if (!string.IsNullOrEmpty(userRole))
        {
            menuDtos = menuDtos.Where(m => IsMenuAllowedForRole(m.RolesAllowed, userRole));
            
            foreach (var menu in menuDtos)
            {
                menu.Children = menu.Children
                    .Where(c => IsMenuAllowedForRole(c.RolesAllowed, userRole))
                    .ToList();
            }
        }

        return menuDtos;
    }

    public async Task<MenuDto?> GetMenuByIdAsync(int id)
    {
        var menu = await _menuRepository.GetByIdAsync(id);
        return menu != null ? _mapper.Map<MenuDto>(menu) : null;
    }

    public async Task<MenuDto> CreateMenuAsync(CreateMenuDto createMenuDto, int userId)
    {
        var menu = _mapper.Map<Menu>(createMenuDto);
        menu.CreatedAt = DateTime.UtcNow;
        menu.UpdatedAt = DateTime.UtcNow;

        await _menuRepository.AddAsync(menu);

        // Log activity
        await _activityLogRepository.AddAsync(new ActivityLog
        {
            UserId = userId,
            Action = "CREATE",
            Entity = "Menu",
            EntityId = menu.Id,
            Details = $"Menu '{menu.Name}' criado"
        });

        return _mapper.Map<MenuDto>(menu);
    }

    public async Task<MenuDto> UpdateMenuAsync(int id, UpdateMenuDto updateMenuDto, int userId)
    {
        var menu = await _menuRepository.GetByIdAsync(id);
        if (menu == null)
        {
            throw new Exception("Menu não encontrado.");
        }

        menu.Name = updateMenuDto.Name;
        menu.ContentType = updateMenuDto.ContentType;
        menu.UrlOrPath = updateMenuDto.UrlOrPath;
        menu.RolesAllowed = updateMenuDto.RolesAllowed;
        menu.Order = updateMenuDto.Order;
        menu.Icon = updateMenuDto.Icon;
        menu.ParentId = updateMenuDto.ParentId;
        menu.UpdatedAt = DateTime.UtcNow;

        await _menuRepository.UpdateAsync(menu);

        // Log activity
        await _activityLogRepository.AddAsync(new ActivityLog
        {
            UserId = userId,
            Action = "UPDATE",
            Entity = "Menu",
            EntityId = menu.Id,
            Details = $"Menu '{menu.Name}' atualizado"
        });

        return _mapper.Map<MenuDto>(menu);
    }

    public async Task DeleteMenuAsync(int id, int userId)
    {
        var menu = await _menuRepository.GetByIdAsync(id);
        if (menu == null)
        {
            throw new Exception("Menu não encontrado.");
        }

        await _menuRepository.DeleteAsync(menu);

        // Log activity
        await _activityLogRepository.AddAsync(new ActivityLog
        {
            UserId = userId,
            Action = "DELETE",
            Entity = "Menu",
            EntityId = id,
            Details = $"Menu '{menu.Name}' excluído"
        });
    }

    private bool IsMenuAllowedForRole(string rolesAllowed, string userRole)
    {
        if (string.IsNullOrEmpty(rolesAllowed))
            return true;

        try
        {
            var roles = JsonSerializer.Deserialize<string[]>(rolesAllowed);
            return roles == null || roles.Length == 0 || roles.Contains(userRole);
        }
        catch
        {
            return true;
        }
    }
}
