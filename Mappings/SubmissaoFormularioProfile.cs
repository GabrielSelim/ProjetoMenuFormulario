using AutoMapper;
using FormEngineAPI.Models;
using FormEngineAPI.DTOs;

namespace FormEngineAPI.Mappings
{
    /// <summary>
    /// Profile do AutoMapper para submissões de formulário
    /// </summary>
    public class SubmissaoFormularioProfile : Profile
    {
        public SubmissaoFormularioProfile()
        {
            // Mapeamento de FormSubmission para DTOs de resposta
            CreateMap<FormSubmission, SubmissaoFormularioDto>()
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Form.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UsuarioAprovadorName, opt => opt.MapFrom(src => src.UsuarioAprovador != null ? src.UsuarioAprovador.Name : null));

            CreateMap<FormSubmission, DetalheSubmissaoFormularioDto>()
                .IncludeBase<FormSubmission, SubmissaoFormularioDto>()
                .ForMember(dest => dest.Historicos, opt => opt.MapFrom(src => src.Historicos));

            // Mapeamento de HistoricoFormSubmission para DTO
            CreateMap<HistoricoFormSubmission, HistoricoSubmissaoDto>()
                .ForMember(dest => dest.UsuarioName, opt => opt.MapFrom(src => src.Usuario.Name))
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario.Email));

            // Mapeamentos de criação (DTOs para Models)
            CreateMap<CriarSubmissaoDto, FormSubmission>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Versao, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.Excluido, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Form, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioAprovador, opt => opt.Ignore())
                .ForMember(dest => dest.Historicos, opt => opt.Ignore());

            // Mapeamento de atualização
            CreateMap<AtualizarSubmissaoDto, FormSubmission>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FormId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Form, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioAprovador, opt => opt.Ignore())
                .ForMember(dest => dest.Historicos, opt => opt.Ignore());
        }
    }

    /// <summary>
    /// Profile do AutoMapper para manter compatibilidade com sistema existente
    /// </summary>
    public class CompatibilidadeProfile : Profile
    {
        public CompatibilidadeProfile()
        {
            // Mapeamentos para manter compatibilidade com DTOs existentes
            CreateMap<FormSubmission, FormSubmissionDto>()
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Form != null ? src.Form.Name : null))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : null));

            CreateMap<CreateSubmissionDto, FormSubmission>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusSubmissao.Rascunho))
                .ForMember(dest => dest.Versao, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.Excluido, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Form, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioAprovador, opt => opt.Ignore())
                .ForMember(dest => dest.Historicos, opt => opt.Ignore());
        }
    }

    /// <summary>
    /// Profile do AutoMapper para histórico e auditoria
    /// </summary>
    public class HistoricoProfile : Profile
    {
        public HistoricoProfile()
        {
            CreateMap<HistoricoFormSubmission, HistoricoSubmissaoDto>()
                .ForMember(dest => dest.UsuarioName, opt => opt.MapFrom(src => src.Usuario.Name))
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario.Email));

            // Mapeamento reverso para criação de histórico
            CreateMap<HistoricoSubmissaoDto, HistoricoFormSubmission>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FormSubmission, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore());
        }
    }

    /// <summary>
    /// Profile do AutoMapper para usuários (para contextos de aprovação)
    /// </summary>
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<User, UsuarioResumoDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
        }
    }

    /// <summary>
    /// Profile do AutoMapper para formulários (para contextos de submissão)
    /// </summary>
    public class FormularioProfile : Profile
    {
        public FormularioProfile()
        {
            CreateMap<Form, FormularioResumoDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Versao, opt => opt.MapFrom(src => src.Version))
                .ForMember(dest => dest.RolesPermitidas, opt => opt.MapFrom(src => src.RolesAllowed));
        }
    }
}

// DTOs auxiliares para os profiles
namespace FormEngineAPI.DTOs
{
    /// <summary>
    /// DTO resumido de usuário para contextos de aprovação
    /// </summary>
    public class UsuarioResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO resumido de formulário para contextos de submissão
    /// </summary>
    public class FormularioResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Versao { get; set; } = string.Empty;
        public string RolesPermitidas { get; set; } = string.Empty;
    }
}