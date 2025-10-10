using FormEngineAPI.Models;
using FormEngineAPI.DTOs;
using FormEngineAPI.Repositories;
using AutoMapper;

namespace FormEngineAPI.Services;

public interface ISubmissionService
{
    Task<FormSubmissionDto> CreateSubmissionAsync(CreateSubmissionDto createSubmissionDto, int userId);
    Task<IEnumerable<FormSubmissionDto>> GetSubmissionsByFormIdAsync(int formId);
    Task<IEnumerable<FormSubmissionDto>> GetSubmissionsByUserIdAsync(int userId);
}

public class SubmissionService : ISubmissionService
{
    private readonly IFormSubmissionRepository _submissionRepository;
    private readonly IFormRepository _formRepository;
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IMapper _mapper;

    public SubmissionService(
        IFormSubmissionRepository submissionRepository,
        IFormRepository formRepository,
        IActivityLogRepository activityLogRepository,
        IMapper mapper)
    {
        _submissionRepository = submissionRepository;
        _formRepository = formRepository;
        _activityLogRepository = activityLogRepository;
        _mapper = mapper;
    }

    public async Task<FormSubmissionDto> CreateSubmissionAsync(CreateSubmissionDto createSubmissionDto, int userId)
    {
        var form = await _formRepository.GetByIdAsync(createSubmissionDto.FormId);
        if (form == null)
        {
            throw new Exception("Formulário não encontrado.");
        }

        var submission = new FormSubmission
        {
            FormId = createSubmissionDto.FormId,
            UserId = userId,
            DataJson = createSubmissionDto.DataJson,
            CreatedAt = DateTime.UtcNow
        };

        await _submissionRepository.AddAsync(submission);

        // Log activity
        await _activityLogRepository.AddAsync(new ActivityLog
        {
            UserId = userId,
            Action = "CREATE",
            Entity = "FormSubmission",
            EntityId = submission.Id,
            Details = $"Submissão criada para formulário '{form.Name}'"
        });

        return _mapper.Map<FormSubmissionDto>(submission);
    }

    public async Task<IEnumerable<FormSubmissionDto>> GetSubmissionsByFormIdAsync(int formId)
    {
        var submissions = await _submissionRepository.GetByFormIdAsync(formId);
        return _mapper.Map<IEnumerable<FormSubmissionDto>>(submissions);
    }

    public async Task<IEnumerable<FormSubmissionDto>> GetSubmissionsByUserIdAsync(int userId)
    {
        var submissions = await _submissionRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<FormSubmissionDto>>(submissions);
    }
}
