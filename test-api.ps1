# Script de Teste da API FormEngine
# Execute após iniciar a aplicação com: dotnet run ou docker-compose up

$baseUrl = "http://localhost:5000/api"

Write-Host "=== FormEngine API - Testes ===" -ForegroundColor Green
Write-Host ""

# 1. Teste de Health Check / Login Admin
Write-Host "1. Testando login com usuário admin padrão..." -ForegroundColor Yellow

$loginBody = @{
    email = "admin@formengine.com"
    password = "Admin@123"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body $loginBody -ContentType "application/json"
    $token = $loginResponse.token
    Write-Host "✓ Login realizado com sucesso!" -ForegroundColor Green
    Write-Host "  Token: $($token.Substring(0, 50))..." -ForegroundColor Gray
    Write-Host "  Usuário: $($loginResponse.user.name) ($($loginResponse.user.role))" -ForegroundColor Gray
} catch {
    Write-Host "✗ Erro no login: $($_.Exception.Message)" -ForegroundColor Red
    exit
}

Write-Host ""

# 2. Teste de listagem de formulários
Write-Host "2. Listando formulários..." -ForegroundColor Yellow

$headers = @{
    Authorization = "Bearer $token"
}

try {
    $forms = Invoke-RestMethod -Uri "$baseUrl/forms" -Method Get -Headers $headers
    Write-Host "✓ Formulários listados com sucesso! Total: $($forms.Count)" -ForegroundColor Green
} catch {
    Write-Host "✗ Erro ao listar formulários: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# 3. Teste de criação de formulário
Write-Host "3. Criando novo formulário..." -ForegroundColor Yellow

$formBody = @{
    name = "Teste Automatizado - $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
    schemaJson = '{"fields":[{"type":"text","label":"Nome","required":true},{"type":"email","label":"Email","required":true}]}'
    rolesAllowed = '["admin","gestor","user"]'
    version = "1.0.0"
} | ConvertTo-Json

try {
    $newForm = Invoke-RestMethod -Uri "$baseUrl/forms" -Method Post -Body $formBody -Headers $headers -ContentType "application/json"
    $formId = $newForm.id
    Write-Host "✓ Formulário criado com sucesso!" -ForegroundColor Green
    Write-Host "  ID: $formId" -ForegroundColor Gray
    Write-Host "  Nome: $($newForm.name)" -ForegroundColor Gray
} catch {
    Write-Host "✗ Erro ao criar formulário: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# 4. Teste de criação de submissão
if ($formId) {
    Write-Host "4. Criando submissão do formulário..." -ForegroundColor Yellow
    
    $submissionBody = @{
        formId = $formId
        dataJson = '{"nome":"João Teste","email":"joao.teste@example.com"}'
    } | ConvertTo-Json
    
    try {
        $newSubmission = Invoke-RestMethod -Uri "$baseUrl/submissions" -Method Post -Body $submissionBody -Headers $headers -ContentType "application/json"
        Write-Host "✓ Submissão criada com sucesso!" -ForegroundColor Green
        Write-Host "  ID: $($newSubmission.id)" -ForegroundColor Gray
        Write-Host "  Form ID: $($newSubmission.formId)" -ForegroundColor Gray
    } catch {
        Write-Host "✗ Erro ao criar submissão: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""

# 5. Teste de listagem de menus
Write-Host "5. Listando menus..." -ForegroundColor Yellow

try {
    $menus = Invoke-RestMethod -Uri "$baseUrl/menus" -Method Get -Headers $headers
    Write-Host "✓ Menus listados com sucesso! Total: $($menus.Count)" -ForegroundColor Green
} catch {
    Write-Host "✗ Erro ao listar menus: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# 6. Teste de criação de menu
Write-Host "6. Criando novo menu..." -ForegroundColor Yellow

$menuBody = @{
    name = "Menu Teste - $(Get-Date -Format 'HH:mm:ss')"
    contentType = "page"
    urlOrPath = "/test"
    rolesAllowed = '["admin"]'
    order = 99
    icon = "test-icon"
} | ConvertTo-Json

try {
    $newMenu = Invoke-RestMethod -Uri "$baseUrl/menus" -Method Post -Body $menuBody -Headers $headers -ContentType "application/json"
    Write-Host "✓ Menu criado com sucesso!" -ForegroundColor Green
    Write-Host "  ID: $($newMenu.id)" -ForegroundColor Gray
    Write-Host "  Nome: $($newMenu.name)" -ForegroundColor Gray
} catch {
    Write-Host "✗ Erro ao criar menu: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Testes Concluídos ===" -ForegroundColor Green
Write-Host ""
Write-Host "Acesse a documentação Swagger em: http://localhost:5000" -ForegroundColor Cyan
