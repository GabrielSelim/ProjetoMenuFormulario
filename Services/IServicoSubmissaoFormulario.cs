using FormEngineAPI.DTOs;
using FormEngineAPI.Models;

namespace FormEngineAPI.Services
{
    /// <summary>
    /// Interface para serviço de submissões de formulário com workflow completo
    /// </summary>
    public interface IServicoSubmissaoFormulario
    {
        #region Operações CRUD

        /// <summary>
        /// Obter submissões com filtros e paginação
        /// </summary>
        /// <param name="filtro">Filtros para busca</param>
        /// <param name="usuarioId">ID do usuário que está fazendo a consulta</param>
        /// <param name="roleUsuario">Role do usuário para controle de acesso</param>
        /// <returns>Resultado paginado com as submissões</returns>
        Task<ResultadoPaginadoDto<SubmissaoFormularioDto>> ObterSubmissoesAsync(
            FiltroSubmissaoFormularioDto filtro, 
            int usuarioId, 
            string roleUsuario);

        /// <summary>
        /// Obter detalhes completos de uma submissão
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="usuarioId">ID do usuário que está fazendo a consulta</param>
        /// <param name="roleUsuario">Role do usuário para controle de acesso</param>
        /// <returns>Detalhes da submissão ou null se não encontrada/sem acesso</returns>
        Task<DetalheSubmissaoFormularioDto?> ObterSubmissaoPorIdAsync(
            int id, 
            int usuarioId, 
            string roleUsuario);

        /// <summary>
        /// Criar nova submissão
        /// </summary>
        /// <param name="dto">Dados para criação</param>
        /// <param name="usuarioId">ID do usuário que está criando</param>
        /// <param name="enderecoIp">IP de origem da requisição</param>
        /// <param name="userAgent">User agent da requisição</param>
        /// <returns>Resultado da operação</returns>
        Task<RespostaOperacaoDto> CriarSubmissaoAsync(
            CriarSubmissaoDto dto, 
            int usuarioId, 
            string? enderecoIp, 
            string? userAgent);

        /// <summary>
        /// Atualizar submissão existente (apenas se em rascunho)
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="dto">Dados para atualização</param>
        /// <param name="usuarioId">ID do usuário que está atualizando</param>
        /// <param name="enderecoIp">IP de origem da requisição</param>
        /// <param name="userAgent">User agent da requisição</param>
        /// <returns>Resultado da operação</returns>
        Task<RespostaOperacaoDto> AtualizarSubmissaoAsync(
            int id, 
            AtualizarSubmissaoDto dto, 
            int usuarioId, 
            string? enderecoIp, 
            string? userAgent);

        /// <summary>
        /// Excluir submissão logicamente
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="usuarioId">ID do usuário que está excluindo</param>
        /// <param name="comentario">Motivo da exclusão</param>
        /// <param name="enderecoIp">IP de origem da requisição</param>
        /// <param name="userAgent">User agent da requisição</param>
        /// <returns>Resultado da operação</returns>
        Task<RespostaOperacaoDto> ExcluirSubmissaoAsync(
            int id, 
            int usuarioId, 
            string comentario, 
            string? enderecoIp, 
            string? userAgent);

        #endregion

        #region Workflow de Aprovação

        /// <summary>
        /// Enviar submissão para análise (sair do rascunho)
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="comentario">Comentário opcional</param>
        /// <param name="usuarioId">ID do usuário que está enviando</param>
        /// <param name="enderecoIp">IP de origem da requisição</param>
        /// <param name="userAgent">User agent da requisição</param>
        /// <returns>Resultado da operação</returns>
        Task<RespostaOperacaoDto> EnviarSubmissaoAsync(
            int id, 
            string? comentario, 
            int usuarioId, 
            string? enderecoIp, 
            string? userAgent);

        /// <summary>
        /// Aprovar submissão
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="comentario">Comentário da aprovação</param>
        /// <param name="usuarioAprovadorId">ID do usuário aprovador</param>
        /// <param name="enderecoIp">IP de origem da requisição</param>
        /// <param name="userAgent">User agent da requisição</param>
        /// <returns>Resultado da operação</returns>
        Task<RespostaOperacaoDto> AprovarSubmissaoAsync(
            int id, 
            string comentario, 
            int usuarioAprovadorId, 
            string? enderecoIp, 
            string? userAgent);

        /// <summary>
        /// Rejeitar submissão
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="comentario">Comentário da rejeição</param>
        /// <param name="motivoRejeicao">Motivo da rejeição</param>
        /// <param name="usuarioAprovadorId">ID do usuário que rejeitou</param>
        /// <param name="enderecoIp">IP de origem da requisição</param>
        /// <param name="userAgent">User agent da requisição</param>
        /// <returns>Resultado da operação</returns>
        Task<RespostaOperacaoDto> RejeitarSubmissaoAsync(
            int id, 
            string comentario, 
            string motivoRejeicao, 
            int usuarioAprovadorId, 
            string? enderecoIp, 
            string? userAgent);

        /// <summary>
        /// Cancelar submissão
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="comentario">Motivo do cancelamento</param>
        /// <param name="usuarioId">ID do usuário que está cancelando</param>
        /// <param name="enderecoIp">IP de origem da requisição</param>
        /// <param name="userAgent">User agent da requisição</param>
        /// <returns>Resultado da operação</returns>
        Task<RespostaOperacaoDto> CancelarSubmissaoAsync(
            int id, 
            string comentario, 
            int usuarioId, 
            string? enderecoIp, 
            string? userAgent);

