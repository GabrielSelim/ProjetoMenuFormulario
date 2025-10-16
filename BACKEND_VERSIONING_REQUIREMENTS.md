# 📋 BACKEND - Sistema de Versionamento de Formulários

## 🎯 **PROBLEMA IDENTIFICADO**

Atualmente, quando um **menu é vinculado a um formulário** (contentType = 'form' e urlOrPath = formId), não temos controle sobre **qual versão** do formulário será aberta. Isso gera os seguintes problemas:

1. **Menu sempre abre a versão atual** - não há seleção de versão específica
2. **Falta rastreamento** - não sabemos qual versão estava vinculada originalmente  
3. **Perda de contexto histórico** - se o formulário evolui, menus antigos podem quebrar

## 🔧 **MODIFICAÇÕES NECESSÁRIAS NO BACKEND**

### 1. **Estrutura de Formulários com Versionamento**

**Atual:**
```json
{
  "id": 1,
  "name": "Formulário de Contato",
  "schema": {...}
}
```

**Nova estrutura proposta:**
```json
{
  "id": 1,
  "originalFormId": 1,      // ID do formulário original
  "name": "Formulário de Contato",
  "version": "2.0",         // Versão atual
  "isLatest": true,         // Flag para identificar versão mais recente
  "schema": {...},
  "createdAt": "2024-01-01",
  "updatedAt": "2024-01-15"
}
```

### 2. **Endpoint para Buscar Versões de um Formulário**

```http
GET /api/forms/{originalFormId}/versions
```

**Resposta:**
```json
[
  {
    "id": 1,
    "originalFormId": 1,
    "version": "1.0",
    "name": "Formulário de Contato",
    "isLatest": false,
    "createdAt": "2024-01-01"
  },
  {
    "id": 5,
    "originalFormId": 1,
    "version": "2.0", 
    "name": "Formulário de Contato",
    "isLatest": true,
    "createdAt": "2024-01-15"
  }
]
```

### 3. **Modificação na Estrutura de Menus**

**Atual:**
```json
{
  "id": 1,
  "name": "Fale Conosco",
  "contentType": "form",
  "urlOrPath": "1"  // Apenas ID do formulário
}
```

**Nova estrutura proposta:**
```json
{
  "id": 1,
  "name": "Fale Conosco",
  "contentType": "form",
  "urlOrPath": "1",           // ID do formulário específico
  "originalFormId": 1,        // ID do formulário original (para buscar versões)
  "formVersion": "2.0",       // Versão específica vinculada
  "useLatestVersion": false   // Flag: sempre usar última versão ou versão específica
}
```

### 4. **Endpoint para Vincular Menu com Versão Específica**

```http
POST/PUT /api/menus
```

**Body:**
```json
{
  "name": "Fale Conosco",
  "contentType": "form",
  "originalFormId": 1,        // Form original
  "formVersion": "2.0",       // Versão específica
  "useLatestVersion": true,   // true = sempre usar última / false = usar versão específica
  "urlOrPath": "5"            // ID da versão específica (se useLatestVersion = false)
}
```

### 5. **Modificação na Criação/Edição de Formulários**

**Quando editar um formulário:**
1. **NÃO sobrescrever** o formulário existente
2. **Criar nova entrada** com:
   - `originalFormId` = ID do formulário original
   - `version` = próxima versão (ex: "2.0", "2.1")
   - `isLatest` = true (e marcar a anterior como false)

**Endpoint modificado:**
```http
PUT /api/forms/{id}
```

**Comportamento novo:**
- Se `createNewVersion = true` no body → cria nova versão
- Se `createNewVersion = false` → sobrescreve versão atual (comportamento atual)

### 6. **Endpoint para Submissões com Versionamento**

```http
POST /api/submissions
```

**Body atual:**
```json
{
  "formId": 1,
  "data": {...}
}
```

**Body proposto:**
```json
{
  "formId": 1,              // ID da versão específica usada
  "originalFormId": 1,      // ID do formulário original  
  "formVersion": "2.0",     // Versão usada na submissão
  "data": {...}
}
```

## 🎯 **FUNCIONALIDADES QUE ISSO HABILITARÁ**

### No Frontend (já implementado):
1. ✅ **FormList** - Dropdown para selecionar versão do formulário
2. ✅ **SubmissionCard** - Exibe versão do formulário usada na submissão
3. ✅ **Agrupamento** - Formulários agrupados por nome com seleção de versão

### Novo no MenuManager:
4. **Seleção de versão** ao vincular formulário ao menu
5. **Opção "sempre usar última versão"** vs versão específica
6. **Histórico de vinculações** - saber qual versão estava vinculada quando

## 📝 **EXEMPLO DE USO PRÁTICO**

