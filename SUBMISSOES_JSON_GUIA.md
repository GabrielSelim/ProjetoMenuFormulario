# 📋 SUBMISSÕES - JSONs LEGÍVEIS E PARA SWAGGER

## 🔍 Estrutura dos DTOs

### CriarSubmissaoDto
- `formId` (int): ID do formulário
- `dataJson` (string): Dados em formato JSON string
- `comentarioInicial` (string, opcional): Comentário inicial

### AtualizarSubmissaoDto  
- `dataJson` (string): Dados em formato JSON string
- `comentarioAtualizacao` (string, opcional): Comentário da atualização

---

## 📝 CRIAR SUBMISSÃO - POST /api/SubmissoesFormulario

### Dados Legíveis (Para Referência)
```json
{
  "formId": 1,
  "dataJson": {
    "solicitante": "João Silva",
    "descricao": "Solicitação de compra de notebook para desenvolvimento", 
    "tipo": "compra",
    "valor": 3500.00
  },
  "comentarioInicial": "Primeira versão da solicitação"
}
```

### JSON para usar no Swagger
```json
{
  "formId": 1,
  "dataJson": "{\"solicitante\":\"João Silva\",\"descricao\":\"Solicitação de compra de notebook para desenvolvimento\",\"tipo\":\"compra\",\"valor\":3500.00}",
  "comentarioInicial": "Primeira versão da solicitação"
}
```

---

## ✏️ ATUALIZAR SUBMISSÃO - PUT /api/SubmissoesFormulario/{id}

### Dados Legíveis (Para Referência)
```json
{
  "dataJson": {
    "solicitante": "João Silva Santos",
    "descricao": "Solicitação de compra de notebook Dell Inspiron para desenvolvimento de software",
    "tipo": "compra", 
    "valor": 4200.00
  },
  "comentario": "Valores atualizados conforme cotação mais recente",
  "versao": 1
}
```

### JSON para usar no Swagger
```json
{
  "dataJson": "{\"solicitante\":\"João Silva Santos\",\"descricao\":\"Solicitação de compra de notebook Dell Inspiron para desenvolvimento de software\",\"tipo\":\"compra\",\"valor\":4200.00}",
  "comentario": "Valores atualizados conforme cotação mais recente",
  "versao": 1
}
```

**⚠️ OBRIGATÓRIO**: O campo `versao` deve ser obtido via GET da submissão antes de atualizar

---

## 🎯 EXEMPLOS ADICIONAIS

### Submissão de Viagem - JSON para Swagger
```json
{
  "formId": 1,
  "dataJson": "{\"solicitante\":\"Maria Santos\",\"descricao\":\"Viagem para treinamento em São Paulo\",\"tipo\":\"viagem\",\"valor\":2800.00}",
  "comentarioInicial": "Treinamento obrigatório para certificação"
}
```

### Submissão de Treinamento - JSON para Swagger  
```json
{
  "formId": 1,
  "dataJson": "{\"solicitante\":\"Pedro Costa\",\"descricao\":\"Curso de especialização em DevOps\",\"tipo\":\"treinamento\",\"valor\":1500.00}",
  "comentarioInicial": "Curso online com certificação internacional"
}
```

### Submissão Simples - Para Teste Rápido
```json
{
  "formId": 1,
  "dataJson": "{\"nome\":\"Teste\",\"descricao\":\"Teste simples\"}",
  "comentarioInicial": "Teste do sistema"
}
```

---

## 🔧 DICAS IMPORTANTES

1. **DataJson sempre como string**: Nunca como objeto JSON
2. **Escape de aspas**: Use `\"` dentro da string JSON
3. **Validação**: Certifique-se que o JSON dentro da string está válido
4. **Comentários**: Use os campos corretos (`comentarioInicial`, `comentario`)
5. **FormId**: Certifique-se que o formulário existe (criar primeiro)
6. **Versão**: SEMPRE obtenha a versão atual via GET antes de atualizar

---

## ⚡ FERRAMENTA PARA CONVERTER JSON

Se você tem um objeto JSON e precisa transformar em string:

**Objeto Original:**
```json
{
  "solicitante": "João Silva",
  "descricao": "Teste",
  "tipo": "compra",
  "valor": 1000.00
}
```

**String JSON (para usar no dataJson):**
```
"{\"solicitante\":\"João Silva\",\"descricao\":\"Teste\",\"tipo\":\"compra\",\"valor\":1000.00}"
```

Use um conversor online ou ferramenta de desenvolvimento para fazer essa conversão automaticamente.

---

## 🔄 **CONTROLE DE CONCORRÊNCIA**

### ❗ **PROBLEMA COMUM**: "Conflito de versão"

**Erro**:
```json
{
  "sucesso": false,
  "mensagem": "Conflito de versão",
  "erros": ["A submissão foi modificada por outro usuário"]
}
```

### ✅ **SOLUÇÃO**: Fluxo Correto

1. **ANTES de atualizar**, sempre faça GET da submissão:
   ```
   GET /api/SubmissoesFormulario/{id}
   ```

2. **Copie o campo `versao`** da resposta:
   ```json
   {
     "id": 1,
     "versao": 1,  ← COPIE ESTE VALOR
     ...
   }
   ```

3. **Use a versão no PUT**:
   ```json
   {
     "dataJson": "...",
     "comentario": "...",
     "versao": 1     ← USE O VALOR COPIADO
   }
   ```

### 🔄 **Como Funciona**

- Cada modificação **incrementa a versão**
- Se outro usuário modificou enquanto você editava, a versão muda
- O sistema **rejeita** atualizações com versão desatualizada
- Isso **previne perda de dados** por edições simultâneas

### 💡 **Dica**
Se receber erro de versão, faça GET novamente para obter a versão atual e tente novamente.