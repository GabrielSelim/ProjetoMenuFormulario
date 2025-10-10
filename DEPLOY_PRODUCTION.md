# 🚀 Deploy para Produção - FormEngine API

## 📋 Pré-requisitos no Servidor

### Sistema Operacional
- Ubuntu 20.04+ ou CentOS 7+
- Acesso root ou sudo

### Software Necessário
- Docker
- Docker Compose
- Git
- Nginx (como reverse proxy)
- Certbot (para SSL/HTTPS)

## 🔧 Passo 1: Preparar o Servidor

### 1.1 Atualizar Sistema
```bash
sudo apt update && sudo apt upgrade -y
```

### 1.2 Instalar Docker
```bash
# Remover versões antigas
sudo apt remove docker docker-engine docker.io containerd runc

# Instalar dependências
sudo apt install apt-transport-https ca-certificates curl gnupg lsb-release

# Adicionar chave GPG oficial do Docker
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg

# Adicionar repositório
echo "deb [arch=amd64 signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Instalar Docker
sudo apt update
sudo apt install docker-ce docker-ce-cli containerd.io

# Adicionar usuário ao grupo docker
sudo usermod -aG docker $USER
```

### 1.3 Instalar Docker Compose
```bash
sudo curl -L "https://github.com/docker/compose/releases/download/v2.21.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
```

### 1.4 Instalar Nginx
```bash
sudo apt install nginx -y
sudo systemctl enable nginx
sudo systemctl start nginx
```

### 1.5 Instalar Certbot
```bash
sudo apt install certbot python3-certbot-nginx -y
```

## 📂 Passo 2: Clonar e Configurar Projeto

### 2.1 Clonar Repositório
```bash
cd /opt
sudo git clone https://github.com/GabrielSelim/ProjetoMenuFormulario.git
sudo chown -R $USER:$USER /opt/ProjetoMenuFormulario
cd /opt/ProjetoMenuFormulario
```

### 2.2 Criar docker-compose.prod.yml
```bash
nano docker-compose.prod.yml
```

**Conteúdo do arquivo:**
```yaml
version: '3.8'

services:
  db:
    image: mysql:8.0
    container_name: formengine_mysql_prod
    restart: always
    environment:
      MYSQL_ROOT_PASSWORD: ${MYSQL_ROOT_PASSWORD}
      MYSQL_DATABASE: formengine_db
      MYSQL_USER: formengine_user
      MYSQL_PASSWORD: ${MYSQL_PASSWORD}
    ports:
      - "127.0.0.1:3306:3306"  # Só aceita conexões locais
    volumes:
      - mysql_prod_data:/var/lib/mysql
      - ./mysql-init:/docker-entrypoint-initdb.d
    networks:
      - formengine_network
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost", "-u", "root", "-p${MYSQL_ROOT_PASSWORD}"]
      interval: 10s
      timeout: 5s
      retries: 10
      start_period: 60s

  api:
    build:
      context: .
      dockerfile: Dockerfile.prod
    container_name: formengine_api_prod
    restart: always
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=Server=db;Port=3306;Database=formengine_db;User=formengine_user;Password=${MYSQL_PASSWORD};
      - JwtSettings__SecretKey=${JWT_SECRET_KEY}
      - JwtSettings__ExpirationHours=24
      - ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
    ports:
      - "127.0.0.1:5000:80"  # Só aceita conexões locais
    depends_on:
      db:
        condition: service_healthy
    networks:
      - formengine_network
    volumes:
      - ./logs:/app/logs

volumes:
  mysql_prod_data:

networks:
  formengine_network:
    driver: bridge
```

### 2.3 Criar Dockerfile.prod
```bash
nano Dockerfile.prod
```

**Conteúdo do arquivo:**
```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY FormEngineAPI.csproj ./
RUN dotnet restore FormEngineAPI.csproj

# Copy everything else and build
COPY . .
RUN dotnet build FormEngineAPI.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish FormEngineAPI.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

EXPOSE 80

ENTRYPOINT ["dotnet", "FormEngineAPI.dll"]
```

