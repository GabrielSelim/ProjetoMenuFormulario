# Teste de Conexão com Banco de Desenvolvimento

Este arquivo documenta como testar a conexão com o banco MySQL local.

## Status do Ambiente

✅ **MySQL Docker**: Rodando na porta 3309
✅ **Database**: formengine_db criado
✅ **Migrations**: Aplicadas com sucesso
✅ **Compilação**: Projeto compila sem erros

## Próximos Passos

1. **Completar implementação do sistema:**
   - [ ] Interface IServicoSubmissaoFormulario
   - [ ] Controller SubmissoesFormularioController  
   - [ ] AutoMapper profiles
   - [ ] Dependency Injection no Program.cs

2. **Testar a API:**
   ```powershell
   # Executar a API
   dotnet run
   
   # Acessar Swagger
   # http://localhost:5000/swagger
   ```

3. **Comandos úteis:**
   ```powershell
   # Ver status dos containers
   docker-compose ps
   
   # Ver logs do MySQL
   docker-compose logs db
   
   # Parar ambiente
   docker-compose down
   
   # Reiniciar ambiente
   .\dev-setup.ps1 restart
   ```

## Configuração Atual

- **MySQL**: localhost:3309
- **API**: localhost:5000 (HTTP) / localhost:5001 (HTTPS)
- **Swagger**: localhost:5000/swagger
- **Database**: formengine_db
- **User**: root / rootpassword

## Estrutura do Banco

### Tabelas Principais:
- `Users` - Usuários do sistema
- `Forms` - Formulários disponíveis
- `FormSubmissions` - Submissões com workflow completo
- `HistoricoFormSubmissions` - Auditoria completa
- `Menus` - Sistema de menus
- `ActivityLogs` - Logs de atividades

### Novos Campos em FormSubmissions:
- `Status` (enum: Rascunho, Enviado, EmAnalise, Aprovado, Rejeitado, Cancelado)
- `DataAtualizacao`, `DataSubmissao`, `DataAprovacao`
- `UsuarioAprovadorId`, `MotivoRejeicao`
- `EnderecoIp`, `UserAgent`
- `Versao` (controle de concorrência)
- `Excluido`, `DataExclusao` (soft delete)

## Próxima Etapa

Execute o script para iniciar o desenvolvimento:

```powershell
# Usar o script automatizado
.\dev-setup.ps1 start

# Ou manualmente:
docker-compose up -d db
dotnet run
```