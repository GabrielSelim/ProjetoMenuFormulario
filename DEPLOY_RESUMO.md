# 🚀 Resumo Executivo - Deploy FormEngine API

## ⚡ Comandos Principais

### No seu servidor (via SSH):

```bash
# 1. Clonar o projeto
cd /opt
sudo git clone https://github.com/GabrielSelim/ProjetoMenuFormulario.git
sudo chown -R $USER:$USER /opt/ProjetoMenuFormulario
cd /opt/ProjetoMenuFormulario

# 2. Configurar ambiente
cp .env.prod.example .env.prod
nano .env.prod  # Editar com suas senhas seguras

# 3. Dar permissão aos scripts
chmod +x deploy.sh monitor.sh

# 4. Fazer deploy
./deploy.sh
```

## 🔧 Pré-requisitos do Servidor

```bash
# Instalar Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER

# Instalar Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Instalar Nginx
sudo apt update && sudo apt install nginx -y

# Instalar Certbot
sudo apt install certbot python3-certbot-nginx -y
```

## 🌐 Configurar Nginx

```bash
# Criar configuração
sudo nano /etc/nginx/sites-available/formsmenuapi.gabrielsanztech.com.br
```

Cole a configuração do arquivo `DEPLOY_PRODUCTION.md`

```bash
# Ativar site
sudo ln -s /etc/nginx/sites-available/formsmenuapi.gabrielsanztech.com.br /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

## 🔒 Configurar SSL

```bash
# Obter certificado
sudo certbot --nginx -d formsmenuapi.gabrielsanztech.com.br
```

## ✅ URLs Finais

- **API**: https://formsmenuapi.gabrielsanztech.com.br
- **Swagger**: https://formsmenuapi.gabrielsanztech.com.br/swagger  
- **Health**: https://formsmenuapi.gabrielsanztech.com.br/health

## 🔄 Comandos de Manutenção

```bash
# Deploy/Atualizar
./deploy.sh

# Monitorar
./monitor.sh

# Ver logs
docker-compose -f docker-compose.prod.yml logs -f api

# Reiniciar apenas API
docker-compose -f docker-compose.prod.yml restart api

# Parar tudo
docker-compose -f docker-compose.prod.yml down
```

## 🆘 Troubleshooting Rápido

```bash
# Verificar containers
docker ps

# Verificar logs de erro
docker-compose -f docker-compose.prod.yml logs api | grep -i error

# Testar conectividade local
curl http://localhost:5000/health

# Renovar SSL
sudo certbot renew --force-renewal

# Reiniciar Nginx
sudo systemctl restart nginx
```

## 📋 Checklist de Deploy

- [ ] Servidor com Docker instalado
- [ ] Projeto clonado em `/opt/ProjetoMenuFormulario`
- [ ] Arquivo `.env.prod` configurado com senhas seguras
- [ ] Nginx configurado e rodando
- [ ] Certificado SSL obtido via Certbot
- [ ] Script `deploy.sh` executado com sucesso
- [ ] URLs acessíveis via HTTPS
- [ ] Health check retornando status "Healthy"

---

**🎯 Objetivo**: Ter a FormEngine API rodando em produção no domínio `formsmenuapi.gabrielsanztech.com.br` com HTTPS.