### 2.4 Criar arquivo de variáveis de ambiente
```bash
nano .env.prod
```

**Conteúdo do arquivo:**
```env
# MySQL Settings
MYSQL_ROOT_PASSWORD=SUA_SENHA_ROOT_SUPER_SEGURA_AQUI
MYSQL_PASSWORD=SUA_SENHA_USER_SUPER_SEGURA_AQUI

# JWT Settings
JWT_SECRET_KEY=SUA_CHAVE_JWT_SUPER_SEGURA_DE_PELO_MENOS_32_CARACTERES_AQUI

# Domain
DOMAIN=formsmenuapi.gabrielsanztech.com.br
```

**⚠️ IMPORTANTE:** Substitua as senhas por senhas seguras reais!

## 🌐 Passo 3: Configurar Nginx

### 3.1 Criar configuração do Nginx
```bash
sudo nano /etc/nginx/sites-available/formsmenuapi.gabrielsanztech.com.br
```

**Conteúdo do arquivo:**
```nginx
server {
    listen 80;
    server_name formsmenuapi.gabrielsanztech.com.br;

    # Redirect all HTTP requests to HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name formsmenuapi.gabrielsanztech.com.br;

    # SSL Configuration (será configurado pelo Certbot)
    # ssl_certificate /etc/letsencrypt/live/formsmenuapi.gabrielsanztech.com.br/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/formsmenuapi.gabrielsanztech.com.br/privkey.pem;

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Referrer-Policy "no-referrer-when-downgrade" always;
    add_header Content-Security-Policy "default-src 'self' http: https: data: blob: 'unsafe-inline'" always;

    # Proxy settings
    location / {
        proxy_pass http://127.0.0.1:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        
        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
        
        # Buffer settings
        proxy_buffering on;
        proxy_buffer_size 128k;
        proxy_buffers 4 256k;
        proxy_busy_buffers_size 256k;
    }

    # Health check endpoint
    location /health {
        proxy_pass http://127.0.0.1:5000/health;
        access_log off;
    }

    # Static files optimization
    location ~* \.(jpg|jpeg|png|gif|ico|css|js)$ {
        proxy_pass http://127.0.0.1:5000;
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

### 3.2 Ativar site
```bash
sudo ln -s /etc/nginx/sites-available/formsmenuapi.gabrielsanztech.com.br /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

## 🔒 Passo 4: Configurar HTTPS com Let's Encrypt

### 4.1 Obter certificado SSL
```bash
sudo certbot --nginx -d formsmenuapi.gabrielsanztech.com.br
```

Siga as instruções do Certbot:
1. Digite seu email
2. Aceite os termos
3. Escolha se quer compartilhar email (opcional)
4. Certbot configurará automaticamente o SSL

### 4.2 Testar renovação automática
```bash
sudo certbot renew --dry-run
```

## 🚀 Passo 5: Deploy da Aplicação

### 5.1 Carregar variáveis de ambiente
```bash
cd /opt/ProjetoMenuFormulario
export $(cat .env.prod | xargs)
```

### 5.2 Parar containers antigos (se existirem)
```bash
docker-compose -f docker-compose.prod.yml down -v
```

### 5.3 Construir e iniciar aplicação
```bash
docker-compose -f docker-compose.prod.yml up -d --build
```

### 5.4 Verificar logs
```bash
docker-compose -f docker-compose.prod.yml logs -f api
```

Deve mostrar:
```
✓ Conexão com banco de dados estabelecida!
✓ Migrations aplicadas com sucesso!
Application started...
```

## 🔧 Passo 6: Scripts de Automação

### 6.1 Script de deploy
```bash
nano deploy.sh
chmod +x deploy.sh
```

