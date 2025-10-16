using FormEngineAPI.Models;
using FormEngineAPI.DTOs;
using FormEngineAPI.Repositories;
using AutoMapper;

namespace FormEngineAPI.Services;

public interface IFormService
{
    Task<IEnumerable<FormDto>> GetAllFormsAsync();
    Task<FormDto?> GetFormByIdAsync(int id);
    Task<FormDto> CreateFormAsync(CreateFormDto createFormDto, int userId);
    Task<FormDto> UpdateFormAsync(int id, UpdateFormDto updateFormDto, int userId);
    Task DeleteFormAsync(int id, int userId);
    Task<IEnumerable<FormVersionDto>> GetFormVersionsAsync(int originalFormId);
    Task<FormDto?> GetLatestVersionAsync(int originalFormId);
    Task<FormDto?> GetSpecificVersionAsync(int originalFormId, string version);
}

public class FormService : IFormService
{
    private readonly IFormRepository _formRepository;
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IMapper _mapper;

    public FormService(
        IFormRepository formRepository,
        IActivityLogRepository activityLogRepository,
        IMapper mapper)
    {
        _formRepository = formRepository;
        _activityLogRepository = activityLogRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FormDto>> GetAllFormsAsync()
    {
        var forms = await _formRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<FormDto>>(forms);
    }

    public async Task<FormDto?> GetFormByIdAsync(int id)
    {
        var form = await _formRepository.GetByIdAsync(id);
        return form != null ? _mapper.Map<FormDto>(form) : null;
    }

    public async Task<FormDto> CreateFormAsync(CreateFormDto createFormDto, int userId)
    {
        var form = _mapper.Map<Form>(createFormDto);
        form.CreatedAt = DateTime.UtcNow;
        form.UpdatedAt = DateTime.UtcNow;

        await _formRepository.AddAsync(form);

        // Log activity
        await _activityLogRepository.AddAsync(new ActivityLog
        {
            UserId = userId,
            Action = "CREATE",
            Entity = "Form",
            EntityId = form.Id,
            Details = $"Formulário '{form.Name}' criado"
        });

        return _mapper.Map<FormDto>(form);
    }

    public async Task<FormDto> UpdateFormAsync(int id, UpdateFormDto updateFormDto, int userId)
    {
        var form = await _formRepository.GetByIdAsync(id);
        if (form == null)
        {
            throw new Exception("Formulário não encontrado.");
        }

        if (updateFormDto.CreateNewVersion)
        {
            var originalFormId = form.OriginalFormId ?? form.Id;
            
            await _formRepository.MarkAllVersionsAsNotLatestAsync(originalFormId);
            
            var nextVersion = await _formRepository.GetNextVersionAsync(originalFormId);
            
            var newForm = new Form
            {
                OriginalFormId = originalFormId,
                Name = updateFormDto.Name,
                SchemaJson = updateFormDto.SchemaJson,
                RolesAllowed = updateFormDto.RolesAllowed,
                Version = nextVersion,
                IsLatest = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _formRepository.AddAsync(newForm);

            await _activityLogRepository.AddAsync(new ActivityLog
            {
                UserId = userId,
                Action = "CREATE_VERSION",
                Entity = "Form",
                EntityId = newForm.Id,
                Details = $"Nova versão {nextVersion} do formulário '{newForm.Name}' criada"
            });

            return _mapper.Map<FormDto>(newForm);
        }
        else
        {
            form.Name = updateFormDto.Name;
            form.SchemaJson = updateFormDto.SchemaJson;
            form.RolesAllowed = updateFormDto.RolesAllowed;
            form.Version = updateFormDto.Version;
            form.UpdatedAt = DateTime.UtcNow;

            await _formRepository.UpdateAsync(form);

            await _activityLogRepository.AddAsync(new ActivityLog
            {
                UserId = userId,
                Action = "UPDATE",
                Entity = "Form",
                EntityId = form.Id,
                Details = $"Formulário '{form.Name}' atualizado"
            });

            return _mapper.Map<FormDto>(form);
        }
    }

    public async Task<IEnumerable<FormVersionDto>> GetFormVersionsAsync(int originalFormId)
    {
        var versions = await _formRepository.GetVersionsByOriginalIdAsync(originalFormId);
        return _mapper.Map<IEnumerable<FormVersionDto>>(versions);
    }

    public async Task<FormDto?> GetLatestVersionAsync(int originalFormId)
    {
        var form = await _formRepository.GetLatestVersionAsync(originalFormId);
        return form != null ? _mapper.Map<FormDto>(form) : null;
    }

    public async Task<FormDto?> GetSpecificVersionAsync(int originalFormId, string version)
    {
        var form = await _formRepository.GetSpecificVersionAsync(originalFormId, version);
        return form != null ? _mapper.Map<FormDto>(form) : null;
    }

    public async Task DeleteFormAsync(int id, int userId)
    {
        var form = await _formRepository.GetByIdAsync(id);
        if (form == null)
        {
            throw new Exception("Formulário não encontrado.");
        }

        await _formRepository.DeleteAsync(form);

        // Log activity
        await _activityLogRepository.AddAsync(new ActivityLog
        {
            UserId = userId,
            Action = "DELETE",
            Entity = "Form",
            EntityId = id,
            Details = $"Formulário '{form.Name}' excluído"
        });
    }
}
