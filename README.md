# FormEngine API

API REST completa em ASP.NET Core 8 para gerenciamento de formulários dinâmicos com FormEngine, incluindo autenticação JWT, controle de acesso baseado em roles e auditoria.

## 🚀 Funcionalidades

### Autenticação e Usuários
- ✅ Registro de usuários com hashing de senha (BCrypt)
- ✅ Login com JWT Token
- ✅ Controle de acesso baseado em roles (admin, gestor, user)
- ✅ Endpoints protegidos com autenticação

### Formulários (FormEngine)
- ✅ CRUD completo de formulários
- ✅ Schema JSON para formulários drag-and-drop
- ✅ Controle de acesso por roles
- ✅ Versionamento de formulários
- ✅ Submissões de formulários com dados JSON

### Menus
- ✅ CRUD completo de menus
- ✅ Estrutura hierárquica (menus e submenus)
- ✅ Filtro automático por role do usuário
- ✅ Suporte a ícones e ordem customizada

### Auditoria
- ✅ Log automático de todas as operações (CREATE, UPDATE, DELETE)
- ✅ Rastreamento de usuário, ação, entidade e timestamp
- ✅ Detalhes adicionais em JSON

## 🏗️ Arquitetura

O projeto segue as melhores práticas de desenvolvimento com:

- **Repository Pattern** - Camada de acesso a dados
- **Service Pattern** - Lógica de negócios
- **DTOs** - Data Transfer Objects para APIs
- **AutoMapper** - Mapeamento automático entre entidades e DTOs
- **FluentValidation** - Validação de entrada
- **Entity Framework Core** - ORM com Code-First
- **JWT Authentication** - Autenticação segura
- **Swagger/OpenAPI** - Documentação automática

## 📁 Estrutura do Projeto

```
FormEngineAPI/
├── Controllers/          # API Controllers
│   ├── AuthController.cs
│   ├── FormsController.cs
│   ├── SubmissionsController.cs
│   └── MenusController.cs
├── Models/              # Entidades do banco de dados
│   ├── User.cs
│   ├── Form.cs
│   ├── FormSubmission.cs
│   ├── Menu.cs
│   └── ActivityLog.cs
├── DTOs/                # Data Transfer Objects
│   ├── AuthDtos.cs
│   ├── FormDtos.cs
│   ├── SubmissionDtos.cs
│   └── MenuDtos.cs
├── Services/            # Lógica de negócios
│   ├── AuthService.cs
│   ├── FormService.cs
│   ├── SubmissionService.cs
│   └── MenuService.cs
├── Repositories/        # Acesso a dados
│   ├── IRepository.cs
│   ├── UserRepository.cs
│   ├── FormRepository.cs
│   ├── FormSubmissionRepository.cs
│   ├── MenuRepository.cs
│   └── ActivityLogRepository.cs
├── Validators/          # Validação com FluentValidation
│   ├── AuthValidators.cs
│   ├── FormValidators.cs
│   ├── SubmissionValidators.cs
│   └── MenuValidators.cs
├── Mappings/            # AutoMapper Profiles
│   └── MappingProfile.cs
├── Data/                # DbContext
│   └── ApplicationDbContext.cs
├── Dockerfile
├── docker-compose.yml
└── Program.cs
```

## 🔧 Tecnologias Utilizadas

- **ASP.NET Core 8** - Framework web
- **Entity Framework Core 9** - ORM
- **Pomelo.EntityFrameworkCore.MySql** - Provider MySQL
- **MySQL 8.0** - Banco de dados
- **BCrypt.Net** - Hashing de senhas
- **JWT Bearer Authentication** - Autenticação
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação
- **Swashbuckle (Swagger)** - Documentação da API
- **Docker & Docker Compose** - Containerização

## 🚦 Endpoints da API

### Autenticação
```
POST /api/auth/register  - Registrar novo usuário
POST /api/auth/login     - Login de usuário
```

### Formulários
```
GET    /api/forms       - Listar todos os formulários
GET    /api/forms/{id}  - Buscar formulário por ID
POST   /api/forms       - Criar formulário (admin/gestor)
PUT    /api/forms/{id}  - Atualizar formulário (admin/gestor)
DELETE /api/forms/{id}  - Excluir formulário (admin)
```

