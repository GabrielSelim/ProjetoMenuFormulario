# 🔧 IMPLEMENTAÇÃO BACKEND - SISTEMA DE SUBMISSIONS

## 🎯 OBJETIVO
Expandir o sistema .NET existente para gerenciar submissions de formulários com workflow completo de aprovação, histórico e relatórios.

## 📊 ESTRUTURA ATUAL (Não mexer)
```csharp
public class Form
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SchemaJson { get; set; } = string.Empty;  // FormEngine.io
    public string RolesAllowed { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<FormSubmission> FormSubmissions { get; set; } = new List<FormSubmission>();
}

public class FormSubmission  
{
    public int Id { get; set; }
    public int FormId { get; set; }
    public int UserId { get; set; }
    public string DataJson { get; set; } = string.Empty;  // Dados preenchidos
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Form Form { get; set; } = null!;
    public User User { get; set; } = null!;
}
```

## 🚀 EXPANSÃO NECESSÁRIA

### **1. ATUALIZAR FormSubmission (Migration Aditiva)**
```csharp
public class FormSubmission
{
    // CAMPOS EXISTENTES (não mexer)
    public int Id { get; set; }
    public int FormId { get; set; }
    public int UserId { get; set; }
    public string DataJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // NOVOS CAMPOS (adicionar via migration)
    public StatusSubmissao Status { get; set; } = StatusSubmissao.Rascunho;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataSubmissao { get; set; }
    public DateTime? DataAprovacao { get; set; }
    public int? UsuarioAprovadorId { get; set; }
    public string? MotivoRejeicao { get; set; }
    public string? EnderecoIp { get; set; }
    public string? UserAgent { get; set; }
    
    // Navigation properties
    public Form Form { get; set; } = null!;
    public User User { get; set; } = null!;
    public User? UsuarioAprovador { get; set; }
    public ICollection<HistoricoFormSubmission> Historico { get; set; } = new List<HistoricoFormSubmission>();
}

public enum StatusSubmissao
{
    Rascunho = 1,
    Enviado = 2,
    EmAnalise = 3,
    Aprovado = 4,
    Rejeitado = 5,
    Cancelado = 6
}
```

### **2. NOVA MODEL: HistoricoFormSubmission**
```csharp
public class HistoricoFormSubmission
{
    public int Id { get; set; }
    public int FormSubmissionId { get; set; }
    public int UsuarioId { get; set; }
    public AcaoSubmissao Acao { get; set; }
    public string? DadosAnterioresJson { get; set; }
    public string? DadosNovosJson { get; set; }
    public string? Comentarios { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    
    public FormSubmission FormSubmission { get; set; } = null!;
    public User Usuario { get; set; } = null!;
}

public enum AcaoSubmissao
{
    Criado = 1,
    Atualizado = 2,
    Enviado = 3,
    Aprovado = 4,
    Rejeitado = 5,
    Cancelado = 6,
    Excluido = 7
}
```

## 🔧 MIGRATION NECESSÁRIA
```bash
dotnet ef migrations add AdicionarWorkflowSubmissao
dotnet ef database update
```

