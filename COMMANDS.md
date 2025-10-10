# Comandos Úteis - FormEngine API

## Docker

### Iniciar aplicação
```bash
docker-compose up -d
```

### Ver logs
```bash
docker-compose logs -f api
docker-compose logs -f db
```

### Parar aplicação
```bash
docker-compose down
```

### Reconstruir após mudanças
```bash
docker-compose up -d --build
```

### Remover volumes (limpar banco de dados)
```bash
docker-compose down -v
```

## Entity Framework

### Criar nova migration
```bash
dotnet ef migrations add NomeDaMigration
```

### Aplicar migrations
```bash
dotnet ef database update
```

### Reverter última migration
```bash
dotnet ef migrations remove
```

### Listar migrations
```bash
dotnet ef migrations list
```

### Gerar script SQL da migration
```bash
dotnet ef migrations script
```

## Desenvolvimento

### Restaurar pacotes
```bash
dotnet restore
```

### Compilar
```bash
dotnet build
```

### Executar
```bash
dotnet run
```

### Executar com watch (recarrega ao salvar)
```bash
dotnet watch run
```

### Limpar build
```bash
dotnet clean
```

## Testes com curl

### Registrar usuário
```bash
curl -X POST http://localhost:5000/api/auth/register ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Test User\",\"email\":\"test@test.com\",\"password\":\"Test@123\",\"role\":\"user\"}"
```

### Login
```bash
curl -X POST http://localhost:5000/api/auth/login ^
  -H "Content-Type: application/json" ^
  -d "{\"email\":\"admin@formengine.com\",\"password\":\"Admin@123\"}"
```

### Listar formulários (com token)
```bash
curl -X GET http://localhost:5000/api/forms ^
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### Criar formulário (com token)
```bash
curl -X POST http://localhost:5000/api/forms ^
  -H "Authorization: Bearer SEU_TOKEN_AQUI" ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Teste\",\"schemaJson\":\"{}\",\"rolesAllowed\":\"[]\",\"version\":\"1.0.0\"}"
```

## MySQL

### Conectar ao MySQL no Docker
```bash
docker exec -it formengine_mysql mysql -u root -prootpassword formengine_db
```

### Queries úteis
```sql
-- Ver todas as tabelas
SHOW TABLES;

-- Ver estrutura de uma tabela
DESCRIBE Users;

-- Ver todos os usuários
SELECT * FROM Users;

-- Ver todos os formulários
SELECT * FROM Forms;

-- Ver logs de atividade
SELECT * FROM ActivityLogs ORDER BY Timestamp DESC LIMIT 10;
```

## NuGet

### Adicionar pacote
```bash
dotnet add package NomeDoPacote
```

### Remover pacote
```bash
dotnet remove package NomeDoPacote
```

### Listar pacotes
```bash
dotnet list package
```

### Atualizar pacotes
```bash
dotnet list package --outdated
dotnet add package NomeDoPacote
```

## Publicação

### Publicar para produção
```bash
dotnet publish -c Release -o ./publish
```

### Build Docker manualmente
```bash
docker build -t formengine-api .
docker run -p 5000:80 formengine-api
```