### Cenário:
1. **Formulário "Avaliação"** v1.0 vinculado ao menu "Avaliar Serviço"
2. **Formulário evoluiu** para v2.0 (novos campos obrigatórios)
3. **Administrador decide**:
   - **Opção A:** Menu sempre usa última versão (v2.0) → `useLatestVersion = true`
   - **Opção B:** Menu continua com v1.0 específica → `useLatestVersion = false`

### Interface no MenuManager:
```
┌─ Vincular Formulário ──────────────────┐
│ Formulário: [Avaliação ▼]              │
│ Versão: [v2.0 (mais recente) ▼]       │  
│ □ Sempre usar versão mais recente      │
│ ✓ Usar versão específica: v2.0        │
└───────────────────────────────────────┘
```

## 🚀 **PRIORIDADE DE IMPLEMENTAÇÃO**

### **Alta Prioridade:**
1. **Adicionar campos de versionamento** na tabela Forms
   - `originalFormId` (int, nullable para formulários existentes)
   - `version` (string, default "1.0")
   - `isLatest` (boolean, default true)

2. **Modificar endpoint de edição** para criar versões ao invés de sobrescrever
   - Parâmetro `createNewVersion` no PUT /api/forms/{id}
   - Lógica para incrementar versão automaticamente

3. **Endpoint para buscar versões** de um formulário
   - GET /api/forms/{originalFormId}/versions
   - Ordenação por versão/data de criação

### **Média Prioridade:**
4. **Modificar estrutura de Menus** para suportar versionamento
   - `originalFormId` (int, nullable)
   - `formVersion` (string, nullable)
   - `useLatestVersion` (boolean, default true)

5. **Atualizar submissões** para rastrear versão usada
   - `originalFormId` (int)
   - `formVersion` (string)

### **Baixa Prioridade:**
6. **Migração de dados existentes** para nova estrutura
   - Script para setar `originalFormId` = `id` nos formulários existentes
   - Setar `version` = "1.0" e `isLatest` = true

7. **Endpoints de limpeza/arquivamento** de versões antigas
   - Soft delete de versões antigas
   - Compactação de histórico

## 🗃️ **ESTRUTURA DE BANCO SUGERIDA**

### **Tabela Forms (modificada):**
```sql
ALTER TABLE Forms ADD COLUMN originalFormId INT NULL;
ALTER TABLE Forms ADD COLUMN version VARCHAR(10) DEFAULT '1.0';
ALTER TABLE Forms ADD COLUMN isLatest BOOLEAN DEFAULT true;

-- Índices para performance
CREATE INDEX idx_forms_original_id ON Forms(originalFormId);
CREATE INDEX idx_forms_latest ON Forms(originalFormId, isLatest);
```

### **Tabela Menus (modificada):**
```sql
ALTER TABLE Menus ADD COLUMN originalFormId INT NULL;
ALTER TABLE Menus ADD COLUMN formVersion VARCHAR(10) NULL;
ALTER TABLE Menus ADD COLUMN useLatestVersion BOOLEAN DEFAULT true;
```

### **Tabela Submissions (modificada):**
```sql
ALTER TABLE Submissions ADD COLUMN originalFormId INT NULL;
ALTER TABLE Submissions ADD COLUMN formVersion VARCHAR(10) NULL;
```

## ❓ **DÚVIDAS PARA DISCUSSÃO**

1. **Numeração de versões:** Usar "1.0, 2.0" ou "v1, v2" ou timestamp?
2. **Limite de versões:** Quantas versões manter por formulário?
3. **Migração:** Como tratar formulários e menus existentes?
4. **Performance:** Indexação necessária para consultas de versões?
5. **Versionamento automático:** Incremento automático (1.0 → 2.0) ou manual?

## 🔄 **FLUXO DE DESENVOLVIMENTO SUGERIDO**

### **Fase 1: Base de Versionamento**
1. Adicionar campos na tabela Forms
2. Modificar endpoint de criação/edição de formulários
3. Criar endpoint para buscar versões

### **Fase 2: Integração com Menus**
4. Modificar tabela e endpoints de Menus
5. Atualizar frontend do MenuManager para seleção de versão

### **Fase 3: Rastreamento Completo**
6. Modificar tabela e endpoints de Submissions
7. Atualizar serviços de submissão no frontend

### **Fase 4: Migração e Limpeza**
8. Script de migração de dados existentes
9. Testes de integração completos
10. Documentação e cleanup

## 📧 **PRÓXIMOS PASSOS**

1. **Validar estrutura proposta** com a equipe de backend
2. **Definir padrão de versionamento** (1.0, 2.0 vs v1, v2)
3. **Priorizar implementação** por fases
4. **Criar testes unitários** para novos endpoints
5. **Documentar APIs** no Swagger

---

**🎯 Objetivo:** Permitir controle completo de versões de formulários, desde a criação até a vinculação com menus e rastreamento em submissões, mantendo compatibilidade com o sistema atual.