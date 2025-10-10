# Script para Iniciar o Projeto com Docker

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   FormEngine API - Docker Startup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se Docker Desktop está rodando
Write-Host "1. Verificando Docker..." -ForegroundColor Yellow
$dockerRunning = $false
try {
    docker ps | Out-Null
    $dockerRunning = $true
    Write-Host "   ✓ Docker está rodando!" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Docker não está rodando" -ForegroundColor Red
}

# Se Docker não está rodando, tentar iniciar
if (-not $dockerRunning) {
    Write-Host ""
    Write-Host "2. Tentando iniciar Docker Desktop..." -ForegroundColor Yellow
    
    # Procurar Docker Desktop
    $dockerDesktopPath = "C:\Program Files\Docker\Docker\Docker Desktop.exe"
    
    if (Test-Path $dockerDesktopPath) {
        Start-Process $dockerDesktopPath
        Write-Host "   Docker Desktop iniciado!" -ForegroundColor Green
        Write-Host "   Aguardando Docker ficar pronto..." -ForegroundColor Yellow
        
        # Aguardar até Docker estar pronto (máximo 60 segundos)
        $timeout = 60
        $elapsed = 0
        while ($elapsed -lt $timeout) {
            Start-Sleep -Seconds 2
            $elapsed += 2
            try {
                docker ps | Out-Null
                Write-Host "   ✓ Docker está pronto!" -ForegroundColor Green
                $dockerRunning = $true
                break
            } catch {
                Write-Host "   Aguardando... ($elapsed/$timeout segundos)" -ForegroundColor Gray
            }
        }
        
        if (-not $dockerRunning) {
            Write-Host "   ✗ Timeout aguardando Docker" -ForegroundColor Red
            Write-Host ""
            Write-Host "Por favor:" -ForegroundColor Yellow
            Write-Host "1. Abra o Docker Desktop manualmente" -ForegroundColor White
            Write-Host "2. Aguarde ele inicializar completamente" -ForegroundColor White
            Write-Host "3. Execute este script novamente" -ForegroundColor White
            Write-Host ""
            Read-Host "Pressione Enter para sair"
            exit 1
        }
    } else {
        Write-Host "   ✗ Docker Desktop não encontrado em: $dockerDesktopPath" -ForegroundColor Red
        Write-Host ""
        Write-Host "Por favor:" -ForegroundColor Yellow
        Write-Host "1. Instale o Docker Desktop: https://www.docker.com/products/docker-desktop" -ForegroundColor White
        Write-Host "2. Ou inicie o Docker Desktop manualmente" -ForegroundColor White
        Write-Host "3. Execute este script novamente" -ForegroundColor White
        Write-Host ""
        Read-Host "Pressione Enter para sair"
        exit 1
    }
}

Write-Host ""
Write-Host "3. Parando containers antigos (se existirem)..." -ForegroundColor Yellow
docker-compose down 2>$null
Write-Host "   ✓ Feito!" -ForegroundColor Green

Write-Host ""
Write-Host "4. Construindo e iniciando containers..." -ForegroundColor Yellow
Write-Host "   (Isso pode levar alguns minutos na primeira vez)" -ForegroundColor Gray
Write-Host ""

docker-compose up -d --build

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "   ✓ PROJETO INICIADO COM SUCESSO!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Serviços disponíveis:" -ForegroundColor Cyan
    Write-Host "  • API + Swagger: http://localhost:5000" -ForegroundColor White
    Write-Host "  • MySQL:         localhost:3306" -ForegroundColor White
    Write-Host ""
    Write-Host "Credenciais padrão:" -ForegroundColor Cyan
    Write-Host "  • Email:  admin@formengine.com" -ForegroundColor White
    Write-Host "  • Senha:  Admin@123" -ForegroundColor White
    Write-Host ""
    Write-Host "Comandos úteis:" -ForegroundColor Cyan
    Write-Host "  • Ver logs:    docker-compose logs -f api" -ForegroundColor White
    Write-Host "  • Parar tudo:  docker-compose down" -ForegroundColor White
    Write-Host "  • Reiniciar:   docker-compose restart" -ForegroundColor White
    Write-Host ""
    
    # Aguardar alguns segundos para a API inicializar
    Write-Host "Aguardando API inicializar..." -ForegroundColor Yellow
    Start-Sleep -Seconds 5
    
    # Tentar abrir o navegador
    Write-Host "Abrindo Swagger no navegador..." -ForegroundColor Yellow
    Start-Process "http://localhost:5000"
    
    Write-Host ""
    Write-Host "Deseja ver os logs da API? (S/N)" -ForegroundColor Yellow
    $response = Read-Host
    if ($response -eq "S" -or $response -eq "s") {
        docker-compose logs -f api
    }
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "   ✗ ERRO AO INICIAR PROJETO" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Verifique os logs:" -ForegroundColor Yellow
    Write-Host "  docker-compose logs" -ForegroundColor White
    Write-Host ""
    Read-Host "Pressione Enter para sair"
    exit 1
}
