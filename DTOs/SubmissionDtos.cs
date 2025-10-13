using System.ComponentModel.DataAnnotations;
using FormEngineAPI.Models;

namespace FormEngineAPI.DTOs;

/// <summary>
/// DTO para filtros de busca de submissões
/// </summary>
public class FiltroSubmissaoFormularioDto
{
    /// <summary>
    /// ID do formulário (opcional)
    /// </summary>
    public int? FormId { get; set; }

    /// <summary>
    /// ID do usuário que criou a submissão (opcional)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Status da submissão (opcional)
    /// </summary>
    public StatusSubmissao? Status { get; set; }

    /// <summary>
    /// Data inicial para filtro por data de criação
    /// </summary>
    public DateTime? DataInicialCriacao { get; set; }

    /// <summary>
    /// Data final para filtro por data de criação
    /// </summary>
    public DateTime? DataFinalCriacao { get; set; }

    /// <summary>
    /// Data inicial para filtro por data de submissão
    /// </summary>
    public DateTime? DataInicialSubmissao { get; set; }

    /// <summary>
    /// Data final para filtro por data de submissão
    /// </summary>
    public DateTime? DataFinalSubmissao { get; set; }

    /// <summary>
    /// ID do usuário aprovador (opcional)
    /// </summary>
    public int? UsuarioAprovadorId { get; set; }

    /// <summary>
    /// Incluir submissões excluídas logicamente
    /// </summary>
    public bool IncluirExcluidas { get; set; } = false;

    /// <summary>
    /// Página para paginação
    /// </summary>
    public int Pagina { get; set; } = 1;

    /// <summary>
    /// Tamanho da página para paginação
    /// </summary>
    public int TamanhoPagina { get; set; } = 20;

    /// <summary>
    /// Campo para ordenação
    /// </summary>
    public string? CampoOrdenacao { get; set; } = "CreatedAt";

    /// <summary>
    /// Direção da ordenação (asc/desc)
    /// </summary>
    public string DirecaoOrdenacao { get; set; } = "desc";
}

/// <summary>
/// DTO para criação de nova submissão
/// </summary>
public class CriarSubmissaoDto
{
    /// <summary>
    /// ID do formulário
    /// </summary>
    [Required]
    public int FormId { get; set; }

    /// <summary>
    /// Dados JSON da submissão
    /// </summary>
    [Required]
    public string DataJson { get; set; } = string.Empty;

    /// <summary>
    /// Status inicial da submissão (padrão: Rascunho)
    /// </summary>
    public StatusSubmissao Status { get; set; } = StatusSubmissao.Rascunho;

    /// <summary>
    /// Comentário inicial (opcional)
    /// </summary>
    [StringLength(1000)]
    public string? ComentarioInicial { get; set; }
}

/// <summary>
/// DTO para atualização de submissão
/// </summary>
public class AtualizarSubmissaoDto
{
    /// <summary>
    /// Dados JSON da submissão
    /// </summary>
    [Required]
    public string DataJson { get; set; } = string.Empty;

    /// <summary>
    /// Comentário da atualização (opcional)
    /// </summary>
    [StringLength(1000)]
    public string? Comentario { get; set; }

    /// <summary>
    /// Versão para controle de concorrência
    /// </summary>
    [Required]
    public int Versao { get; set; }
}

/// <summary>
/// DTO para mudança de status da submissão
/// </summary>
public class MudarStatusSubmissaoDto
{
    /// <summary>
    /// Novo status da submissão
    /// </summary>
    [Required]
    public StatusSubmissao NovoStatus { get; set; }

    /// <summary>
    /// Comentário obrigatório para mudança de status
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Comentario { get; set; } = string.Empty;

    /// <summary>
    /// Motivo da rejeição (obrigatório se status for Rejeitado)
    /// </summary>
    [StringLength(1000)]
    public string? MotivoRejeicao { get; set; }

    /// <summary>
    /// Versão para controle de concorrência
    /// </summary>
    [Required]
    public int Versao { get; set; }
}

/// <summary>
/// DTO básico de resposta para submissão
/// </summary>
public class SubmissaoFormularioDto
{
    /// <summary>
    /// ID da submissão
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID do formulário
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// Nome do formulário
    /// </summary>
    public string FormName { get; set; } = string.Empty;

    /// <summary>
    /// ID do usuário que criou
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Nome do usuário que criou
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário que criou
    /// </summary>
    public string UserEmail { get; set; } = string.Empty;

