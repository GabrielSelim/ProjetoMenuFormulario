# 🚀 INÍCIO RÁPIDO - FormEngine API

## ⚡ Start em 3 Passos

### 1️⃣ Inicie com Docker
```powershell
docker-compose up -d
```

### 2️⃣ Acesse o Swagger
Abra no navegador: **http://localhost:5000**

### 3️⃣ Faça Login
Use estas credenciais no endpoint `/api/auth/login`:
```json
{
  "email": "admin@formengine.com",
  "password": "Admin@123"
}
```

---

## 🎯 Teste Rápido Completo

Execute o script de teste automatizado:
```powershell
.\test-api.ps1
```

Este script irá:
- ✅ Fazer login
- ✅ Criar um formulário
- ✅ Criar uma submissão
- ✅ Criar um menu
- ✅ Verificar todas as APIs

---

## 📖 Documentação

- **Visão Geral**: `README.md`
- **Banco de Dados**: `DATABASE.md`
- **Deploy**: `DEPLOY.md`
- **Front-end**: `FRONTEND_INTEGRATION.md`
- **Comandos**: `COMMANDS.md`
- **Status Completo**: `PROJETO_COMPLETO.md`

---

## 🔧 Comandos Essenciais

```powershell
# Iniciar
docker-compose up -d

# Ver logs
docker-compose logs -f api

# Parar
docker-compose down

# Rebuild
docker-compose up -d --build

# Desenvolvimento local
dotnet run
```

---

## 📡 URLs Principais

- **API Base**: http://localhost:5000/api
- **Swagger**: http://localhost:5000
- **MySQL**: localhost:3306

---

## 🎨 Estrutura de Roles

- **admin** - Acesso total
- **gestor** - Criar/editar formulários e menus
- **user** - Submeter formulários

---

## 💡 Exemplo Rápido de Uso

### 1. Login
```bash
POST /api/auth/login
{
  "email": "admin@formengine.com",
  "password": "Admin@123"
}
```
**Resposta**: Token JWT

### 2. Criar Formulário
```bash
POST /api/forms
Authorization: Bearer {token}
{
  "name": "Meu Formulário",
  "schemaJson": "{\"fields\":[{\"type\":\"text\",\"label\":\"Nome\"}]}",
  "rolesAllowed": "[\"admin\",\"user\"]",
  "version": "1.0.0"
}
```

### 3. Submeter Formulário
```bash
POST /api/submissions
Authorization: Bearer {token}
{
  "formId": 1,
  "dataJson": "{\"nome\":\"João Silva\"}"
}
```

---

## ❓ Problemas?

1. **API não inicia**: Verifique se a porta 5000 está livre
2. **Erro de banco**: Execute `docker-compose down -v` e tente novamente
3. **Token inválido**: Faça login novamente

---

## 📦 Importar no Postman

Use o arquivo: `FormEngineAPI.postman_collection.json`

---

**Pronto para começar! 🎉**