**Conteúdo do script:**
```bash
#!/bin/bash
set -e

echo "🚀 Iniciando deploy da FormEngine API..."

# Carregar variáveis de ambiente
export $(cat .env.prod | xargs)

# Fazer backup do banco (opcional)
echo "📦 Fazendo backup do banco..."
docker exec formengine_mysql_prod mysqldump -u root -p$MYSQL_ROOT_PASSWORD formengine_db > backup_$(date +%Y%m%d_%H%M%S).sql

# Atualizar código
echo "📥 Atualizando código..."
git pull origin main

# Reconstruir e reiniciar
echo "🔄 Reconstruindo aplicação..."
docker-compose -f docker-compose.prod.yml down
docker-compose -f docker-compose.prod.yml up -d --build

# Verificar saúde
echo "🔍 Verificando saúde da aplicação..."
sleep 30
curl -f http://localhost:5000/health || echo "⚠️ Health check falhou"

echo "✅ Deploy concluído!"
```

### 6.2 Script de monitoramento
```bash
nano monitor.sh
chmod +x monitor.sh
```

**Conteúdo do script:**
```bash
#!/bin/bash

echo "📊 Status dos containers:"
docker-compose -f docker-compose.prod.yml ps

echo -e "\n🔍 Logs da API (últimas 20 linhas):"
docker-compose -f docker-compose.prod.yml logs --tail=20 api

echo -e "\n🌐 Teste de conectividade:"
curl -s -o /dev/null -w "Status: %{http_code}\nTempo: %{time_total}s\n" https://formsmenuapi.gabrielsanztech.com.br/health
```

## 🔍 Passo 7: Verificação Final

### 7.1 Testar endpoints
```bash
# Health check
curl https://formsmenuapi.gabrielsanztech.com.br/health

# Swagger
curl https://formsmenuapi.gabrielsanztech.com.br/swagger/index.html
```

### 7.2 Verificar SSL
```bash
openssl s_client -connect formsmenuapi.gabrielsanztech.com.br:443 -servername formsmenuapi.gabrielsanztech.com.br
```

## 🛡️ Passo 8: Segurança Adicional

### 8.1 Configurar Firewall
```bash
sudo ufw enable
sudo ufw allow ssh
sudo ufw allow 'Nginx Full'
sudo ufw status
```

### 8.2 Configurar fail2ban (opcional)
```bash
sudo apt install fail2ban -y
sudo systemctl enable fail2ban
sudo systemctl start fail2ban
```

## 📱 URLs Finais

Após o deploy, sua aplicação estará disponível em:

- **API**: https://formsmenuapi.gabrielsanztech.com.br
- **Swagger**: https://formsmenuapi.gabrielsanztech.com.br/swagger
- **Health**: https://formsmenuapi.gabrielsanztech.com.br/health

## 🔄 Comandos Úteis para Manutenção

```bash
# Ver logs
docker-compose -f docker-compose.prod.yml logs -f api

# Reiniciar apenas a API
docker-compose -f docker-compose.prod.yml restart api

# Fazer backup do banco
docker exec formengine_mysql_prod mysqldump -u root -p$MYSQL_ROOT_PASSWORD formengine_db > backup.sql

# Ver status dos containers
docker-compose -f docker-compose.prod.yml ps

# Atualizar aplicação
./deploy.sh
```

## 🆘 Troubleshooting

### Problema: SSL não funciona
```bash
sudo certbot certificates
sudo certbot renew --force-renewal -d formsmenuapi.gabrielsanztech.com.br
```

### Problema: API não responde
```bash
docker-compose -f docker-compose.prod.yml logs api
curl http://localhost:5000/health
```

### Problema: Banco não conecta
```bash
docker-compose -f docker-compose.prod.yml logs db
docker exec -it formengine_mysql_prod mysql -u root -p
```

---

**🎉 Pronto! Sua FormEngine API estará rodando em produção com HTTPS!**