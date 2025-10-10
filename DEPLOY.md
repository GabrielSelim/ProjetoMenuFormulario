# Guia de Deploy - FormEngine API

## Deploy com Docker (Recomendado)

### Pré-requisitos
- Docker
- Docker Compose
- Servidor Linux/Windows com Docker instalado

### Passos

1. **Clone o repositório no servidor**
```bash
git clone <url-do-repositorio>
cd ProjetoMenuFormulario
```

2. **Configure variáveis de ambiente**
Crie um arquivo `.env`:
```bash
cp .env.example .env
nano .env
```

Edite as seguintes variáveis:
```env
MYSQL_ROOT_PASSWORD=senha_forte_aqui
JwtSettings__SecretKey=sua-chave-secreta-muito-forte-com-mais-de-32-caracteres
```

3. **Inicie os containers**
```bash
docker-compose up -d
```

4. **Verifique os logs**
```bash
docker-compose logs -f api
```

5. **Acesse a API**
- API: `http://seu-servidor:5000`
- Swagger: `http://seu-servidor:5000`

### Configuração de Proxy Reverso (Nginx)

Crie um arquivo de configuração Nginx:

```nginx
server {
    listen 80;
    server_name api.seudominio.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### SSL com Let's Encrypt

```bash
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d api.seudominio.com
```

---

## Deploy Manual (sem Docker)

### Pré-requisitos
- .NET 8 SDK/Runtime
- MySQL 8.0
- Servidor Linux/Windows

### Passos

1. **Publique a aplicação**
```bash
dotnet publish -c Release -o ./publish
```

2. **Configure o banco de dados**
- Instale MySQL
- Crie o banco: `CREATE DATABASE formengine_db;`
- Atualize a connection string em `appsettings.json`

3. **Aplique as migrations**
```bash
cd publish
dotnet FormEngineAPI.dll ef database update
```

4. **Configure como serviço (Linux - systemd)**

Crie `/etc/systemd/system/formengineapi.service`:

```ini
[Unit]
Description=FormEngine API
After=network.target

[Service]
WorkingDirectory=/var/www/formengineapi
ExecStart=/usr/bin/dotnet /var/www/formengineapi/FormEngineAPI.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=formengineapi
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

Ative o serviço:
```bash
sudo systemctl enable formengineapi
sudo systemctl start formengineapi
sudo systemctl status formengineapi
```

---

## Deploy em Cloud Providers

### Azure App Service

1. **Crie um App Service**
```bash
az webapp create --resource-group myResourceGroup --plan myAppServicePlan --name formengine-api --runtime "DOTNETCORE:8.0"
```

2. **Configure connection string**
```bash
az webapp config connection-string set --resource-group myResourceGroup --name formengine-api --settings DefaultConnection="Server=..." --connection-string-type MySql
```

3. **Deploy**
```bash
az webapp deployment source config-zip --resource-group myResourceGroup --name formengine-api --src publish.zip
```

### AWS Elastic Beanstalk

1. **Instale EB CLI**
```bash
pip install awsebcli
```

2. **Inicialize**
```bash
eb init -p "64bit Amazon Linux 2 v2.5.0 running .NET Core" formengine-api
```

3. **Deploy**
```bash
eb create formengine-api-env
eb deploy
```

### Google Cloud Run

1. **Build e push da imagem**
```bash
gcloud builds submit --tag gcr.io/PROJECT-ID/formengine-api
```

2. **Deploy**
```bash
gcloud run deploy formengine-api --image gcr.io/PROJECT-ID/formengine-api --platform managed --region us-central1 --allow-unauthenticated
```

---

## Configurações de Produção

### appsettings.Production.json

Crie um arquivo `appsettings.Production.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=production-db;Port=3306;Database=formengine_db;User=prod_user;Password=${DB_PASSWORD};"
  },
  "JwtSettings": {
    "SecretKey": "${JWT_SECRET}",
    "ExpirationHours": "24"
  },
  "AllowedHosts": "api.seudominio.com"
}
```

### Variáveis de Ambiente Recomendadas

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://+:80
export ConnectionStrings__DefaultConnection="Server=..."
export JwtSettings__SecretKey="..."
```

### Segurança

1. **HTTPS Only**
   - Configure certificado SSL
   - Force HTTPS redirect

2. **CORS**
   - Limite origens permitidas
   - Em produção, substitua `AllowAll` por domínios específicos

3. **Rate Limiting**
   - Implemente rate limiting
   - Use ferramentas como Nginx ou API Gateway

4. **Secrets**
   - Use Azure Key Vault, AWS Secrets Manager, ou similar
   - Nunca commite secrets no código

### Monitoring

1. **Application Insights (Azure)**
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

2. **Logs**
   - Configure Serilog ou NLog
   - Envie logs para serviço centralizado (ELK, Splunk)

3. **Health Checks**
Adicione em `Program.cs`:
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

app.MapHealthChecks("/health");
```

### Backup

**Backup do Banco de Dados (MySQL)**
```bash
# Backup
mysqldump -u root -p formengine_db > backup_$(date +%Y%m%d_%H%M%S).sql

# Restore
mysql -u root -p formengine_db < backup_20250109_120000.sql
```

**Script de Backup Automatizado**
```bash
#!/bin/bash
BACKUP_DIR="/backups/mysql"
DATE=$(date +%Y%m%d_%H%M%S)
mysqldump -u root -p${MYSQL_ROOT_PASSWORD} formengine_db > ${BACKUP_DIR}/backup_${DATE}.sql
find ${BACKUP_DIR} -name "backup_*.sql" -mtime +7 -delete
```

### Performance

1. **Connection Pooling** - Já configurado no EF Core
2. **Response Caching** - Para endpoints GET
3. **CDN** - Para assets estáticos
4. **Database Indexing** - Já implementado nas migrations

---

## Troubleshooting

### API não inicia
```bash
# Verifique logs
docker-compose logs api

# Verifique se o banco está rodando
docker-compose ps
```

### Erro de conexão com banco
```bash
# Teste conexão
docker exec -it formengine_mysql mysql -u root -p

# Verifique variáveis de ambiente
docker-compose config
```

### Migrations não aplicadas
```bash
# Aplique manualmente
docker exec -it formengine_api dotnet ef database update
```

### Performance lenta
- Verifique índices do banco
- Analise queries com SQL Profiler
- Aumente recursos do container

---

## Checklist de Deploy

- [ ] Variáveis de ambiente configuradas
- [ ] Connection string de produção
- [ ] JWT Secret forte e seguro
- [ ] CORS configurado corretamente
- [ ] HTTPS configurado
- [ ] Backup automatizado
- [ ] Monitoring ativo
- [ ] Health checks funcionando
- [ ] Logs centralizados
- [ ] Documentação atualizada
- [ ] Testes passando
- [ ] Migrations aplicadas
- [ ] Seed data (se necessário)

---

Para mais informações, consulte a documentação oficial:
- [ASP.NET Core Deployment](https://docs.microsoft.com/aspnet/core/host-and-deploy/)
- [Docker Documentation](https://docs.docker.com/)