### Submissões
```
POST /api/submissions                - Criar submissão
GET  /api/submissions/form/{formId}  - Listar submissões por formulário (admin/gestor)
GET  /api/submissions/user/{userId}  - Listar submissões por usuário
GET  /api/submissions/my-submissions - Minhas submissões
```

### Menus
```
GET    /api/menus       - Listar menus (filtrado por role)
GET    /api/menus/{id}  - Buscar menu por ID
POST   /api/menus       - Criar menu (admin)
PUT    /api/menus/{id}  - Atualizar menu (admin)
DELETE /api/menus/{id}  - Excluir menu (admin)
```

## 🐳 Docker - Instalação e Execução

### Pré-requisitos
- Docker
- Docker Compose

### Executar com Docker Compose

1. Clone o repositório:
```bash
git clone <url-do-repositorio>
cd ProjetoMenuFormulario
```

2. Inicie os containers:
```bash
docker-compose up -d
```

3. A API estará disponível em: `http://localhost:5000`
4. A documentação Swagger em: `http://localhost:5000`

### Parar os containers
```bash
docker-compose down
```

### Reconstruir após mudanças
```bash
docker-compose up -d --build
```

## 💻 Desenvolvimento Local

### Pré-requisitos
- .NET 8 SDK
- MySQL 8.0 (ou use Docker apenas para o banco)

### Configuração

1. Atualize a connection string no `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=formengine_db;User=root;Password=sua-senha;"
}
```

2. Execute as migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

3. Execute a aplicação:
```bash
dotnet run
```

4. Acesse: `http://localhost:5000`

## 📊 Banco de Dados

### Tabelas

- **Users** - Usuários do sistema
- **Forms** - Formulários (schema JSON)
- **FormSubmissions** - Submissões de formulários
- **Menus** - Estrutura de menus
- **ActivityLogs** - Logs de auditoria

### Usuário Padrão (Seed)

```
Email: admin@formengine.com
Senha: Admin@123
Role: admin
```

## 🔐 Autenticação JWT

Para usar endpoints protegidos:

1. Faça login em `/api/auth/login`
2. Copie o token retornado
3. No Swagger, clique em "Authorize" e cole: `Bearer {seu-token}`
4. Ou adicione o header: `Authorization: Bearer {seu-token}`

## 📝 Exemplo de Uso

### 1. Registrar Usuário
```bash
POST /api/auth/register
Content-Type: application/json

{
  "name": "João Silva",
  "email": "joao@example.com",
  "password": "senha123",
  "role": "user"
}
```

### 2. Login
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "joao@example.com",
  "password": "senha123"
}
```

### 3. Criar Formulário
```bash
POST /api/forms
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Cadastro de Cliente",
  "schemaJson": "{\"fields\":[{\"type\":\"text\",\"label\":\"Nome\"}]}",
  "rolesAllowed": "[\"admin\",\"gestor\",\"user\"]",
  "version": "1.0.0"
}
```

### 4. Submeter Formulário
```bash
POST /api/submissions
Authorization: Bearer {token}
Content-Type: application/json

{
  "formId": 1,
  "dataJson": "{\"nome\":\"Maria\",\"email\":\"maria@example.com\"}"
}
```

### 5. Criar Menu
```bash
POST /api/menus
Authorization: Bearer {token-admin}
Content-Type: application/json

{
  "name": "Dashboard",
  "contentType": "page",
  "urlOrPath": "/dashboard",
  "rolesAllowed": "[\"admin\",\"gestor\"]",
  "order": 1,
  "icon": "dashboard"
}
```

## 🔧 Migrations

### Criar nova migration
```bash
dotnet ef migrations add NomeDaMigration
```

### Aplicar migrations
```bash
dotnet ef database update
```

### Reverter migration
```bash
dotnet ef database update PreviousMigrationName
```

## 📦 Pacotes NuGet

- Microsoft.EntityFrameworkCore.Design
- Pomelo.EntityFrameworkCore.MySql
- Microsoft.AspNetCore.Authentication.JwtBearer
- AutoMapper.Extensions.Microsoft.DependencyInjection
- FluentValidation.AspNetCore
- Swashbuckle.AspNetCore
- BCrypt.Net-Next

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT.

## 📞 Suporte

Para dúvidas e suporte:
- Email: contact@formengine.com
- Issues: [GitHub Issues](link-do-repositorio/issues)

---

Desenvolvido com ❤️ usando ASP.NET Core 8
