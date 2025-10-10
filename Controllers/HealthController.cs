using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormEngineAPI.Data;

namespace FormEngineAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Verificar se a API está respondendo
            var timestamp = DateTime.UtcNow;
            
            // Verificar conexão com banco de dados
            var canConnect = await _context.Database.CanConnectAsync();
            if (!canConnect)
            {
                _logger.LogError("Health check falhou: Não foi possível conectar ao banco de dados");
                return StatusCode(503, new
                {
                    status = "Unhealthy",
                    timestamp = timestamp,
                    database = "Disconnected",
                    message = "Não foi possível conectar ao banco de dados"
                });
            }

            // Verificar se existem tabelas (migrations aplicadas)
            var tablesExist = await _context.Users.CountAsync() >= 0;
            
            return Ok(new
            {
                status = "Healthy",
                timestamp = timestamp,
                database = "Connected",
                migrations = "Applied",
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check falhou com erro: {Message}", ex.Message);
            return StatusCode(503, new
            {
                status = "Unhealthy",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }

    [HttpGet("database")]
    public async Task<IActionResult> DatabaseHealth()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            var userCount = await _context.Users.CountAsync();
            
            return Ok(new
            {
                database = new
                {
                    connected = canConnect,
                    userCount = userCount,
                    timestamp = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check falhou: {Message}", ex.Message);
            return StatusCode(503, new
            {
                database = new
                {
                    connected = false,
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                }
            });
        }
    }
}