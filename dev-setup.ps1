# Script para gerenciar o ambiente de desenvolvimento local
# Uso: .\dev-setup.ps1 [start|stop|restart|logs|db-only]

param(
    [Parameter(Position=0)]
    [ValidateSet("start", "stop", "restart", "logs", "db-only", "status")]
    [string]$Action = "start"
)

Write-Host "=== FormEngine Development Environment ===" -ForegroundColor Green

switch ($Action) {
    "start" {
        Write-Host "Iniciando banco MySQL no Docker..." -ForegroundColor Yellow
        docker-compose up -d db
        
        Write-Host "Aguardando MySQL ficar disponível..." -ForegroundColor Yellow
        $maxAttempts = 30
        $attempt = 0
        do {
            $attempt++
            Start-Sleep -Seconds 2
            $result = docker-compose exec -T db mysqladmin ping -h localhost -u root -prootpassword 2>$null
            if ($LASTEXITCODE -eq 0) {
                Write-Host "MySQL está pronto!" -ForegroundColor Green
                break
            }
            Write-Host "Tentativa $attempt/$maxAttempts - Aguardando MySQL..." -ForegroundColor Yellow
        } while ($attempt -lt $maxAttempts)
        
        if ($attempt -eq $maxAttempts) {
            Write-Host "Erro: MySQL não ficou disponível após $maxAttempts tentativas" -ForegroundColor Red
            exit 1
        }
        
        Write-Host "Aplicando migrations..." -ForegroundColor Yellow
        dotnet ef database update
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Ambiente pronto! Você pode executar:" -ForegroundColor Green
            Write-Host "  dotnet run" -ForegroundColor Cyan
            Write-Host "  ou usar o Visual Studio" -ForegroundColor Cyan
            Write-Host ""
            Write-Host "MySQL está disponível em: localhost:3309" -ForegroundColor White
            Write-Host "Database: formengine_db" -ForegroundColor White
            Write-Host "User: root / Password: rootpassword" -ForegroundColor White
        } else {
            Write-Host "Erro ao aplicar migrations" -ForegroundColor Red
        }
    }
    
    "db-only" {
        Write-Host "Iniciando apenas o banco MySQL..." -ForegroundColor Yellow
        docker-compose up -d db
        Write-Host "MySQL disponível em localhost:3309" -ForegroundColor Green
    }
    
    "stop" {
        Write-Host "Parando containers..." -ForegroundColor Yellow
        docker-compose down
        Write-Host "Containers parados" -ForegroundColor Green
    }
    
    "restart" {
        Write-Host "Reiniciando containers..." -ForegroundColor Yellow
        docker-compose down
        docker-compose up -d db
        Write-Host "Containers reiniciados" -ForegroundColor Green
    }
    
    "logs" {
        Write-Host "Mostrando logs do MySQL..." -ForegroundColor Yellow
        docker-compose logs -f db
    }
    
    "status" {
        Write-Host "Status dos containers:" -ForegroundColor Yellow
        docker-compose ps
        
        Write-Host "`nTestando conexão com MySQL..." -ForegroundColor Yellow
        $result = docker-compose exec -T db mysqladmin ping -h localhost -u root -prootpassword 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ MySQL está funcionando" -ForegroundColor Green
        } else {
            Write-Host "✗ MySQL não está respondendo" -ForegroundColor Red
        }
    }
    
    default {
        Write-Host "Uso: .\dev-setup.ps1 [start|stop|restart|logs|db-only|status]" -ForegroundColor White
        Write-Host ""
        Write-Host "Comandos:" -ForegroundColor Yellow
        Write-Host "  start    - Inicia MySQL e aplica migrations" -ForegroundColor White
        Write-Host "  db-only  - Inicia apenas o MySQL" -ForegroundColor White
        Write-Host "  stop     - Para todos os containers" -ForegroundColor White
        Write-Host "  restart  - Reinicia os containers" -ForegroundColor White
        Write-Host "  logs     - Mostra logs do MySQL" -ForegroundColor White
        Write-Host "  status   - Mostra status dos containers" -ForegroundColor White
    }
}