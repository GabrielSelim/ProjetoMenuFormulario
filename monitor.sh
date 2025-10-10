#!/bin/bash

echo "🔍 FormEngine API - Monitor de Produção"
echo "Domínio: formsmenuapi.gabrielsanztech.com.br"
echo "Data: $(date)"
echo "==============================================="

# Verificar se os containers estão rodando
echo ""
echo "📊 Status dos containers:"
docker-compose -f docker-compose.prod.yml ps

echo ""
echo "🔧 Uso de recursos:"
echo "CPU e Memória dos containers:"
docker stats formengine_api_prod formengine_mysql_prod --no-stream --format "table {{.Name}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}"

echo ""
echo "💾 Uso de disco:"
df -h | grep -E "(Filesystem|/dev/)"

echo ""
echo "🌐 Conectividade:"
echo "Testing HTTP (local):"
if curl -s -o /dev/null -w "Status: %{http_code} | Tempo: %{time_total}s\n" http://localhost:5000/health; then
    echo "✅ HTTP local funcionando"
else
    echo "❌ HTTP local com problemas"
fi

echo ""
echo "Testing HTTPS (público):"
if curl -s -o /dev/null -w "Status: %{http_code} | Tempo: %{time_total}s\n" https://formsmenuapi.gabrielsanztech.com.br/health; then
    echo "✅ HTTPS público funcionando"
else
    echo "❌ HTTPS público com problemas"
fi

echo ""
echo "🔍 Health Check Detalhado:"
curl -s https://formsmenuapi.gabrielsanztech.com.br/health | jq '.' 2>/dev/null || curl -s https://formsmenuapi.gabrielsanztech.com.br/health

echo ""
echo "📋 Logs recentes da API (últimas 10 linhas):"
docker-compose -f docker-compose.prod.yml logs --tail=10 api

echo ""
echo "📋 Logs recentes do MySQL (últimas 5 linhas):"
docker-compose -f docker-compose.prod.yml logs --tail=5 db

echo ""
echo "🔒 Status do certificado SSL:"
echo | openssl s_client -servername formsmenuapi.gabrielsanztech.com.br -connect formsmenuapi.gabrielsanztech.com.br:443 2>/dev/null | openssl x509 -noout -dates

echo ""
echo "🌡️ Status do Nginx:"
sudo systemctl status nginx --no-pager -l

echo ""
echo "==============================================="
echo "✅ Monitoramento concluído!"
echo ""
echo "🚨 Para ver logs em tempo real:"
echo "docker-compose -f docker-compose.prod.yml logs -f api"
echo ""
echo "🔄 Para reiniciar a aplicação:"
echo "./deploy.sh"