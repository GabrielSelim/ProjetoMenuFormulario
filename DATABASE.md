# Estrutura do Banco de Dados - FormEngine API

## Diagrama de Relacionamento

```
┌─────────────────┐
│     Users       │
├─────────────────┤
│ Id (PK)         │───┐
│ Name            │   │
│ Email (Unique)  │   │
│ PasswordHash    │   │
│ Role            │   │
│ CreatedAt       │   │
│ UpdatedAt       │   │
└─────────────────┘   │
                      │
                      │ 1:N
                      │
       ┌──────────────┴──────────────┬──────────────┐
       │                             │              │
       │                             │              │
       ▼                             ▼              ▼
┌─────────────────┐       ┌──────────────────┐  ┌─────────────────┐
│ FormSubmissions │       │   ActivityLogs   │  │      Forms      │
├─────────────────┤       ├──────────────────┤  ├─────────────────┤
│ Id (PK)         │       │ Id (PK)          │  │ Id (PK)         │───┐
│ FormId (FK)     │───┐   │ UserId (FK)      │  │ Name            │   │
│ UserId (FK)     │   │   │ Action           │  │ SchemaJson      │   │
│ DataJson        │   │   │ Entity           │  │ RolesAllowed    │   │
│ CreatedAt       │   │   │ EntityId         │  │ Version         │   │
└─────────────────┘   │   │ Timestamp        │  │ CreatedAt       │   │
                      │   │ Details          │  │ UpdatedAt       │   │
                      │   └──────────────────┘  └─────────────────┘   │
                      │                                                │
                      │                                                │
                      │                              1:N               │
                      └────────────────────────────────────────────────┘


┌─────────────────┐
│     Menus       │
├─────────────────┤
│ Id (PK)         │───┐
│ Name            │   │
│ ContentType     │   │ Self-Reference
│ UrlOrPath       │   │ (Parent/Children)
│ RolesAllowed    │   │
│ Order           │   │
│ Icon            │   │
│ ParentId (FK)   │◄──┘
│ CreatedAt       │
│ UpdatedAt       │
└─────────────────┘
```

## Tabelas Detalhadas

### Users
Armazena informações dos usuários do sistema.

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | INT (PK) | Identificador único |
| Name | VARCHAR(200) | Nome do usuário |
| Email | VARCHAR(200) UNIQUE | Email (login) |
| PasswordHash | TEXT | Hash BCrypt da senha |
| Role | VARCHAR(50) | Papel: admin, gestor, user |
| CreatedAt | DATETIME | Data de criação |
| UpdatedAt | DATETIME | Data de atualização |

**Índices:**
- PRIMARY KEY (Id)
- UNIQUE INDEX (Email)

**Usuário Padrão (Seed):**
- Email: admin@formengine.com
- Senha: Admin@123
- Role: admin

---

### Forms
Armazena a definição dos formulários criados com FormEngine.

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | INT (PK) | Identificador único |
| Name | VARCHAR(200) | Nome do formulário |
| SchemaJson | TEXT | JSON com estrutura do formulário |
| RolesAllowed | VARCHAR(500) | JSON array de roles permitidas |
| Version | VARCHAR(50) | Versão do formulário |
| CreatedAt | DATETIME | Data de criação |
| UpdatedAt | DATETIME | Data de atualização |

**Índices:**
- PRIMARY KEY (Id)

**Exemplo de SchemaJson:**
```json
{
  "fields": [
    {
      "type": "text",
      "label": "Nome",
      "required": true
    },
    {
      "type": "email",
      "label": "Email",
      "required": true
    }
  ]
}
```

---

### FormSubmissions
Armazena as submissões (respostas) dos formulários.

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | INT (PK) | Identificador único |
| FormId | INT (FK) | Referência ao formulário |
| UserId | INT (FK) | Usuário que submeteu |
| DataJson | TEXT | JSON com dados preenchidos |
| CreatedAt | DATETIME | Data da submissão |

**Relacionamentos:**
- FormId → Forms.Id (CASCADE)
- UserId → Users.Id (CASCADE)

**Índices:**
- PRIMARY KEY (Id)
- FOREIGN KEY (FormId)
- FOREIGN KEY (UserId)

**Exemplo de DataJson:**
```json
{
  "nome": "João Silva",
  "email": "joao@example.com",
  "telefone": "11999999999"
}
```

---

