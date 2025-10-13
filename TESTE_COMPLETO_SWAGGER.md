# 🧪 TESTE COMPLETO - WORKFLOW DE SUBMISSÕES VIA SWAGGER

## 📋 **PRÉ-REQUISITOS**

1. **API rodando**: `dotnet run --project FormEngineAPI.csproj`
2. **Swagger aberto**: http://localhost:5187/swagger
3. **Database**: MySQL rodando na porta 3309

## 🔐 **PASSO 1: AUTENTICAÇÃO**

### 1.1 Registrar Usuários de Teste

**Endpoint**: `POST /api/Auth/register`

**Admin**:
```json
{
  "name": "Admin Teste",
  "email": "admin@teste.com",
  "password": "Admin123!",
  "role": "admin"
}
```

**Usuário Comum**:
```json
{
  "name": "Usuario Teste",
  "email": "user@teste.com", 
  "password": "User123!",
  "role": "user"
}
```

**Gestor**:
```json
{
  "name": "Gestor Teste",
  "email": "gestor@teste.com",
  "password": "Gestor123!",
  "role": "gestor"
}
```

**⚠️ IMPORTANTE**: Use roles corretas do sistema: `admin`, `gestor`, `user` (minúsculas)

### 1.2 Fazer Login e Obter Tokens

**Endpoint**: `POST /api/Auth/login`

**Login Admin**:
```json
{
  "email": "admin@teste.com",
  "password": "Admin123!"
}
```

**⚠️ IMPORTANTE**: Copie o `token` retornado e clique em **"Authorize"** no topo do Swagger, cole o token no formato: `Bearer SEU_TOKEN_AQUI`

---

## 📝 **PASSO 2: CRIAR FORMULÁRIO (Pré-requisito)**

### 2.1 Criar Menu
**Endpoint**: `POST /api/Menus`
```json
{
  "name": "Formulários de Teste",
  "contentType": "application/json",
  "urlOrPath": "/formularios",
  "rolesAllowed": "user,gestor,admin",
  "order": 1,
  "icon": "form",
  "parentId": null
}
```

**⚠️ IMPORTANTE**: Para menu raiz (sem pai), use `"parentId": null` em vez de `0`.

### 2.2 Criar Formulário
**Endpoint**: `POST /api/Forms`
```json
{
  "name": "Formulário de Solicitação",
  "schemaJson": "{\"components\":[{\"type\":\"textfield\",\"key\":\"solicitante\",\"label\":\"Nome do Solicitante\",\"validate\":{\"required\":true}},{\"type\":\"textarea\",\"key\":\"descricao\",\"label\":\"Descrição da Solicitação\",\"validate\":{\"required\":true}},{\"type\":\"select\",\"key\":\"tipo\",\"label\":\"Tipo de Solicitação\",\"data\":{\"values\":[{\"label\":\"Compra\",\"value\":\"compra\"},{\"label\":\"Viagem\",\"value\":\"viagem\"},{\"label\":\"Treinamento\",\"value\":\"treinamento\"}]}},{\"type\":\"currency\",\"key\":\"valor\",\"label\":\"Valor (R$)\"}]}",
  "rolesAllowed": "user,gestor,admin",
  "version": "1.0.0"
}
```

**⚠️ IMPORTANTE**: 
- Use `"name"` em vez de `"title"`
- Use `"schemaJson"` como string JSON (não objeto)
- Use `"rolesAllowed"` em vez de roles separadas

---

## 🚀 **PASSO 3: TESTE DO WORKFLOW COMPLETO**

### 3.1 RASCUNHO - Criar Submissão
**Endpoint**: `POST /api/SubmissoesFormulario`

```json
{
  "formId": 1,
  "dataJson": "{\"solicitante\":\"João Silva\",\"descricao\":\"Solicitação de compra de notebook para desenvolvimento\",\"tipo\":\"compra\",\"valor\":3500.00}",
  "comentarioInicial": "Primeira versão da solicitação"
}
```

