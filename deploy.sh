#!/bin/bash
set -e

echo "🚀 Iniciando deploy da FormEngine API..."
echo "Domínio: formsmenuapi.gabrielsanztech.com.br"
echo "Data: $(date)"

# Verificar se estamos no diretório correto
if [ ! -f "docker-compose.prod.yml" ]; then
    echo "❌ Erro: arquivo docker-compose.prod.yml não encontrado!"
    echo "Execute este script no diretório raiz do projeto."
    exit 1
fi

# Verificar se o arquivo .env.prod existe
if [ ! -f ".env.prod" ]; then
    echo "❌ Erro: arquivo .env.prod não encontrado!"
    echo "Crie o arquivo .env.prod com as variáveis de ambiente necessárias."
    exit 1
fi

# Carregar variáveis de ambiente
echo "📋 Carregando variáveis de ambiente..."
export $(cat .env.prod | grep -v '^#' | xargs)

# Fazer backup do banco (se existir)
if docker container inspect formengine_mysql_prod >/dev/null 2>&1; then
    echo "📦 Fazendo backup do banco de dados..."
    mkdir -p backups
    docker exec formengine_mysql_prod mysqldump -u root -p$MYSQL_ROOT_PASSWORD formengine_db > backups/backup_$(date +%Y%m%d_%H%M%S).sql
    echo "✅ Backup criado em backups/"
fi

# Atualizar código (se estiver em um repositório git)
if [ -d ".git" ]; then
    echo "📥 Atualizando código do repositório..."
    git pull origin main
else
    echo "⚠️ Não é um repositório git, pulando atualização..."
fi

# Parar containers antigos
echo "🛑 Parando containers antigos..."
docker-compose -f docker-compose.prod.yml down

# Construir e iniciar novos containers
echo "🔨 Construindo e iniciando containers..."
docker-compose -f docker-compose.prod.yml up -d --build

# Aguardar inicialização
echo "⏳ Aguardando inicialização da aplicação..."
sleep 30

# Verificar saúde da aplicação
echo "🔍 Verificando saúde da aplicação..."
for i in {1..10}; do
    if curl -f http://localhost:5000/health >/dev/null 2>&1; then
        echo "✅ Aplicação está saudável!"
        break
    else
        echo "⏳ Tentativa $i/10 - Aguardando aplicação responder..."
        sleep 10
    fi
    
    if [ $i -eq 10 ]; then
        echo "❌ Aplicação não está respondendo após 10 tentativas"
        echo "📋 Logs da API:"
        docker-compose -f docker-compose.prod.yml logs --tail=20 api
        exit 1
    fi
done

# Verificar se HTTPS está funcionando
echo "🔒 Verificando HTTPS..."
if curl -f https://formsmenuapi.gabrielsanztech.com.br/health >/dev/null 2>&1; then
    echo "✅ HTTPS está funcionando!"
else
    echo "⚠️ HTTPS não está respondendo - verifique certificado SSL"
fi

# Mostrar status final
echo ""
echo "🎉 Deploy concluído com sucesso!"
echo ""
echo "📊 Status dos containers:"
docker-compose -f docker-compose.prod.yml ps
echo ""
echo "🌐 URLs disponíveis:"
echo "  • API: https://formsmenuapi.gabrielsanztech.com.br"
echo "  • Swagger: https://formsmenuapi.gabrielsanztech.com.br/swagger"
echo "  • Health: https://formsmenuapi.gabrielsanztech.com.br/health"
echo ""
echo "🔧 Comandos úteis:"
echo "  • Ver logs: docker-compose -f docker-compose.prod.yml logs -f api"
echo "  • Reiniciar: docker-compose -f docker-compose.prod.yml restart api"
echo "  • Parar tudo: docker-compose -f docker-compose.prod.yml down"
echo ""
echo "✨ FormEngine API está rodando em produção!"