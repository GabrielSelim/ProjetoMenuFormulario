using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FormEngineAPI.Services;
using FormEngineAPI.DTOs;
using FormEngineAPI.Models;

namespace FormEngineAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciamento completo de submissões de formulários
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubmissoesFormularioController : ControllerBase
    {
        private readonly IServicoSubmissaoFormulario _servico;
        private readonly ILogger<SubmissoesFormularioController> _logger;

        public SubmissoesFormularioController(
            IServicoSubmissaoFormulario servico, 
            ILogger<SubmissoesFormularioController> logger)
        {
            _servico = servico;
            _logger = logger;
        }

        #region Operações CRUD

        /// <summary>
        /// Obter submissões com filtros e paginação
        /// </summary>
        /// <param name="filtro">Filtros para busca</param>
        /// <returns>Lista paginada de submissões</returns>
        [HttpGet]
        public async Task<ActionResult<ResultadoPaginadoDto<SubmissaoFormularioDto>>> ObterSubmissoes(
            [FromQuery] FiltroSubmissaoFormularioDto filtro)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var roleUsuario = ObterRoleUsuarioAtual();
                
                var resultado = await _servico.ObterSubmissoesAsync(filtro, usuarioId, roleUsuario);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter submissões para usuário {UsuarioId}", ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obter detalhes de uma submissão específica
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <returns>Detalhes completos da submissão</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DetalheSubmissaoFormularioDto>> ObterSubmissao(int id)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var roleUsuario = ObterRoleUsuarioAtual();
                
                var submissao = await _servico.ObterSubmissaoPorIdAsync(id, usuarioId, roleUsuario);
                
                if (submissao == null)
                {
                    return NotFound(new { message = "Submissão não encontrada ou sem permissão de acesso" });
                }
                
                return Ok(submissao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Criar nova submissão
        /// </summary>
        /// <param name="criarDto">Dados para criação da submissão</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost]
        public async Task<ActionResult<RespostaOperacaoDto>> CriarSubmissao(CriarSubmissaoDto criarDto)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var enderecoIp = ObterEnderecoIp();
                var userAgent = ObterUserAgent();
                
                var resultado = await _servico.CriarSubmissaoAsync(criarDto, usuarioId, enderecoIp, userAgent);
                
                if (resultado.Sucesso)
                {
                    return CreatedAtAction(
                        nameof(ObterSubmissao), 
                        new { id = resultado.Id }, 
                        resultado);
                }
                
                return BadRequest(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar submissão para usuário {UsuarioId}", ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualizar submissão existente
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="atualizarDto">Dados para atualização</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<RespostaOperacaoDto>> AtualizarSubmissao(int id, AtualizarSubmissaoDto atualizarDto)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var enderecoIp = ObterEnderecoIp();
                var userAgent = ObterUserAgent();
                
                var resultado = await _servico.AtualizarSubmissaoAsync(id, atualizarDto, usuarioId, enderecoIp, userAgent);
                
                if (resultado.Sucesso)
                {
                    return Ok(resultado);
                }
                
                return BadRequest(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Excluir submissão logicamente
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="comentario">Motivo da exclusão</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<RespostaOperacaoDto>> ExcluirSubmissao(int id, [FromBody] string? comentario = null)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var enderecoIp = ObterEnderecoIp();
                var userAgent = ObterUserAgent();
                
                var resultado = await _servico.ExcluirSubmissaoAsync(
                    id, usuarioId, comentario ?? "Submissão excluída pelo usuário", enderecoIp, userAgent);
                
                if (resultado.Sucesso)
                {
                    return Ok(resultado);
                }
                
                return BadRequest(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        #endregion

        #region Workflow de Aprovação

        /// <summary>
        /// Enviar submissão para análise
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="comentario">Comentário opcional</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("{id}/enviar")]
        public async Task<ActionResult<RespostaOperacaoDto>> EnviarSubmissao(int id, [FromBody] string? comentario = null)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var enderecoIp = ObterEnderecoIp();
                var userAgent = ObterUserAgent();
                
                var resultado = await _servico.EnviarSubmissaoAsync(id, comentario, usuarioId, enderecoIp, userAgent);
                
                if (resultado.Sucesso)
                {
                    return Ok(resultado);
                }
                
                return BadRequest(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Aprovar submissão
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="comentario">Comentário da aprovação</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("{id}/aprovar")]
        [Authorize(Roles = "admin,gestor")]
        public async Task<ActionResult<RespostaOperacaoDto>> AprovarSubmissao(int id, [FromBody] string comentario)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var enderecoIp = ObterEnderecoIp();
                var userAgent = ObterUserAgent();
                
                var resultado = await _servico.AprovarSubmissaoAsync(id, comentario, usuarioId, enderecoIp, userAgent);
                
                if (resultado.Sucesso)
                {
                    return Ok(resultado);
                }
                
                return BadRequest(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao aprovar submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Rejeitar submissão
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="dto">Dados da rejeição</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("{id}/rejeitar")]
        [Authorize(Roles = "admin,gestor")]
        public async Task<ActionResult<RespostaOperacaoDto>> RejeitarSubmissao(int id, [FromBody] RejeitarSubmissaoDto dto)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var enderecoIp = ObterEnderecoIp();
                var userAgent = ObterUserAgent();
                
                var resultado = await _servico.RejeitarSubmissaoAsync(
                    id, dto.Comentario, dto.MotivoRejeicao, usuarioId, enderecoIp, userAgent);
                
                if (resultado.Sucesso)
                {
                    return Ok(resultado);
                }
                
                return BadRequest(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao rejeitar submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Cancelar submissão
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="comentario">Motivo do cancelamento</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("{id}/cancelar")]
        public async Task<ActionResult<RespostaOperacaoDto>> CancelarSubmissao(int id, [FromBody] string comentario)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var enderecoIp = ObterEnderecoIp();
                var userAgent = ObterUserAgent();
                
                var resultado = await _servico.CancelarSubmissaoAsync(id, comentario, usuarioId, enderecoIp, userAgent);
                
                if (resultado.Sucesso)
                {
                    return Ok(resultado);
                }
                
                return BadRequest(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Mudar status da submissão (método genérico)
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <param name="dto">Dados da mudança de status</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}/status")]
        public async Task<ActionResult<RespostaOperacaoDto>> MudarStatusSubmissao(int id, MudarStatusSubmissaoDto dto)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var enderecoIp = ObterEnderecoIp();
                var userAgent = ObterUserAgent();
                
                var resultado = await _servico.MudarStatusSubmissaoAsync(id, dto, usuarioId, enderecoIp, userAgent);
                
                if (resultado.Sucesso)
                {
                    return Ok(resultado);
                }
                
                return BadRequest(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao mudar status da submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        #endregion

        #region Histórico e Auditoria

        /// <summary>
        /// Obter histórico completo de uma submissão
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <returns>Histórico da submissão</returns>
        [HttpGet("{id}/historico")]
        public async Task<ActionResult<List<HistoricoSubmissaoDto>>> ObterHistoricoSubmissao(int id)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var roleUsuario = ObterRoleUsuarioAtual();
                
                var historico = await _servico.ObterHistoricoSubmissaoAsync(id, usuarioId, roleUsuario);
                return Ok(historico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter histórico da submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        #endregion

        #region Estatísticas e Relatórios

        /// <summary>
        /// Obter estatísticas gerais de submissões
        /// </summary>
        /// <param name="usuarioId">ID do usuário (opcional)</param>
        /// <param name="formId">ID do formulário (opcional)</param>
        /// <returns>Estatísticas das submissões</returns>
        [HttpGet("estatisticas")]
        public async Task<ActionResult<EstatisticasSubmissaoDto>> ObterEstatisticas(
            [FromQuery] int? usuarioId = null,
            [FromQuery] int? formId = null)
        {
            try
            {
                var roleUsuario = ObterRoleUsuarioAtual();
                var usuarioAtual = ObterUsuarioAtualId();
                
                // Se não for admin, só pode ver estatísticas próprias
                if (roleUsuario.ToLower() != "admin" && usuarioId.HasValue && usuarioId != usuarioAtual)
                {
                    return Forbid("Sem permissão para ver estatísticas de outros usuários");
                }
                
                var estatisticas = await _servico.ObterEstatisticasAsync(usuarioId, formId, roleUsuario);
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas para usuário {UsuarioId}", ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obter submissões pendentes de aprovação
        /// </summary>
        /// <param name="pagina">Página</param>
        /// <param name="tamanhoPagina">Tamanho da página</param>
        /// <returns>Submissões pendentes</returns>
        [HttpGet("pendentes-aprovacao")]
        [Authorize(Roles = "admin,gestor")]
        public async Task<ActionResult<ResultadoPaginadoDto<SubmissaoFormularioDto>>> ObterSubmissoesPendenteAprovacao(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 20)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var resultado = await _servico.ObterSubmissoesPendenteAprovacaoAsync(usuarioId, pagina, tamanhoPagina);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter submissões pendentes para usuário {UsuarioId}", ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        #endregion

        #region Validações e Consultas

        /// <summary>
        /// Verificar se usuário pode visualizar submissão
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <returns>True se pode visualizar</returns>
        [HttpGet("{id}/pode-visualizar")]
        public async Task<ActionResult<bool>> PodeVisualizarSubmissao(int id)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var roleUsuario = ObterRoleUsuarioAtual();
                
                var pode = await _servico.PodeVisualizarSubmissaoAsync(id, usuarioId, roleUsuario);
                return Ok(pode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar permissão de visualização da submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Verificar se usuário pode editar submissão
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <returns>True se pode editar</returns>
        [HttpGet("{id}/pode-editar")]
        public async Task<ActionResult<bool>> PodeEditarSubmissao(int id)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var roleUsuario = ObterRoleUsuarioAtual();
                
                var pode = await _servico.PodeEditarSubmissaoAsync(id, usuarioId, roleUsuario);
                return Ok(pode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar permissão de edição da submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Verificar se usuário pode aprovar submissão
        /// </summary>
        /// <param name="id">ID da submissão</param>
        /// <returns>True se pode aprovar</returns>
        [HttpGet("{id}/pode-aprovar")]
        public async Task<ActionResult<bool>> PodeAprovarSubmissao(int id)
        {
            try
            {
                var usuarioId = ObterUsuarioAtualId();
                var roleUsuario = ObterRoleUsuarioAtual();
                
                var pode = await _servico.PodeAprovarSubmissaoAsync(id, usuarioId, roleUsuario);
                return Ok(pode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar permissão de aprovação da submissão {SubmissaoId} para usuário {UsuarioId}", id, ObterUsuarioAtualId());
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Obter ID do usuário atual a partir do token JWT
        /// </summary>
        /// <returns>ID do usuário</returns>
        private int ObterUsuarioAtualId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                           ?? User.FindFirst("sub") 
                           ?? User.FindFirst("id");
            
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            
            throw new UnauthorizedAccessException("Token JWT inválido - ID do usuário não encontrado");
        }

        /// <summary>
        /// Obter role do usuário atual a partir do token JWT
        /// </summary>
        /// <returns>Role do usuário</returns>
        private string ObterRoleUsuarioAtual()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role) 
                         ?? User.FindFirst("role");
            
            return roleClaim?.Value ?? "user";
        }

        /// <summary>
        /// Obter endereço IP da requisição
        /// </summary>
        /// <returns>Endereço IP</returns>
        private string? ObterEnderecoIp()
        {
            // Verificar se há proxy reverso (X-Forwarded-For)
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            // Verificar X-Real-IP
            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // IP direto da conexão
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        /// <summary>
        /// Obter User Agent da requisição
        /// </summary>
        /// <returns>User Agent</returns>
        private string? ObterUserAgent()
        {
            return Request.Headers["User-Agent"].FirstOrDefault();
        }

        #endregion
    }

    /// <summary>
    /// DTO para rejeição de submissão
    /// </summary>
    public class RejeitarSubmissaoDto
    {
        /// <summary>
        /// Comentário da rejeição
        /// </summary>
        public string Comentario { get; set; } = string.Empty;

        /// <summary>
        /// Motivo da rejeição
        /// </summary>
        public string MotivoRejeicao { get; set; } = string.Empty;
    }
}