        /// <summary>
        /// Mudar status da submissão (método genérico)
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="dto">Dados da mudança de status</param>
        /// <param name="usuarioId">ID do usuário que está alterando</param>
        /// <param name="enderecoIp">IP de origem da requisição</param>
        /// <param name="userAgent">User agent da requisição</param>
        /// <returns>Resultado da operação</returns>
        Task<RespostaOperacaoDto> MudarStatusSubmissaoAsync(
            int id, 
            MudarStatusSubmissaoDto dto, 
            int usuarioId, 
            string? enderecoIp, 
            string? userAgent);

        #endregion

        #region Histórico e Auditoria

        /// <summary>
        /// Obter histórico completo de uma submissão
        /// </summary>
        /// <param name="submissaoId">ID da submissão</param>
        /// <param name="usuarioId">ID do usuário que está consultando</param>
        /// <param name="roleUsuario">Role do usuário para controle de acesso</param>
        /// <returns>Lista com o histórico</returns>
        Task<List<HistoricoSubmissaoDto>> ObterHistoricoSubmissaoAsync(
            int submissaoId, 
            int usuarioId, 
            string roleUsuario);

        /// <summary>
        /// Criar registro no histórico
        /// </summary>
        /// <param name="submissaoId">ID da submissão</param>
        /// <param name="acao">Ação realizada</param>
        /// <param name="usuarioId">ID do usuário que realizou a ação</param>
        /// <param name="comentario">Comentário da ação</param>
        /// <param name="statusAnterior">Status anterior (opcional)</param>
        /// <param name="novoStatus">Novo status (opcional)</param>
        /// <param name="enderecoIp">IP de origem</param>
        /// <param name="userAgent">User agent</param>
        /// <param name="dadosAlteracao">Dados JSON das alterações (opcional)</param>
        /// <returns>Task</returns>
        Task CriarHistoricoAsync(
            int submissaoId, 
            AcaoSubmissao acao, 
            int usuarioId, 
            string? comentario, 
            StatusSubmissao? statusAnterior, 
            StatusSubmissao? novoStatus, 
            string? enderecoIp, 
            string? userAgent, 
            string? dadosAlteracao = null);

        #endregion

        #region Estatísticas e Relatórios

        /// <summary>
        /// Obter estatísticas gerais de submissões
        /// </summary>
        /// <param name="usuarioId">ID do usuário (opcional, para filtrar por usuário)</param>
        /// <param name="formId">ID do formulário (opcional, para filtrar por formulário)</param>
        /// <param name="roleUsuario">Role do usuário para controle de acesso</param>
        /// <returns>Estatísticas das submissões</returns>
        Task<EstatisticasSubmissaoDto> ObterEstatisticasAsync(
            int? usuarioId = null, 
            int? formId = null, 
            string? roleUsuario = null);

        /// <summary>
        /// Obter submissões que precisam de aprovação
        /// </summary>
        /// <param name="usuarioAprovadorId">ID do usuário aprovador</param>
        /// <param name="pagina">Página para paginação</param>
        /// <param name="tamanhoPagina">Tamanho da página</param>
        /// <returns>Submissões pendentes de aprovação</returns>
        Task<ResultadoPaginadoDto<SubmissaoFormularioDto>> ObterSubmissoesPendenteAprovacaoAsync(
            int usuarioAprovadorId, 
            int pagina = 1, 
            int tamanhoPagina = 20);

        #endregion

        #region Validações e Permissões

        /// <summary>
        /// Verificar se usuário pode visualizar submissão
        /// </summary>
        /// <param name="submissaoId">ID da submissão</param>
        /// <param name="usuarioId">ID do usuário</param>
        /// <param name="roleUsuario">Role do usuário</param>
        /// <returns>True se pode visualizar</returns>
        Task<bool> PodeVisualizarSubmissaoAsync(int submissaoId, int usuarioId, string roleUsuario);

        /// <summary>
        /// Verificar se usuário pode editar submissão
        /// </summary>
        /// <param name="submissaoId">ID da submissão</param>
        /// <param name="usuarioId">ID do usuário</param>
        /// <param name="roleUsuario">Role do usuário</param>
        /// <returns>True se pode editar</returns>
        Task<bool> PodeEditarSubmissaoAsync(int submissaoId, int usuarioId, string roleUsuario);

        /// <summary>
        /// Verificar se usuário pode aprovar submissão
        /// </summary>
        /// <param name="submissaoId">ID da submissão</param>
        /// <param name="usuarioId">ID do usuário</param>
        /// <param name="roleUsuario">Role do usuário</param>
        /// <returns>True se pode aprovar</returns>
        Task<bool> PodeAprovarSubmissaoAsync(int submissaoId, int usuarioId, string roleUsuario);

        /// <summary>
        /// Validar se mudança de status é permitida
        /// </summary>
        /// <param name="statusAtual">Status atual</param>
        /// <param name="novoStatus">Novo status desejado</param>
        /// <param name="roleUsuario">Role do usuário</param>
        /// <returns>True se mudança é válida</returns>
        bool ValidarMudancaStatus(StatusSubmissao statusAtual, StatusSubmissao novoStatus, string roleUsuario);

        #endregion
    }
}