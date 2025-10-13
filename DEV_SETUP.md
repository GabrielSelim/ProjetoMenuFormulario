# Configuração do Ambiente de Desenvolvimento Local

Este documento explica como configurar e usar o ambiente de desenvolvimento local.

## Pré-requisitos

- Docker Desktop instalado e rodando
- .NET 8.0 SDK instalado
- Visual Studio 2022 ou VS Code

## Configuração Inicial

### 1. Iniciar o Banco MySQL

```powershell
# Opção 1: Usar o script automatizado (RECOMENDADO)
.\dev-setup.ps1 start

# Opção 2: Manualmente
docker-compose up -d db
dotnet ef database update
```

### 2. Executar a API

```powershell
# Via linha de comando
dotnet run

# Ou usar o Visual Studio/VS Code
# F5 ou Ctrl+F5
```

## Scripts Disponíveis

### dev-setup.ps1

Script PowerShell para gerenciar o ambiente de desenvolvimento:

```powershell
# Iniciar tudo (MySQL + migrations)
.\dev-setup.ps1 start

# Iniciar apenas MySQL
.\dev-setup.ps1 db-only

# Parar containers
.\dev-setup.ps1 stop

# Reiniciar containers
.\dev-setup.ps1 restart

# Ver logs do MySQL
.\dev-setup.ps1 logs

# Ver status
.\dev-setup.ps1 status
```

## Configurações de Desenvolvimento

### Banco de Dados Local

- **Host**: localhost
- Port: 3309
- **Database**: formengine_db
- **Usuário**: root
- **Senha**: rootpassword

### URLs da API

- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: http://localhost:5000/swagger

### Configurações JWT

As configurações JWT para desenvolvimento estão em `appsettings.Development.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-change-this-in-development-min-32-chars",
    "ExpirationHours": 24
  }
}
```

## Fluxo de Desenvolvimento

### 1. Primeira vez
```powershell
# Clone o repositório (se ainda não fez)
git clone <repo-url>
cd ProjetoMenuFormulario

# Inicie o ambiente
.\dev-setup.ps1 start

# Execute a API
dotnet run
```

### 2. Dia a dia
```powershell
# Ao começar a trabalhar
.\dev-setup.ps1 start

# Desenvolver normalmente...
# dotnet run ou F5 no Visual Studio

# Ao terminar (opcional - containers ficam rodando)
.\dev-setup.ps1 stop
```

### 3. Resetar banco (quando necessário)
```powershell
# Parar containers
.\dev-setup.ps1 stop

# Remover volumes (CUIDADO: perde dados)
docker-compose down -v

# Reiniciar
.\dev-setup.ps1 start
```

## Troubleshooting

### MySQL não inicia
```powershell
# Verificar logs
.\dev-setup.ps1 logs

# Verificar se porta 3309 está livre
netstat -an | findstr :3309

# Reiniciar containers
.\dev-setup.ps1 restart
```

### Erro de conexão na API
```powershell
# Verificar se MySQL está rodando
.\dev-setup.ps1 status

# Testar conexão manualmente
docker-compose exec db mysql -u root -prootpassword -e "SHOW DATABASES;"
```

### Problemas com migrations
```powershell
# Aplicar migrations manualmente
dotnet ef database update

# Ver status das migrations
dotnet ef migrations list
```

## Estrutura do Docker

### docker-compose.yml

O arquivo está configurado para desenvolvimento com:

- **MySQL**: Porta 3309 (para não conflitar com MySQL local)
- **API**: Desabilitada por padrão (roda via dotnet run)
- **Volumes**: Persistência dos dados do MySQL
- **Health checks**: Garantem que MySQL está pronto

### Containers em produção vs desenvolvimento

- **Desenvolvimento**: Apenas MySQL no Docker, API roda localmente
- **Produção**: MySQL + API + Nginx todos no Docker

## Conectar com ferramentas externas

### MySQL Workbench
- Host: localhost
- Port: 3309
- Username: root
- Password: rootpassword

### VS Code MySQL Extension
```json
{
  "host": "localhost",
  "port": 3307,
  "user": "root",
  "password": "rootpassword",
  "database": "formengine_db"
}
```

## Próximos Passos

Após configurar o ambiente, você pode:

1. Testar os endpoints no Swagger: http://localhost:5000/swagger
2. Criar um usuário admin via endpoint `/api/Auth/register`
3. Fazer login e obter JWT token
4. Testar os novos endpoints de submissões com workflow