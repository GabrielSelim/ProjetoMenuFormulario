using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormEngineAPI.Models;

public class FormSubmission
{
    /// <summary>
    /// ID único da submissão
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Referência para o formulário
    /// </summary>
    [Required]
    public int FormId { get; set; }

    /// <summary>
    /// ID do formulário original (para versionamento)
    /// </summary>
    public int? OriginalFormId { get; set; }

    /// <summary>
    /// Versão do formulário usada na submissão
    /// </summary>
    [StringLength(10)]
    public string? FormVersion { get; set; }

    /// <summary>
    /// Referência para o usuário que criou a submissão
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Dados JSON da submissão
    /// </summary>
    [Required]
    [Column(TypeName = "JSON")]
    public string DataJson { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação da submissão
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Status atual da submissão
    /// </summary>
    [Required]
    public StatusSubmissao Status { get; set; } = StatusSubmissao.Rascunho;

    /// <summary>
    /// Data da última atualização
    /// </summary>
    [Required]
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data em que a submissão foi enviada (saiu do rascunho)
    /// </summary>
    public DateTime? DataSubmissao { get; set; }

    /// <summary>
    /// Data em que a submissão foi aprovada
    /// </summary>
    public DateTime? DataAprovacao { get; set; }

    /// <summary>
    /// ID do usuário que aprovou/rejeitou a submissão
    /// </summary>
    public int? UsuarioAprovadorId { get; set; }

    /// <summary>
    /// Motivo da rejeição (quando aplicável)
    /// </summary>
    [StringLength(1000)]
    public string? MotivoRejeicao { get; set; }

    /// <summary>
    /// Endereço IP de onde foi criada/atualizada a submissão
    /// </summary>
    [StringLength(45)]
    public string? EnderecoIp { get; set; }

    /// <summary>
    /// User Agent do navegador/aplicação
    /// </summary>
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Versão da submissão (para controle de concorrência)
    /// </summary>
    [Required]
    public int Versao { get; set; } = 1;

    /// <summary>
    /// Indica se a submissão foi excluída logicamente
    /// </summary>
    [Required]
    public bool Excluido { get; set; } = false;

    /// <summary>
    /// Data de exclusão lógica
    /// </summary>
    public DateTime? DataExclusao { get; set; }

    // Navigation properties
    /// <summary>
    /// Referência para o formulário
    /// </summary>
    public virtual Form Form { get; set; } = null!;

    /// <summary>
    /// Referência para o usuário que criou a submissão
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Referência para o usuário aprovador (quando aplicável)
    /// </summary>
    public virtual User? UsuarioAprovador { get; set; }

    /// <summary>
    /// Histórico de ações da submissão
    /// </summary>
    public virtual ICollection<HistoricoFormSubmission> Historicos { get; set; } = new List<HistoricoFormSubmission>();
}