## 📋 CONTROLLER COMPLETO
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissoesFormularioController : ControllerBase
{
    private readonly IServicoSubmissaoFormulario _servico;
    private readonly ILogger<SubmissoesFormularioController> _logger;

    public SubmissoesFormularioController(IServicoSubmissaoFormulario servico, ILogger<SubmissoesFormularioController> logger)
    {
        _servico = servico;
        _logger = logger;
    }

    // GET: api/submissoesformulario (com filtros e paginação)
    [HttpGet]
    public async Task<ActionResult<ResultadoPaginado<SubmissaoFormularioDto>>> ObterSubmissoes(
        [FromQuery] FiltroSubmissaoFormularioDto filtro)
    {
        var usuarioId = ObterUsuarioAtualId();
        var resultado = await _servico.ObterSubmissoesAsync(usuarioId, filtro);
        return Ok(resultado);
    }

    // GET: api/submissoesformulario/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DetalheSubmissaoFormularioDto>> ObterSubmissao(int id)
    {
        var usuarioId = ObterUsuarioAtualId();
        var submissao = await _servico.ObterSubmissaoPorIdAsync(id, usuarioId);
        
        if (submissao == null) return NotFound();
        return Ok(submissao);
    }

    // POST: api/submissoesformulario
    [HttpPost]
    public async Task<ActionResult<SubmissaoFormularioDto>> CriarSubmissao(CriarSubmissaoDto criarDto)
    {
        var usuarioId = ObterUsuarioAtualId();
        var enderecoIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].FirstOrDefault();
        
        var submissao = await _servico.CriarSubmissaoAsync(usuarioId, criarDto, enderecoIp, userAgent);
        return CreatedAtAction(nameof(ObterSubmissao), new { id = submissao.Id }, submissao);
    }

    // PUT: api/submissoesformulario/5
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarSubmissao(int id, AtualizarSubmissaoDto atualizarDto)
    {
        var usuarioId = ObterUsuarioAtualId();
        await _servico.AtualizarSubmissaoAsync(id, usuarioId, atualizarDto);
        return NoContent();
    }

    // POST: api/submissoesformulario/5/enviar
    [HttpPost("{id}/enviar")]
    public async Task<IActionResult> EnviarSubmissao(int id)
    {
        var usuarioId = ObterUsuarioAtualId();
        await _servico.EnviarSubmissaoAsync(id, usuarioId);
        return NoContent();
    }

    // POST: api/submissoesformulario/5/aprovar
    [HttpPost("{id}/aprovar")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AprovarSubmissao(int id, [FromBody] AprovarSubmissaoDto dto)
    {
        var usuarioId = ObterUsuarioAtualId();
        await _servico.AprovarSubmissaoAsync(id, usuarioId, dto.Comentarios);
        return NoContent();
    }

    // POST: api/submissoesformulario/5/rejeitar
    [HttpPost("{id}/rejeitar")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> RejeitarSubmissao(int id, [FromBody] RejeitarSubmissaoDto dto)
    {
        var usuarioId = ObterUsuarioAtualId();
        await _servico.RejeitarSubmissaoAsync(id, usuarioId, dto.Motivo);
        return NoContent();
    }

    // DELETE: api/submissoesformulario/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirSubmissao(int id)
    {
        var usuarioId = ObterUsuarioAtualId();
        await _servico.ExcluirSubmissaoAsync(id, usuarioId);
        return NoContent();
    }

    // GET: api/submissoesformulario/5/historico
    [HttpGet("{id}/historico")]
    public async Task<ActionResult<List<HistoricoSubmissaoFormularioDto>>> ObterHistoricoSubmissao(int id)
    {
        var usuarioId = ObterUsuarioAtualId();
        var historico = await _servico.ObterHistoricoSubmissaoAsync(id, usuarioId);
        return Ok(historico);
    }

    private int ObterUsuarioAtualId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim?.Value ?? "0");
    }
}
```

## 📋 DTOs NECESSÁRIOS
```csharp
// Filtros para listagem
public class FiltroSubmissaoFormularioDto
{
    public int? FormularioId { get; set; }
    public StatusSubmissao? Status { get; set; }
    public DateTime? DataDe { get; set; }
    public DateTime? DataAte { get; set; }
    public string? Busca { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
}

// Criação de submission
public class CriarSubmissaoDto
{
    public int FormularioId { get; set; }
    public string DadosJson { get; set; } = string.Empty;
    public StatusSubmissao Status { get; set; } = StatusSubmissao.Rascunho;
}

// Atualização de submission
public class AtualizarSubmissaoDto
{
    public string DadosJson { get; set; } = string.Empty;
    public StatusSubmissao? Status { get; set; }
}

// Response para listagem
public class SubmissaoFormularioDto
{
    public int Id { get; set; }
    public int FormularioId { get; set; }
    public string NomeFormulario { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public StatusSubmissao Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
    public DateTime? DataSubmissao { get; set; }
}

// Response detalhado
public class DetalheSubmissaoFormularioDto : SubmissaoFormularioDto
{
    public string DadosJson { get; set; } = string.Empty;
    public DateTime? DataAprovacao { get; set; }
    public string? NomeUsuarioAprovador { get; set; }
    public string? MotivoRejeicao { get; set; }
    public List<HistoricoSubmissaoFormularioDto> Historico { get; set; } = new();
}

// Ações de aprovação/rejeição
public class AprovarSubmissaoDto
{
    public string? Comentarios { get; set; }
}

public class RejeitarSubmissaoDto
{
    public string Motivo { get; set; } = string.Empty;
}

// Histórico
public class HistoricoSubmissaoFormularioDto
{
    public int Id { get; set; }
    public AcaoSubmissao Acao { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string? Comentarios { get; set; }
    public DateTime DataCriacao { get; set; }
}

// Paginação
public class ResultadoPaginado<T>
{
    public List<T> Itens { get; set; } = new();
    public int TotalRegistros { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / TamanhoPagina);
}
```

## 🔧 SERVICE INTERFACE
```csharp
public interface IServicoSubmissaoFormulario
{
    Task<ResultadoPaginado<SubmissaoFormularioDto>> ObterSubmissoesAsync(int usuarioId, FiltroSubmissaoFormularioDto filtro);
    Task<DetalheSubmissaoFormularioDto?> ObterSubmissaoPorIdAsync(int id, int usuarioId);
    Task<SubmissaoFormularioDto> CriarSubmissaoAsync(int usuarioId, CriarSubmissaoDto criarDto, string? enderecoIp, string? userAgent);
    Task AtualizarSubmissaoAsync(int id, int usuarioId, AtualizarSubmissaoDto atualizarDto);
    Task EnviarSubmissaoAsync(int id, int usuarioId);
    Task AprovarSubmissaoAsync(int id, int usuarioAprovadorId, string? comentarios);
    Task RejeitarSubmissaoAsync(int id, int usuarioAprovadorId, string motivo);
    Task ExcluirSubmissaoAsync(int id, int usuarioId);
    Task<List<HistoricoSubmissaoFormularioDto>> ObterHistoricoSubmissaoAsync(int submissaoId, int usuarioId);
}
```

## 🔒 REGRAS DE NEGÓCIO OBRIGATÓRIAS

### **Permissões:**
- **Usuário comum**: Vê apenas suas próprias submissions
- **Manager**: Vê submissions dos formulários que tem acesso + pode aprovar
- **Admin**: Vê tudo + pode aprovar/rejeitar

### **Workflow de Status:**
1. **Rascunho** → **Enviado** (usuário submete)
2. **Enviado** → **EmAnalise** (admin pega para análise)
3. **EmAnalise** → **Aprovado/Rejeitado** (decisão final)
4. **Rejeitado** → **Rascunho** (usuário pode corrigir)

### **Validações:**
- Não pode editar submission aprovada
- Não pode deletar submission em análise
- Apenas Admin/Manager pode aprovar/rejeitar
- Sempre criar histórico nas mudanças de status

### **Histórico Automático:**
- Toda criação, edição, mudança de status deve gerar histórico
- Armazenar dados antigos e novos para auditoria
- Registrar usuário que fez a ação

## 🎯 CONFIGURAÇÃO DEPENDENCY INJECTION
```csharp
// Program.cs
services.AddScoped<IServicoSubmissaoFormulario, ServicoSubmissaoFormulario>();
services.AddAutoMapper(typeof(PerfilSubmissaoFormulario));
```

## 📊 ÍNDICES DE PERFORMANCE
```sql
-- Adicionar estes índices na migration
CREATE INDEX IX_FormSubmissions_Status ON FormSubmissions(Status);
CREATE INDEX IX_FormSubmissions_UserId_Status ON FormSubmissions(UserId, Status);
CREATE INDEX IX_FormSubmissions_FormId_Status ON FormSubmissions(FormId, Status);
CREATE INDEX IX_FormSubmissions_DataSubmissao ON FormSubmissions(DataSubmissao);
CREATE INDEX IX_HistoricoFormSubmission_FormSubmissionId ON HistoricoFormSubmission(FormSubmissionId);
```

## 🎯 RESULTADO ESPERADO
Com essa implementação você terá:

✅ **CRUD completo** de submissions com paginação e filtros  
✅ **Workflow de aprovação** Draft → Submitted → Approved/Rejected  
✅ **Histórico completo** de todas as ações  
✅ **Permissões** baseadas em roles  
✅ **APIs RESTful** prontas para o frontend  
✅ **Performance otimizada** com índices corretos  
✅ **Compatibilidade 100%** com FormEngine.io existente  

O sistema permitirá que usuários preencham formulários, salvem rascunhos, submetam para aprovação e acompanhem o status, enquanto admins terão controle total do workflow!