### Menus
Armazena a estrutura de menus do sistema com suporte a hierarquia.

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | INT (PK) | Identificador único |
| Name | VARCHAR(200) | Nome do menu |
| ContentType | VARCHAR(50) | Tipo: form, page, link, etc. |
| UrlOrPath | VARCHAR(500) | URL ou caminho |
| RolesAllowed | VARCHAR(500) | JSON array de roles |
| Order | INT | Ordem de exibição |
| Icon | VARCHAR(100) | Nome do ícone |
| ParentId | INT (FK) NULLABLE | Menu pai (null = menu raiz) |
| CreatedAt | DATETIME | Data de criação |
| UpdatedAt | DATETIME | Data de atualização |

**Relacionamentos:**
- ParentId → Menus.Id (RESTRICT)

**Índices:**
- PRIMARY KEY (Id)
- FOREIGN KEY (ParentId)

**Estrutura Hierárquica:**
```
Dashboard (id=1, ParentId=null)
  ├─ Visão Geral (id=2, ParentId=1)
  └─ Relatórios (id=3, ParentId=1)
Formulários (id=4, ParentId=null)
  ├─ Criar Novo (id=5, ParentId=4)
  └─ Listar Todos (id=6, ParentId=4)
```

---

### ActivityLogs
Registra todas as ações importantes do sistema para auditoria.

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | INT (PK) | Identificador único |
| UserId | INT (FK) | Usuário que executou a ação |
| Action | VARCHAR(50) | Tipo: CREATE, UPDATE, DELETE, VIEW, LOGIN |
| Entity | VARCHAR(100) | Entidade afetada: User, Form, Menu, etc. |
| EntityId | INT | ID da entidade afetada |
| Timestamp | DATETIME | Data/hora da ação |
| Details | TEXT | JSON com detalhes adicionais |

**Relacionamentos:**
- UserId → Users.Id (CASCADE)

**Índices:**
- PRIMARY KEY (Id)
- FOREIGN KEY (UserId)
- INDEX (Entity, EntityId)
- INDEX (Timestamp)

**Exemplo de Details:**
```json
{
  "description": "Formulário 'Cadastro de Cliente' criado",
  "ip_address": "192.168.1.10",
  "user_agent": "Mozilla/5.0..."
}
```

---

## Queries Úteis

### Ver todos os formulários com contagem de submissões
```sql
SELECT 
    f.Id,
    f.Name,
    f.Version,
    COUNT(fs.Id) as TotalSubmissions
FROM Forms f
LEFT JOIN FormSubmissions fs ON f.Id = fs.FormId
GROUP BY f.Id, f.Name, f.Version
ORDER BY f.CreatedAt DESC;
```

### Ver menus hierárquicos
```sql
SELECT 
    parent.Id as ParentId,
    parent.Name as ParentName,
    child.Id as ChildId,
    child.Name as ChildName,
    child.Order
FROM Menus parent
LEFT JOIN Menus child ON parent.Id = child.ParentId
WHERE parent.ParentId IS NULL
ORDER BY parent.Order, child.Order;
```

### Ver atividades recentes de um usuário
```sql
SELECT 
    al.Timestamp,
    al.Action,
    al.Entity,
    al.EntityId,
    al.Details,
    u.Name as UserName
FROM ActivityLogs al
INNER JOIN Users u ON al.UserId = u.Id
WHERE al.UserId = 1
ORDER BY al.Timestamp DESC
LIMIT 20;
```

### Ver submissões de um formulário com dados do usuário
```sql
SELECT 
    fs.Id,
    fs.CreatedAt,
    u.Name as UserName,
    u.Email as UserEmail,
    f.Name as FormName,
    fs.DataJson
FROM FormSubmissions fs
INNER JOIN Users u ON fs.UserId = u.Id
INNER JOIN Forms f ON fs.FormId = f.Id
WHERE fs.FormId = 1
ORDER BY fs.CreatedAt DESC;
```

### Estatísticas gerais
```sql
SELECT 
    (SELECT COUNT(*) FROM Users) as TotalUsers,
    (SELECT COUNT(*) FROM Forms) as TotalForms,
    (SELECT COUNT(*) FROM FormSubmissions) as TotalSubmissions,
    (SELECT COUNT(*) FROM Menus) as TotalMenus,
    (SELECT COUNT(*) FROM ActivityLogs) as TotalLogs;
```

---

## Considerações de Performance

1. **Índices**: Todas as foreign keys têm índices automáticos
2. **SchemaJson e DataJson**: Campos TEXT para flexibilidade do FormEngine
3. **Cascade Delete**: FormSubmissions são deletadas ao deletar Form ou User
4. **Restrict Delete**: Menus não podem ser deletados se tiverem filhos

## Migrations

Para criar o banco de dados:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Para reverter:
```bash
dotnet ef database update 0
dotnet ef migrations remove
```
