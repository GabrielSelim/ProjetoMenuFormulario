using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormEngineAPI.Models
{
    /// <summary>
    /// Histórico de ações realizadas em submissões de formulários
    /// </summary>
    public class HistoricoFormSubmission
    {
        /// <summary>
        /// ID único do registro de histórico
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Referência para a submissão do formulário
        /// </summary>
        [Required]
        public int FormSubmissionId { get; set; }

        /// <summary>
        /// Ação realizada na submissão
        /// </summary>
        [Required]
        public AcaoSubmissao Acao { get; set; }

        /// <summary>
        /// Data e hora da ação
        /// </summary>
        [Required]
        public DateTime DataAcao { get; set; }

        /// <summary>
        /// ID do usuário que realizou a ação
        /// </summary>
        [Required]
        public int UsuarioId { get; set; }

        /// <summary>
        /// Comentário ou motivo da ação (opcional)
        /// </summary>
        [StringLength(1000)]
        public string? Comentario { get; set; }

        /// <summary>
        /// Status anterior da submissão (para auditoria)
        /// </summary>
        public StatusSubmissao? StatusAnterior { get; set; }

        /// <summary>
        /// Novo status da submissão
        /// </summary>
        public StatusSubmissao? NovoStatus { get; set; }

        /// <summary>
        /// Endereço IP de onde foi feita a ação
        /// </summary>
        [StringLength(45)]
        public string? EnderecoIp { get; set; }

        /// <summary>
        /// User Agent do navegador/aplicação
        /// </summary>
        [StringLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Dados JSON das alterações (para auditoria detalhada)
        /// </summary>
        [Column(TypeName = "JSON")]
        public string? DadosAlteracao { get; set; }

        // Navigation Properties
        /// <summary>
        /// Referência para a submissão do formulário
        /// </summary>
        public virtual FormSubmission FormSubmission { get; set; } = null!;

        /// <summary>
        /// Referência para o usuário que realizou a ação
        /// </summary>
        public virtual User Usuario { get; set; } = null!;
    }
}