using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FormEngineAPI.Data;
using FormEngineAPI.DTOs;
using FormEngineAPI.Models;
using System.Text.Json;

namespace FormEngineAPI.Services
{
    /// <summary>
    /// Implementação do serviço de submissões de formulário com workflow completo
    /// </summary>
    public class ServicoSubmissaoFormulario : IServicoSubmissaoFormulario
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ServicoSubmissaoFormulario(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Operações CRUD

        public async Task<ResultadoPaginadoDto<SubmissaoFormularioDto>> ObterSubmissoesAsync(
            FiltroSubmissaoFormularioDto filtro, 
            int usuarioId, 
            string roleUsuario)
        {
            var query = _context.FormSubmissions
                .Include(fs => fs.Form)
                .Include(fs => fs.User)
                .Include(fs => fs.UsuarioAprovador)
                .AsQueryable();

            // Filtros de segurança baseados na role
            if (roleUsuario.ToLower() != "admin")
            {
                if (roleUsuario.ToLower() == "user")
                {
                    // Usuários comuns só veem suas próprias submissões
                    query = query.Where(fs => fs.UserId == usuarioId);
                }
                else if (roleUsuario.ToLower() == "gestor")
                {
                    // Gestores veem suas próprias e as que podem aprovar
                    query = query.Where(fs => fs.UserId == usuarioId || 
                                            fs.Status == StatusSubmissao.Enviado ||
                                            fs.Status == StatusSubmissao.EmAnalise);
                }
            }

            // Aplicar filtros
            if (filtro.FormId.HasValue)
                query = query.Where(fs => fs.FormId == filtro.FormId);

            if (filtro.UserId.HasValue && roleUsuario.ToLower() == "admin")
                query = query.Where(fs => fs.UserId == filtro.UserId);

            if (filtro.Status.HasValue)
                query = query.Where(fs => fs.Status == filtro.Status);

            if (filtro.DataInicialCriacao.HasValue)
                query = query.Where(fs => fs.CreatedAt >= filtro.DataInicialCriacao);

            if (filtro.DataFinalCriacao.HasValue)
                query = query.Where(fs => fs.CreatedAt <= filtro.DataFinalCriacao);

            if (filtro.DataInicialSubmissao.HasValue)
                query = query.Where(fs => fs.DataSubmissao >= filtro.DataInicialSubmissao);

            if (filtro.DataFinalSubmissao.HasValue)
                query = query.Where(fs => fs.DataSubmissao <= filtro.DataFinalSubmissao);

            if (filtro.UsuarioAprovadorId.HasValue)
                query = query.Where(fs => fs.UsuarioAprovadorId == filtro.UsuarioAprovadorId);

            // Filtro de exclusão lógica
            if (!filtro.IncluirExcluidas)
                query = query.Where(fs => !fs.Excluido);

            // Contagem total
            var totalItens = await query.CountAsync();

            // Ordenação
            query = AplicarOrdenacao(query, filtro.CampoOrdenacao, filtro.DirecaoOrdenacao);

            // Paginação
            var itens = await query
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .Select(fs => new SubmissaoFormularioDto
                {
                    Id = fs.Id,
                    FormId = fs.FormId,
                    FormName = fs.Form.Name,
                    UserId = fs.UserId,
                    UserName = fs.User.Name,
                    UserEmail = fs.User.Email,
                    Status = fs.Status,
                    CreatedAt = fs.CreatedAt,
                    DataAtualizacao = fs.DataAtualizacao,
                    DataSubmissao = fs.DataSubmissao,
                    DataAprovacao = fs.DataAprovacao,
                    UsuarioAprovadorId = fs.UsuarioAprovadorId,
                    UsuarioAprovadorName = fs.UsuarioAprovador != null ? fs.UsuarioAprovador.Name : null,
                    Versao = fs.Versao,
                    Excluido = fs.Excluido
                })
                .ToListAsync();

            var totalPaginas = (int)Math.Ceiling((double)totalItens / filtro.TamanhoPagina);

            return new ResultadoPaginadoDto<SubmissaoFormularioDto>
            {
                Itens = itens,
                TotalItens = totalItens,
                PaginaAtual = filtro.Pagina,
                TotalPaginas = totalPaginas,
                TamanhoPagina = filtro.TamanhoPagina
            };
        }

        public async Task<DetalheSubmissaoFormularioDto?> ObterSubmissaoPorIdAsync(
            int id, 
            int usuarioId, 
            string roleUsuario)
        {
            if (!await PodeVisualizarSubmissaoAsync(id, usuarioId, roleUsuario))
                return null;

            var submissao = await _context.FormSubmissions
                .Include(fs => fs.Form)
                .Include(fs => fs.User)
                .Include(fs => fs.UsuarioAprovador)
                .Include(fs => fs.Historicos)
                    .ThenInclude(h => h.Usuario)
                .FirstOrDefaultAsync(fs => fs.Id == id);

            if (submissao == null)
                return null;

            var historicos = submissao.Historicos
                .OrderByDescending(h => h.DataAcao)
                .Select(h => new HistoricoSubmissaoDto
                {
                    Id = h.Id,
                    Acao = h.Acao,
                    DataAcao = h.DataAcao,
                    UsuarioId = h.UsuarioId,
                    UsuarioName = h.Usuario.Name,
                    UsuarioEmail = h.Usuario.Email,
                    Comentario = h.Comentario,
                    StatusAnterior = h.StatusAnterior,
                    NovoStatus = h.NovoStatus,
                    EnderecoIp = h.EnderecoIp
                })
                .ToList();

            return new DetalheSubmissaoFormularioDto
            {
                Id = submissao.Id,
                FormId = submissao.FormId,
                FormName = submissao.Form.Name,
                UserId = submissao.UserId,
                UserName = submissao.User.Name,
                UserEmail = submissao.User.Email,
                Status = submissao.Status,
                CreatedAt = submissao.CreatedAt,
                DataAtualizacao = submissao.DataAtualizacao,
                DataSubmissao = submissao.DataSubmissao,
                DataAprovacao = submissao.DataAprovacao,
                UsuarioAprovadorId = submissao.UsuarioAprovadorId,
                UsuarioAprovadorName = submissao.UsuarioAprovador?.Name,
                Versao = submissao.Versao,
                Excluido = submissao.Excluido,
                DataJson = submissao.DataJson,
                MotivoRejeicao = submissao.MotivoRejeicao,
                EnderecoIp = submissao.EnderecoIp,
                UserAgent = submissao.UserAgent,
                DataExclusao = submissao.DataExclusao,
                Historicos = historicos
            };
        }

        public async Task<RespostaOperacaoDto> CriarSubmissaoAsync(
            CriarSubmissaoDto dto, 
            int usuarioId, 
            string? enderecoIp, 
            string? userAgent)
        {
            try
            {
                // Verificar se o formulário existe
                var form = await _context.Forms.FindAsync(dto.FormId);
                if (form == null)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Formulário não encontrado",
                        Erros = new List<string> { "O formulário especificado não existe" }
                    };
                }

                // Verificar se o usuário tem permissão para usar o formulário
                if (!string.IsNullOrEmpty(form.RolesAllowed))
                {
                    var user = await _context.Users.FindAsync(usuarioId);
                    if (user != null)
                    {
                        // Comparação case-insensitive de roles
                        var rolesPermitidas = form.RolesAllowed.Split(',').Select(r => r.Trim().ToLower()).ToList();
                        var userRole = user.Role.ToLower();
                        
                        if (!rolesPermitidas.Contains(userRole))
                        {
                            return new RespostaOperacaoDto
                            {
                                Sucesso = false,
                                Mensagem = "Sem permissão para usar este formulário",
                                Erros = new List<string> { "Usuário não tem permissão para usar este formulário" }
                            };
                        }
                    }
                }

                var agora = DateTime.UtcNow;
                var submissao = new FormSubmission
                {
                    FormId = dto.FormId,
                    UserId = usuarioId,
                    DataJson = dto.DataJson,
                    Status = dto.Status,
                    CreatedAt = agora,
                    DataAtualizacao = agora,
                    EnderecoIp = enderecoIp,
                    UserAgent = userAgent,
                    Versao = 1,
                    Excluido = false
                };

                // Se não for rascunho, definir data de submissão
                if (dto.Status != StatusSubmissao.Rascunho)
                {
                    submissao.DataSubmissao = agora;
                }

                _context.FormSubmissions.Add(submissao);
                await _context.SaveChangesAsync();

                // Criar histórico
                await CriarHistoricoAsync(
                    submissao.Id,
                    AcaoSubmissao.Criado,
                    usuarioId,
                    dto.ComentarioInicial,
                    null,
                    dto.Status,
                    enderecoIp,
                    userAgent
                );

                return new RespostaOperacaoDto
                {
                    Sucesso = true,
                    Mensagem = "Submissão criada com sucesso",
                    Id = submissao.Id
                };
            }
            catch (Exception ex)
            {
                return new RespostaOperacaoDto
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<RespostaOperacaoDto> AtualizarSubmissaoAsync(
            int id, 
            AtualizarSubmissaoDto dto, 
            int usuarioId, 
            string? enderecoIp, 
            string? userAgent)
        {
            try
            {
                if (!await PodeEditarSubmissaoAsync(id, usuarioId, "user"))
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Sem permissão para editar esta submissão",
                        Erros = new List<string> { "Usuário não tem permissão para editar esta submissão" }
                    };
                }

                var submissao = await _context.FormSubmissions.FindAsync(id);
                if (submissao == null)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Submissão não encontrada"
                    };
                }

                // Controle de concorrência
                if (submissao.Versao != dto.Versao)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Conflito de versão",
                        Erros = new List<string> { "A submissão foi modificada por outro usuário" }
                    };
                }

                // Verificar se pode ser editada (apenas rascunhos)
                if (submissao.Status != StatusSubmissao.Rascunho)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Submissão não pode ser editada",
                        Erros = new List<string> { "Apenas submissões em rascunho podem ser editadas" }
                    };
                }

                var dadosAntigos = JsonSerializer.Serialize(new { submissao.DataJson });
                
                submissao.DataJson = dto.DataJson;
                submissao.DataAtualizacao = DateTime.UtcNow;
                submissao.Versao++;
                submissao.EnderecoIp = enderecoIp;
                submissao.UserAgent = userAgent;

                await _context.SaveChangesAsync();

                // Criar histórico
                await CriarHistoricoAsync(
                    id,
                    AcaoSubmissao.Atualizado,
                    usuarioId,
                    dto.Comentario,
                    null,
                    null,
                    enderecoIp,
                    userAgent,
                    dadosAntigos
                );

                return new RespostaOperacaoDto
                {
                    Sucesso = true,
                    Mensagem = "Submissão atualizada com sucesso",
                    Id = id
                };
            }
            catch (Exception ex)
            {
                return new RespostaOperacaoDto
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<RespostaOperacaoDto> ExcluirSubmissaoAsync(
            int id, 
            int usuarioId, 
            string comentario, 
            string? enderecoIp, 
            string? userAgent)
        {
            try
            {
                var submissao = await _context.FormSubmissions.FindAsync(id);
                if (submissao == null)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Submissão não encontrada"
                    };
                }

                // Verificar permissões
                var user = await _context.Users.FindAsync(usuarioId);
                if (user == null || (submissao.UserId != usuarioId && user.Role.ToLower() != "admin"))
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Sem permissão para excluir esta submissão"
                    };
                }

                // Exclusão lógica
                submissao.Excluido = true;
                submissao.DataExclusao = DateTime.UtcNow;
                submissao.DataAtualizacao = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Criar histórico
                await CriarHistoricoAsync(
                    id,
                    AcaoSubmissao.Excluido,
                    usuarioId,
                    comentario,
                    submissao.Status,
                    submissao.Status,
                    enderecoIp,
                    userAgent
                );

                return new RespostaOperacaoDto
                {
                    Sucesso = true,
                    Mensagem = "Submissão excluída com sucesso"
                };
            }
            catch (Exception ex)
            {
                return new RespostaOperacaoDto
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        #endregion

        #region Workflow de Aprovação

        public async Task<RespostaOperacaoDto> EnviarSubmissaoAsync(
            int id, 
            string? comentario, 
            int usuarioId, 
            string? enderecoIp, 
            string? userAgent)
        {
            return await MudarStatusSubmissaoAsync(
                id,
                new MudarStatusSubmissaoDto
                {
                    NovoStatus = StatusSubmissao.Enviado,
                    Comentario = comentario ?? "Submissão enviada para análise",
                    Versao = await ObterVersaoAtualAsync(id)
                },
                usuarioId,
                enderecoIp,
                userAgent
            );
        }

        public async Task<RespostaOperacaoDto> AprovarSubmissaoAsync(
            int id, 
            string comentario, 
            int usuarioAprovadorId, 
            string? enderecoIp, 
            string? userAgent)
        {
            try
            {
                if (!await PodeAprovarSubmissaoAsync(id, usuarioAprovadorId, "manager"))
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Sem permissão para aprovar esta submissão"
                    };
                }

                var submissao = await _context.FormSubmissions.FindAsync(id);
                if (submissao == null)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Submissão não encontrada"
                    };
                }

                if (submissao.Status != StatusSubmissao.Enviado && submissao.Status != StatusSubmissao.EmAnalise)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Submissão não está em estado para aprovação"
                    };
                }

                var statusAnterior = submissao.Status;
                var agora = DateTime.UtcNow;

                submissao.Status = StatusSubmissao.Aprovado;
                submissao.DataAprovacao = agora;
                submissao.UsuarioAprovadorId = usuarioAprovadorId;
                submissao.DataAtualizacao = agora;

                await _context.SaveChangesAsync();

                // Criar histórico
                await CriarHistoricoAsync(
                    id,
                    AcaoSubmissao.Aprovado,
                    usuarioAprovadorId,
                    comentario,
                    statusAnterior,
                    StatusSubmissao.Aprovado,
                    enderecoIp,
                    userAgent
                );

                return new RespostaOperacaoDto
                {
                    Sucesso = true,
                    Mensagem = "Submissão aprovada com sucesso"
                };
            }
            catch (Exception ex)
            {
                return new RespostaOperacaoDto
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<RespostaOperacaoDto> RejeitarSubmissaoAsync(
            int id, 
            string comentario, 
            string motivoRejeicao, 
            int usuarioAprovadorId, 
            string? enderecoIp, 
            string? userAgent)
        {
            try
            {
                if (!await PodeAprovarSubmissaoAsync(id, usuarioAprovadorId, "manager"))
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Sem permissão para rejeitar esta submissão"
                    };
                }

                var submissao = await _context.FormSubmissions.FindAsync(id);
                if (submissao == null)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Submissão não encontrada"
                    };
                }

                if (submissao.Status != StatusSubmissao.Enviado && submissao.Status != StatusSubmissao.EmAnalise)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Submissão não está em estado para rejeição"
                    };
                }

                var statusAnterior = submissao.Status;

                submissao.Status = StatusSubmissao.Rejeitado;
                submissao.MotivoRejeicao = motivoRejeicao;
                submissao.UsuarioAprovadorId = usuarioAprovadorId;
                submissao.DataAtualizacao = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Criar histórico
                await CriarHistoricoAsync(
                    id,
                    AcaoSubmissao.Rejeitado,
                    usuarioAprovadorId,
                    comentario,
                    statusAnterior,
                    StatusSubmissao.Rejeitado,
                    enderecoIp,
                    userAgent
                );

                return new RespostaOperacaoDto
                {
                    Sucesso = true,
                    Mensagem = "Submissão rejeitada com sucesso"
                };
            }
            catch (Exception ex)
            {
                return new RespostaOperacaoDto
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<RespostaOperacaoDto> CancelarSubmissaoAsync(
            int id, 
            string comentario, 
            int usuarioId, 
            string? enderecoIp, 
            string? userAgent)
        {
            return await MudarStatusSubmissaoAsync(
                id,
                new MudarStatusSubmissaoDto
                {
                    NovoStatus = StatusSubmissao.Cancelado,
                    Comentario = comentario,
                    Versao = await ObterVersaoAtualAsync(id)
                },
                usuarioId,
                enderecoIp,
                userAgent
            );
        }

        public async Task<RespostaOperacaoDto> MudarStatusSubmissaoAsync(
            int id, 
            MudarStatusSubmissaoDto dto, 
            int usuarioId, 
            string? enderecoIp, 
            string? userAgent)
        {
            try
            {
                var submissao = await _context.FormSubmissions.FindAsync(id);
                if (submissao == null)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Submissão não encontrada"
                    };
                }

                var user = await _context.Users.FindAsync(usuarioId);
                if (user == null)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Usuário não encontrado"
                    };
                }

                // Validar mudança de status
                if (!ValidarMudancaStatus(submissao.Status, dto.NovoStatus, user.Role))
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Mudança de status não permitida",
                        Erros = new List<string> { $"Não é possível mudar de {submissao.Status} para {dto.NovoStatus}" }
                    };
                }

                // Controle de concorrência
                if (submissao.Versao != dto.Versao)
                {
                    return new RespostaOperacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Conflito de versão",
                        Erros = new List<string> { "A submissão foi modificada por outro usuário" }
                    };
                }

                var statusAnterior = submissao.Status;
                submissao.Status = dto.NovoStatus;
                submissao.DataAtualizacao = DateTime.UtcNow;

                // Definir campos específicos baseados no status
                switch (dto.NovoStatus)
                {
                    case StatusSubmissao.Enviado:
                        submissao.DataSubmissao = DateTime.UtcNow;
                        break;
                    case StatusSubmissao.Aprovado:
                        submissao.DataAprovacao = DateTime.UtcNow;
                        submissao.UsuarioAprovadorId = usuarioId;
                        break;
                    case StatusSubmissao.Rejeitado:
                        submissao.MotivoRejeicao = dto.MotivoRejeicao;
                        submissao.UsuarioAprovadorId = usuarioId;
                        break;
                }

                await _context.SaveChangesAsync();

                // Mapear status para ação
                var acao = dto.NovoStatus switch
                {
                    StatusSubmissao.Enviado => AcaoSubmissao.Enviado,
                    StatusSubmissao.Aprovado => AcaoSubmissao.Aprovado,
                    StatusSubmissao.Rejeitado => AcaoSubmissao.Rejeitado,
                    StatusSubmissao.Cancelado => AcaoSubmissao.Cancelado,
                    _ => AcaoSubmissao.Atualizado
                };

                // Criar histórico
                await CriarHistoricoAsync(
                    id,
                    acao,
                    usuarioId,
                    dto.Comentario,
                    statusAnterior,
                    dto.NovoStatus,
                    enderecoIp,
                    userAgent
                );

                return new RespostaOperacaoDto
                {
                    Sucesso = true,
                    Mensagem = $"Status alterado para {dto.NovoStatus} com sucesso"
                };
            }
            catch (Exception ex)
            {
                return new RespostaOperacaoDto
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        #endregion

        #region Histórico e Auditoria

        public async Task<List<HistoricoSubmissaoDto>> ObterHistoricoSubmissaoAsync(
            int submissaoId, 
            int usuarioId, 
            string roleUsuario)
        {
            if (!await PodeVisualizarSubmissaoAsync(submissaoId, usuarioId, roleUsuario))
                return new List<HistoricoSubmissaoDto>();

            return await _context.HistoricoFormSubmissions
                .Include(h => h.Usuario)
                .Where(h => h.FormSubmissionId == submissaoId)
                .OrderByDescending(h => h.DataAcao)
                .Select(h => new HistoricoSubmissaoDto
                {
                    Id = h.Id,
                    Acao = h.Acao,
                    DataAcao = h.DataAcao,
                    UsuarioId = h.UsuarioId,
                    UsuarioName = h.Usuario.Name,
                    UsuarioEmail = h.Usuario.Email,
                    Comentario = h.Comentario,
                    StatusAnterior = h.StatusAnterior,
                    NovoStatus = h.NovoStatus,
                    EnderecoIp = h.EnderecoIp
                })
                .ToListAsync();
        }

        public async Task CriarHistoricoAsync(
            int submissaoId, 
            AcaoSubmissao acao, 
            int usuarioId, 
            string? comentario, 
            StatusSubmissao? statusAnterior, 
            StatusSubmissao? novoStatus, 
            string? enderecoIp, 
            string? userAgent, 
            string? dadosAlteracao = null)
        {
            var historico = new HistoricoFormSubmission
            {
                FormSubmissionId = submissaoId,
                Acao = acao,
                DataAcao = DateTime.UtcNow,
                UsuarioId = usuarioId,
                Comentario = comentario,
                StatusAnterior = statusAnterior,
                NovoStatus = novoStatus,
                EnderecoIp = enderecoIp,
                UserAgent = userAgent,
                DadosAlteracao = dadosAlteracao
            };

            _context.HistoricoFormSubmissions.Add(historico);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Estatísticas e Relatórios

        public async Task<EstatisticasSubmissaoDto> ObterEstatisticasAsync(
            int? usuarioId = null, 
            int? formId = null, 
            string? roleUsuario = null)
        {
            var query = _context.FormSubmissions.Where(fs => !fs.Excluido);

            if (usuarioId.HasValue)
                query = query.Where(fs => fs.UserId == usuarioId);

            if (formId.HasValue)
                query = query.Where(fs => fs.FormId == formId);

            var hoje = DateTime.UtcNow.Date;
            var inicioSemana = hoje.AddDays(-(int)hoje.DayOfWeek);
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

            var estatisticas = new EstatisticasSubmissaoDto
            {
                TotalGeral = await query.CountAsync(),
                CriadasHoje = await query.CountAsync(fs => fs.CreatedAt.Date == hoje),
                CriadasEstaSemana = await query.CountAsync(fs => fs.CreatedAt.Date >= inicioSemana),
                CriadasEsteMes = await query.CountAsync(fs => fs.CreatedAt.Date >= inicioMes),
                PendentesAprovacao = await query.CountAsync(fs => 
                    fs.Status == StatusSubmissao.Enviado || fs.Status == StatusSubmissao.EmAnalise)
            };

            // Total por status
            var statusCounts = await query
                .GroupBy(fs => fs.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var item in statusCounts)
            {
                estatisticas.TotalPorStatus[item.Status] = item.Count;
            }

            // Tempo médio de aprovação
            var aprovadas = await query
                .Where(fs => fs.Status == StatusSubmissao.Aprovado && 
                            fs.DataSubmissao.HasValue && 
                            fs.DataAprovacao.HasValue)
                .Select(fs => new { 
                    DataSubmissao = fs.DataSubmissao!.Value, 
                    DataAprovacao = fs.DataAprovacao!.Value 
                })
                .ToListAsync();

            if (aprovadas.Any())
            {
                var temposMedio = aprovadas
                    .Select(a => (a.DataAprovacao - a.DataSubmissao).TotalHours)
                    .Average();
                estatisticas.TempoMedioAprovacao = Math.Round(temposMedio, 2);
            }

            return estatisticas;
        }

        public async Task<ResultadoPaginadoDto<SubmissaoFormularioDto>> ObterSubmissoesPendenteAprovacaoAsync(
            int usuarioAprovadorId, 
            int pagina = 1, 
            int tamanhoPagina = 20)
        {
            var query = _context.FormSubmissions
                .Include(fs => fs.Form)
                .Include(fs => fs.User)
                .Where(fs => !fs.Excluido && 
                            (fs.Status == StatusSubmissao.Enviado || fs.Status == StatusSubmissao.EmAnalise));

            var totalItens = await query.CountAsync();

            var itens = await query
                .OrderBy(fs => fs.DataSubmissao)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .Select(fs => new SubmissaoFormularioDto
                {
                    Id = fs.Id,
                    FormId = fs.FormId,
                    FormName = fs.Form.Name,
                    UserId = fs.UserId,
                    UserName = fs.User.Name,
                    UserEmail = fs.User.Email,
                    Status = fs.Status,
                    CreatedAt = fs.CreatedAt,
                    DataAtualizacao = fs.DataAtualizacao,
                    DataSubmissao = fs.DataSubmissao,
                    DataAprovacao = fs.DataAprovacao,
                    UsuarioAprovadorId = fs.UsuarioAprovadorId,
                    Versao = fs.Versao,
                    Excluido = fs.Excluido
                })
                .ToListAsync();

            var totalPaginas = (int)Math.Ceiling((double)totalItens / tamanhoPagina);

            return new ResultadoPaginadoDto<SubmissaoFormularioDto>
            {
                Itens = itens,
                TotalItens = totalItens,
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas,
                TamanhoPagina = tamanhoPagina
            };
        }

        #endregion

        #region Validações e Permissões

        public async Task<bool> PodeVisualizarSubmissaoAsync(int submissaoId, int usuarioId, string roleUsuario)
        {
            var submissao = await _context.FormSubmissions.FindAsync(submissaoId);
            if (submissao == null) return false;

            return roleUsuario.ToLower() switch
            {
                "admin" => true,
                "manager" => true,
                "user" => submissao.UserId == usuarioId,
                _ => false
            };
        }

        public async Task<bool> PodeEditarSubmissaoAsync(int submissaoId, int usuarioId, string roleUsuario)
        {
            var submissao = await _context.FormSubmissions.FindAsync(submissaoId);
            if (submissao == null) return false;

            // Apenas o próprio usuário pode editar suas submissões (e apenas se estiver em rascunho)
            return submissao.UserId == usuarioId && submissao.Status == StatusSubmissao.Rascunho;
        }

        public async Task<bool> PodeAprovarSubmissaoAsync(int submissaoId, int usuarioId, string roleUsuario)
        {
            if (roleUsuario.ToLower() != "admin" && roleUsuario.ToLower() != "manager")
                return false;

            var submissao = await _context.FormSubmissions.FindAsync(submissaoId);
            if (submissao == null) return false;

            // Não pode aprovar própria submissão
            if (submissao.UserId == usuarioId) return false;

            // Deve estar em status que permite aprovação
            return submissao.Status == StatusSubmissao.Enviado || submissao.Status == StatusSubmissao.EmAnalise;
        }

        public bool ValidarMudancaStatus(StatusSubmissao statusAtual, StatusSubmissao novoStatus, string roleUsuario)
        {
            // Regras de transição de status
            return statusAtual switch
            {
                StatusSubmissao.Rascunho => novoStatus == StatusSubmissao.Enviado || novoStatus == StatusSubmissao.Cancelado,
                StatusSubmissao.Enviado => novoStatus == StatusSubmissao.EmAnalise || 
                                          novoStatus == StatusSubmissao.Aprovado || 
                                          novoStatus == StatusSubmissao.Rejeitado ||
                                          novoStatus == StatusSubmissao.Cancelado,
                StatusSubmissao.EmAnalise => novoStatus == StatusSubmissao.Aprovado || 
                                            novoStatus == StatusSubmissao.Rejeitado ||
                                            novoStatus == StatusSubmissao.Cancelado,
                StatusSubmissao.Aprovado => roleUsuario.ToLower() == "admin", // Apenas admin pode alterar aprovados
                StatusSubmissao.Rejeitado => novoStatus == StatusSubmissao.Rascunho || // Pode voltar para edição
                                            (roleUsuario.ToLower() == "admin"), // Admin pode alterar qualquer coisa
                StatusSubmissao.Cancelado => roleUsuario.ToLower() == "admin", // Apenas admin pode alterar cancelados
                _ => false
            };
        }

        #endregion

        #region Métodos Auxiliares

        private IQueryable<FormSubmission> AplicarOrdenacao(
            IQueryable<FormSubmission> query, 
            string? campoOrdenacao, 
            string direcaoOrdenacao)
        {
            var ascendente = direcaoOrdenacao.ToLower() == "asc";

            return campoOrdenacao?.ToLower() switch
            {
                "id" => ascendente ? query.OrderBy(fs => fs.Id) : query.OrderByDescending(fs => fs.Id),
                "formid" => ascendente ? query.OrderBy(fs => fs.FormId) : query.OrderByDescending(fs => fs.FormId),
                "userid" => ascendente ? query.OrderBy(fs => fs.UserId) : query.OrderByDescending(fs => fs.UserId),
                "status" => ascendente ? query.OrderBy(fs => fs.Status) : query.OrderByDescending(fs => fs.Status),
                "datasubmissao" => ascendente ? query.OrderBy(fs => fs.DataSubmissao) : query.OrderByDescending(fs => fs.DataSubmissao),
                "dataaprovacao" => ascendente ? query.OrderBy(fs => fs.DataAprovacao) : query.OrderByDescending(fs => fs.DataAprovacao),
                "createdat" or _ => ascendente ? query.OrderBy(fs => fs.CreatedAt) : query.OrderByDescending(fs => fs.CreatedAt)
            };
        }

        private async Task<int> ObterVersaoAtualAsync(int submissaoId)
        {
            var submissao = await _context.FormSubmissions.FindAsync(submissaoId);
            return submissao?.Versao ?? 1;
        }

        #endregion
    }
}