**⚠️ IMPORTANTE**: 
- `dataJson` deve ser uma **string JSON**, não um objeto
- Use `comentarioInicial` em vez de `observacoes`

**✅ Resultado Esperado**: Status = `Rascunho` (0)

### 3.2 Obter Versão Atual (Antes de Atualizar)
**Endpoint**: `GET /api/SubmissoesFormulario/{id}`

**⚠️ IMPORTANTE**: Anote o campo `versao` retornado para usar na atualização

### 3.3 Atualizar Rascunho
**Endpoint**: `PUT /api/SubmissoesFormulario/{id}`

```json
{
  "dataJson": "{\"solicitante\":\"João Silva Santos\",\"descricao\":\"Solicitação de compra de notebook Dell Inspiron para desenvolvimento de software\",\"tipo\":\"compra\",\"valor\":4200.00}",
  "comentario": "Valores atualizados conforme cotação mais recente",
  "versao": 1
}
```

**⚠️ IMPORTANTE**: 
- `dataJson` como **string JSON**
- Use `comentario` para comentários
- **`versao`** é obrigatório (obtido no GET da submissão)

### 3.4 ENVIAR - Submeter para Análise
**Endpoint**: `POST /api/SubmissoesFormulario/{id}/enviar`

```json
{
  "observacoes": "Submissão final para aprovação"
}
```

**✅ Resultado Esperado**: Status = `Enviado` (1)

---

## 👥 **PASSO 4: TROCAR DE USUÁRIO (GESTOR/ADMIN)**

### 4.1 Fazer Logout e Login como Gestor
1. **Clique em "Authorize"** e remova o token atual
2. **Faça login** com as credenciais do Gestor:
```json
{
  "email": "gestor@teste.com",
  "password": "Gestor123!"
}
```
3. **Autorize novamente** com o novo token

### 4.2 ANÁLISE - Colocar em Análise
**Endpoint**: `POST /api/SubmissoesFormulario/{id}/colocar-analise`

```json
{
  "observacoes": "Iniciando análise da solicitação de compra"
}
```

**✅ Resultado Esperado**: Status = `EmAnalise` (2)

---

## ✅ **PASSO 5: APROVAÇÃO/REJEIÇÃO**

### 5.1 APROVAR Submissão
**Endpoint**: `POST /api/SubmissoesFormulario/{id}/aprovar`

```json
{
  "observacoes": "Aprovado conforme política de compras da empresa"
}
```

**✅ Resultado Esperado**: Status = `Aprovado` (3)

### 5.2 OU REJEITAR (Teste Alternativo)
**Endpoint**: `POST /api/SubmissoesFormulario/{id}/rejeitar`

```json
{
  "motivo": "Valor acima do limite aprovado para o setor",
  "observacoes": "Solicitar nova cotação com valores menores"
}
```

**✅ Resultado Esperado**: Status = `Rejeitado` (4)

---

## 📊 **PASSO 6: CONSULTAS E RELATÓRIOS**

### 6.1 Listar Todas as Submissões
**Endpoint**: `GET /api/SubmissoesFormulario`

**Parâmetros de teste**:
- `pagina`: 1
- `tamanhoPagina`: 10
- `status`: (deixe em branco para ver todos)

### 6.2 Filtrar por Status
**Endpoint**: `GET /api/SubmissoesFormulario`

**Parâmetros**:
- `status`: 3 (para ver apenas aprovados)
- `dataInicio`: 2024-01-01
- `dataFim`: 2024-12-31

### 6.3 Ver Detalhes Completos
**Endpoint**: `GET /api/SubmissoesFormulario/{id}`

### 6.4 Ver Histórico de Ações
**Endpoint**: `GET /api/SubmissoesFormulario/{id}/historico`

### 6.5 Estatísticas
**Endpoint**: `GET /api/SubmissoesFormulario/estatisticas`

---

## 🔍 **PASSO 7: VALIDAÇÕES E CENÁRIOS DE ERRO**

