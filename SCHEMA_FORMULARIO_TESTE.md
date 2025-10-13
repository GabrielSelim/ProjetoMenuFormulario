# 📝 FORMULÁRIO DE TESTE - JSON LEGÍVEL

## Schema do Formulário (para referência)

**Versão Legível do SchemaJson:**
```json
{
  "components": [
    {
      "type": "textfield",
      "key": "solicitante",
      "label": "Nome do Solicitante",
      "validate": {
        "required": true
      }
    },
    {
      "type": "textarea",
      "key": "descricao",
      "label": "Descrição da Solicitação",
      "validate": {
        "required": true
      }
    },
    {
      "type": "select",
      "key": "tipo",
      "label": "Tipo de Solicitação",
      "data": {
        "values": [
          {"label": "Compra", "value": "compra"},
          {"label": "Viagem", "value": "viagem"},
          {"label": "Treinamento", "value": "treinamento"}
        ]
      }
    },
    {
      "type": "currency",
      "key": "valor",
      "label": "Valor (R$)"
    }
  ]
}
```

## JSON para POST /api/Forms (Use este no Swagger)

**Versão para usar no Swagger:**
```json
{
  "name": "Formulário de Solicitação",
  "schemaJson": "{\"components\":[{\"type\":\"textfield\",\"key\":\"solicitante\",\"label\":\"Nome do Solicitante\",\"validate\":{\"required\":true}},{\"type\":\"textarea\",\"key\":\"descricao\",\"label\":\"Descrição da Solicitação\",\"validate\":{\"required\":true}},{\"type\":\"select\",\"key\":\"tipo\",\"label\":\"Tipo de Solicitação\",\"data\":{\"values\":[{\"label\":\"Compra\",\"value\":\"compra\"},{\"label\":\"Viagem\",\"value\":\"viagem\"},{\"label\":\"Treinamento\",\"value\":\"treinamento\"}]}},{\"type\":\"currency\",\"key\":\"valor\",\"label\":\"Valor (R$)\"}]}",
  "rolesAllowed": "user,gestor,admin",
  "version": "1.0.0"
}
```

## Alternativa: Schema Simples para Teste

**Versão mais simples (se preferir):**
```json
{
  "name": "Formulário Simples",
  "schemaJson": "{\"components\":[{\"type\":\"textfield\",\"key\":\"nome\",\"label\":\"Nome\"},{\"type\":\"textarea\",\"key\":\"descricao\",\"label\":\"Descrição\"}]}",
  "rolesAllowed": "user,gestor,admin",
  "version": "1.0.0"
}
```