    /// <summary>
    /// Status atual da submissão
    /// </summary>
    public StatusSubmissao Status { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização
    /// </summary>
    public DateTime DataAtualizacao { get; set; }

    /// <summary>
    /// Data de submissão (quando saiu do rascunho)
    /// </summary>
    public DateTime? DataSubmissao { get; set; }

    /// <summary>
    /// Data de aprovação
    /// </summary>
    public DateTime? DataAprovacao { get; set; }

    /// <summary>
    /// ID do usuário aprovador
    /// </summary>
    public int? UsuarioAprovadorId { get; set; }

    /// <summary>
    /// Nome do usuário aprovador
    /// </summary>
    public string? UsuarioAprovadorName { get; set; }

    /// <summary>
    /// Versão atual
    /// </summary>
    public int Versao { get; set; }

    /// <summary>
    /// Indica se foi excluída
    /// </summary>
    public bool Excluido { get; set; }
}

/// <summary>
/// DTO detalhado de resposta para submissão
/// </summary>
public class DetalheSubmissaoFormularioDto : SubmissaoFormularioDto
{
    /// <summary>
    /// Dados JSON da submissão
    /// </summary>
    public string DataJson { get; set; } = string.Empty;

    /// <summary>
    /// Motivo da rejeição (quando aplicável)
    /// </summary>
    public string? MotivoRejeicao { get; set; }

    /// <summary>
    /// Endereço IP
    /// </summary>
    public string? EnderecoIp { get; set; }

    /// <summary>
    /// User Agent
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Data de exclusão
    /// </summary>
    public DateTime? DataExclusao { get; set; }

    /// <summary>
    /// Histórico de ações (resumido)
    /// </summary>
    public List<HistoricoSubmissaoDto> Historicos { get; set; } = new();
}

/// <summary>
/// DTO para histórico de submissão
/// </summary>
public class HistoricoSubmissaoDto
{
    /// <summary>
    /// ID do histórico
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Ação realizada
    /// </summary>
    public AcaoSubmissao Acao { get; set; }

    /// <summary>
    /// Data da ação
    /// </summary>
    public DateTime DataAcao { get; set; }

    /// <summary>
    /// ID do usuário que realizou a ação
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Nome do usuário que realizou a ação
    /// </summary>
    public string UsuarioName { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário que realizou a ação
    /// </summary>
    public string UsuarioEmail { get; set; } = string.Empty;

    /// <summary>
    /// Comentário da ação
    /// </summary>
    public string? Comentario { get; set; }

    /// <summary>
    /// Status anterior
    /// </summary>
    public StatusSubmissao? StatusAnterior { get; set; }

    /// <summary>
    /// Novo status
    /// </summary>
    public StatusSubmissao? NovoStatus { get; set; }

    /// <summary>
    /// Endereço IP
    /// </summary>
    public string? EnderecoIp { get; set; }
}

/// <summary>
/// DTO para resultado paginado
/// </summary>
/// <typeparam name="T">Tipo dos itens da lista</typeparam>
public class ResultadoPaginadoDto<T>
{
    /// <summary>
    /// Lista de itens da página atual
    /// </summary>
    public List<T> Itens { get; set; } = new();

    /// <summary>
    /// Total de itens (sem paginação)
    /// </summary>
    public int TotalItens { get; set; }

    /// <summary>
    /// Página atual
    /// </summary>
    public int PaginaAtual { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int TamanhoPagina { get; set; }

    /// <summary>
    /// Indica se há página anterior
    /// </summary>
    public bool TemPaginaAnterior => PaginaAtual > 1;

    /// <summary>
    /// Indica se há próxima página
    /// </summary>
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;
}

/// <summary>
/// DTO para estatísticas de submissões
/// </summary>
public class EstatisticasSubmissaoDto
{
    /// <summary>
    /// Total de submissões por status
    /// </summary>
    public Dictionary<StatusSubmissao, int> TotalPorStatus { get; set; } = new();

    /// <summary>
    /// Total geral de submissões
    /// </summary>
    public int TotalGeral { get; set; }

    /// <summary>
    /// Submissões criadas hoje
    /// </summary>
    public int CriadasHoje { get; set; }

    /// <summary>
    /// Submissões criadas esta semana
    /// </summary>
    public int CriadasEstaSemana { get; set; }

    /// <summary>
    /// Submissões criadas este mês
    /// </summary>
    public int CriadasEsteMes { get; set; }

    /// <summary>
    /// Submissões pendentes de aprovação
    /// </summary>
    public int PendentesAprovacao { get; set; }

    /// <summary>
    /// Tempo médio de aprovação (em horas)
    /// </summary>
    public double? TempoMedioAprovacao { get; set; }
}

/// <summary>
/// DTO para resposta de operações de escrita
/// </summary>
public class RespostaOperacaoDto
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    public bool Sucesso { get; set; }

    /// <summary>
    /// Mensagem da operação
    /// </summary>
    public string Mensagem { get; set; } = string.Empty;

    /// <summary>
    /// ID do recurso criado/atualizado (quando aplicável)
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Erros de validação (quando aplicáveis)
    /// </summary>
    public List<string> Erros { get; set; } = new();

    /// <summary>
    /// Dados adicionais da resposta
    /// </summary>
    public object? Dados { get; set; }
}

// DTOs compatíveis com o sistema existente
/// <summary>
/// DTO de submissão compatível com o sistema existente
/// </summary>
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

/// <summary>
/// DTO de criação compatível com o sistema existente
/// </summary>
public class CreateSubmissionDto
{
    public int FormId { get; set; }
    public string DataJson { get; set; } = string.Empty;
}