### 7.1 Tentar Aprovar como User (Erro 403)
1. **Troque para usuário comum**
2. **Tente aprovar**: `POST /api/SubmissoesFormulario/{id}/aprovar`
3. **✅ Resultado Esperado**: Erro 403 Forbidden

### 7.2 Tentar Editar Submissão Enviada (Erro 400)
1. **Como usuário comum**
2. **Tente editar submissão com status "Enviado"**
3. **✅ Resultado Esperado**: Erro 400 Bad Request

### 7.3 Workflow Inválido (Erro 400)
1. **Tente aprovar submissão em "Rascunho"** (sem enviar antes)
2. **✅ Resultado Esperado**: Erro 400 Bad Request

---

## 📝 **CHECKLIST DE TESTE**

### Funcionalidades Básicas
- [ ] ✅ Registro de usuários (admin, gestor, user)
- [ ] ✅ Login e obtenção de tokens JWT
- [ ] ✅ Criação de formulário
- [ ] ✅ Criar submissão (Rascunho)
- [ ] ✅ Editar submissão em rascunho
- [ ] ✅ Enviar submissão para análise

### Workflow de Aprovação
- [ ] ✅ Colocar submissão em análise (gestor/admin)
- [ ] ✅ Aprovar submissão (gestor/admin)
- [ ] ✅ Rejeitar submissão (gestor/admin)
- [ ] ✅ Cancelar submissão (user/admin)

### Consultas e Relatórios
- [ ] ✅ Listar submissões com paginação
- [ ] ✅ Filtrar por status e datas
- [ ] ✅ Ver detalhes completos da submissão
- [ ] ✅ Consultar histórico de ações
- [ ] ✅ Obter estatísticas

### Validações de Segurança
- [ ] ✅ Permissões por role funcionando
- [ ] ✅ Usuário comum não pode aprovar
- [ ] ✅ Não pode editar submissão enviada
- [ ] ✅ Workflow states validados

### Auditoria
- [ ] ✅ Histórico sendo criado automaticamente
- [ ] ✅ IP e User-Agent sendo capturados
- [ ] ✅ Timestamps corretos
- [ ] ✅ Usuário responsável registrado

---

## 🎯 **DADOS DE EXEMPLO PARA TESTES**

### Submissão de Viagem
```json
{
  "formId": 1,
  "dataJson": "{\"solicitante\":\"Maria Santos\",\"descricao\":\"Viagem para treinamento em São Paulo\",\"tipo\":\"viagem\",\"valor\":2800.00}",
  "comentarioInicial": "Treinamento obrigatório para certificação"
}
```

### Submissão de Treinamento
```json
{
  "formId": 1,
  "dataJson": "{\"solicitante\":\"Pedro Costa\",\"descricao\":\"Curso de especialização em DevOps\",\"tipo\":\"treinamento\",\"valor\":1500.00}",
  "comentarioInicial": "Curso online com certificação internacional"
}
```

---

## 🔧 **TROUBLESHOOTING**

### Erro 401 Unauthorized
- ✅ Verifique se o token JWT está válido
- ✅ Clique em "Authorize" e cole o token no formato: `Bearer SEU_TOKEN`

### Erro 403 Forbidden
- ✅ Verifique se o usuário tem a role necessária
- ✅ Apenas admin/gestor podem aprovar submissões

### Erro 400 Bad Request
- ✅ Verifique se o JSON está bem formado
- ✅ Verifique se o workflow state é válido

### Erro 500 Internal Server Error
- ✅ Verifique os logs no terminal onde a API está rodando
- ✅ Verifique se o banco de dados está conectado

---

## 🚀 **RESULTADO ESPERADO**

Ao final dos testes, você deve ter:

1. **Usuários criados** com diferentes roles
2. **Submissões em todos os status** (Rascunho, Enviado, EmAnalise, Aprovado, Rejeitado)
3. **Histórico completo** de todas as ações
4. **Validações funcionando** (permissões, workflow states)
5. **Relatórios e estatísticas** funcionais

**🎉 Sistema 100% testado e operacional!**