# 🚀 API ENDPOINTS - WORKFLOW DE SUBMISSÕES
## Documentação Completa para Integração Front-end

---

## 📋 **INFORMAÇÕES GERAIS**

- **Base URL**: `http://localhost:5187/api`
- **Autenticação**: Bearer Token JWT
- **Content-Type**: `application/json`
- **Autorização**: Alguns endpoints requerem roles específicas

---

## 🔐 **AUTENTICAÇÃO (Endpoints Existentes)**

### **POST /api/Auth/register**
```typescript
interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  role: "admin" | "gestor" | "user"; // ATENÇÃO: roles em minúsculas
}

interface RegisterResponse {
  message: string;
  user: {
    id: number;
    name: string;
    email: string;
    role: string;
  };
}
```

### **POST /api/Auth/login**
```typescript
interface LoginRequest {
  email: string;
  password: string;
}

interface LoginResponse {
  token: string;
  user: {
    id: number;
    name: string;
    email: string;
    role: string;
  };
}
```

---

## 📝 **FORMULÁRIOS (Endpoints Existentes - Verificar Mudanças)**

### **POST /api/Forms**
```typescript
interface CreateFormRequest {
  name: string; // MUDOU: era "title"
  schemaJson: string; // String JSON, não objeto
  rolesAllowed: string; // "user,gestor,admin"
  version: string; // "1.0.0"
}

interface FormResponse {
  id: number;
  name: string;
  schemaJson: string;
  rolesAllowed: string;
  version: string;
  createdAt: string;
  updatedAt: string;
}
```

### **GET /api/Forms**
```typescript
interface FormListResponse {
  id: number;
  name: string;
  version: string;
  rolesAllowed: string;
  createdAt: string;
}[]
```

### **GET /api/Forms/{id}**
```typescript
// Retorna FormResponse completo
```

---

## 🎯 **SUBMISSÕES - ENDPOINTS NOVOS**

## **1. CRIAR SUBMISSÃO**

### **POST /api/SubmissoesFormulario**
**Permissões**: Qualquer usuário autenticado

```typescript
interface CreateSubmissionRequest {
  formId: number;
  dataJson: string; // String JSON com os dados do formulário
  comentarioInicial?: string; // Opcional
}

interface SubmissionResponse {
  sucesso: boolean;
  mensagem: string;
  id: number;
  erros: string[];
  dados: any; // null na criação
}

// Exemplo de uso:
const request: CreateSubmissionRequest = {
  formId: 2,
  dataJson: JSON.stringify({
    solicitante: "João Silva",
    descricao: "Solicitação de compra de notebook",
    tipo: "compra",
    valor: 3500.00
  }),
  comentarioInicial: "Primeira versão da solicitação"
};
```

---

## **2. LISTAR SUBMISSÕES**

### **GET /api/SubmissoesFormulario**
**Permissões**: Qualquer usuário autenticado
- **Usuários comuns**: Veem apenas suas próprias submissões
- **Admin/Gestor**: Veem todas as submissões

```typescript
interface ListSubmissionsQuery {
  pagina?: number; // Default: 1
  tamanhoPagina?: number; // Default: 20, Max: 100
  status?: StatusSubmissao; // Filtro por status
  formId?: number; // Filtro por formulário
  usuarioId?: number; // Apenas admin/gestor podem usar
  dataInicio?: string; // "2024-01-01"
  dataFim?: string; // "2024-12-31"
  ordenacao?: "dataAtualizacao" | "createdAt" | "status"; // Default: dataAtualizacao
  direcao?: "asc" | "desc"; // Default: desc
}

interface ListSubmissionsResponse {
  itens: SubmissionListItem[];
  totalItens: number;
  paginaAtual: number;
  totalPaginas: number;
  tamanhoPagina: number;
  temPaginaAnterior: boolean;
  temProximaPagina: boolean;
}

interface SubmissionListItem {
  id: number;
  formId: number;
  formName: string;
  userId: number;
  userName: string;
  userEmail: string;
  status: StatusSubmissao;
  createdAt: string;
  dataAtualizacao: string;
  dataSubmissao: string | null;
  dataAprovacao: string | null;
  usuarioAprovadorId: number | null;
  usuarioAprovadorName: string | null;
  versao: number;
  excluido: boolean;
}

enum StatusSubmissao {
  Rascunho = 0,
  Enviado = 1,
  EmAnalise = 2,
  Aprovado = 3,
  Rejeitado = 4,
  Cancelado = 5
}
```

---

## **3. OBTER DETALHES DA SUBMISSÃO**

### **GET /api/SubmissoesFormulario/{id}**
**Permissões**: 
- **Proprietário**: Pode ver suas próprias submissões
- **Admin/Gestor**: Podem ver qualquer submissão

```typescript
interface SubmissionDetails {
  id: number;
  formId: number;
  formName: string;
  userId: number;
  userName: string;
  userEmail: string;
  status: StatusSubmissao;
  createdAt: string;
  dataAtualizacao: string;
  dataSubmissao: string | null;
  dataAprovacao: string | null;
  usuarioAprovadorId: number | null;
  usuarioAprovadorName: string | null;
  versao: number;
  excluido: boolean;
  dataJson: string; // Dados do formulário em JSON
  motivoRejeicao: string | null;
  enderecoIp: string;
  userAgent: string;
  dataExclusao: string | null;
  historicos: HistoricoItem[];
}

interface HistoricoItem {
  id: number;
  acao: AcaoSubmissao;
  dataAcao: string;
  usuarioId: number;
  usuarioName: string;
  usuarioEmail: string;
  comentario: string | null;
  statusAnterior: StatusSubmissao | null;
  novoStatus: StatusSubmissao;
  enderecoIp: string;
}

enum AcaoSubmissao {
  Criacao = 0,
  Atualizacao = 1,
  Envio = 2,
  ColocarAnalise = 3,
  Aprovacao = 4,
  Rejeicao = 5,
  Cancelamento = 6
}
```

---

## **4. ATUALIZAR SUBMISSÃO**

### **PUT /api/SubmissoesFormulario/{id}**
**Permissões**: 
- **Proprietário**: Apenas submissões em status "Rascunho"
- **Admin**: Qualquer submissão exceto "Aprovado"

```typescript
interface UpdateSubmissionRequest {
  dataJson: string; // String JSON com os dados atualizados
  comentario?: string; // Comentário sobre a atualização
  versao: number; // OBRIGATÓRIO - controle de concorrência
}

interface UpdateSubmissionResponse {
  sucesso: boolean;
  mensagem: string;
  id: number;
  erros: string[];
  dados: null;
}

// Exemplo de uso:
const updateRequest: UpdateSubmissionRequest = {
  dataJson: JSON.stringify({
    solicitante: "João Silva Santos",
    descricao: "Solicitação atualizada",
    tipo: "compra",
    valor: 4200.00
  }),
  comentario: "Valores atualizados conforme nova cotação",
  versao: 1 // Obtido do GET da submissão
};
```

---

## **5. WORKFLOW - AÇÕES DE ESTADO**

### **POST /api/SubmissoesFormulario/{id}/enviar**
**Permissões**: Proprietário (status deve ser "Rascunho")

```typescript
interface EnviarSubmissionRequest {
  observacoes?: string;
}

interface WorkflowResponse {
  sucesso: boolean;
  mensagem: string;
  id: number;
  erros: string[];
  dados: null;
}
```

### **POST /api/SubmissoesFormulario/{id}/colocar-analise**
**Permissões**: Admin/Gestor (status deve ser "Enviado")

```typescript
interface ColocarAnaliseRequest {
  observacoes?: string;
}
// Retorna WorkflowResponse
```

### **POST /api/SubmissoesFormulario/{id}/aprovar**
**Permissões**: Admin/Gestor (status deve ser "EmAnalise")

```typescript
interface AprovarSubmissionRequest {
  observacoes?: string;
}
// Retorna WorkflowResponse
```

### **POST /api/SubmissoesFormulario/{id}/rejeitar**
**Permissões**: Admin/Gestor (status deve ser "EmAnalise")

```typescript
interface RejeitarSubmissionRequest {
  motivo: string; // OBRIGATÓRIO
  observacoes?: string;
}
// Retorna WorkflowResponse
```

### **POST /api/SubmissoesFormulario/{id}/cancelar**
**Permissões**: Proprietário ou Admin (não pode cancelar se "Aprovado")

```typescript
interface CancelarSubmissionRequest {
  motivo: string; // OBRIGATÓRIO
  observacoes?: string;
}
// Retorna WorkflowResponse
```

---

## **6. RELATÓRIOS E CONSULTAS**

### **GET /api/SubmissoesFormulario/{id}/historico**
**Permissões**: Proprietário, Admin ou Gestor

```typescript
interface HistoricoResponse {
  submissaoId: number;
  historicos: HistoricoItem[];
}
```

### **GET /api/SubmissoesFormulario/estatisticas**
**Permissões**: Admin/Gestor

```typescript
interface EstatisticasQuery {
  dataInicio?: string; // "2024-01-01"
  dataFim?: string; // "2024-12-31"
  formId?: number;
}

interface EstatisticasResponse {
  totalSubmissoes: number;
  porStatus: {
    rascunho: number;
    enviado: number;
    emAnalise: number;
    aprovado: number;
    rejeitado: number;
    cancelado: number;
  };
  porFormulario: {
    formId: number;
    formName: string;
    total: number;
  }[];
  porUsuario: {
    usuarioId: number;
    userName: string;
    total: number;
  }[];
  tempoMedioAprovacao: number; // em horas
}
```

---

## **7. EXCLUSÃO (SOFT DELETE)**

### **DELETE /api/SubmissoesFormulario/{id}**
**Permissões**: Proprietário (apenas "Rascunho") ou Admin

```typescript
interface DeleteSubmissionRequest {
  motivo: string; // OBRIGATÓRIO
}

interface DeleteResponse {
  sucesso: boolean;
  mensagem: string;
  id: number;
  erros: string[];
  dados: null;
}
```

---

## 🔧 **TRATAMENTO DE ERROS**

### **Códigos de Status HTTP**
```typescript
interface ApiError {
  sucesso: false;
  mensagem: string;
  erros: string[];
  dados: null;
}

// Status Codes:
// 200 - OK
// 400 - Bad Request (dados inválidos, workflow inválido)
// 401 - Unauthorized (token inválido/expirado)
// 403 - Forbidden (sem permissão)
// 404 - Not Found (submissão não encontrada)
// 409 - Conflict (versão desatualizada - concorrência)
// 500 - Internal Server Error
```

### **Exemplos de Erros Comuns**
```typescript
// Erro de Permissão (403)
{
  "sucesso": false,
  "mensagem": "Acesso negado",
  "erros": ["Usuário não tem permissão para esta ação"],
  "dados": null
}

// Erro de Workflow (400)
{
  "sucesso": false,
  "mensagem": "Estado inválido",
  "erros": ["Não é possível aprovar uma submissão em rascunho"],
  "dados": null
}

// Erro de Concorrência (409)
{
  "sucesso": false,
  "mensagem": "Versão desatualizada",
  "erros": ["A submissão foi modificada por outro usuário"],
  "dados": null
}
```

---

## 📱 **EXEMPLOS DE INTEGRAÇÃO FRONT-END**

### **React/JavaScript Example**
```typescript
// Service para Submissões
class SubmissionService {
  private baseUrl = 'http://localhost:5187/api/SubmissoesFormulario';
  
  async createSubmission(data: CreateSubmissionRequest): Promise<SubmissionResponse> {
    const response = await fetch(this.baseUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${this.getToken()}`
      },
      body: JSON.stringify(data)
    });
    
    return await response.json();
  }
  
  async listSubmissions(query: ListSubmissionsQuery = {}): Promise<ListSubmissionsResponse> {
    const params = new URLSearchParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined) params.append(key, value.toString());
    });
    
    const response = await fetch(`${this.baseUrl}?${params}`, {
      headers: {
        'Authorization': `Bearer ${this.getToken()}`
      }
    });
    
    return await response.json();
  }
  
  async getSubmissionDetails(id: number): Promise<SubmissionDetails> {
    const response = await fetch(`${this.baseUrl}/${id}`, {
      headers: {
        'Authorization': `Bearer ${this.getToken()}`
      }
    });
    
    return await response.json();
  }
  
  async updateSubmission(id: number, data: UpdateSubmissionRequest): Promise<UpdateSubmissionResponse> {
    const response = await fetch(`${this.baseUrl}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${this.getToken()}`
      },
      body: JSON.stringify(data)
    });
    
    return await response.json();
  }
  
  async enviarSubmission(id: number, data: EnviarSubmissionRequest): Promise<WorkflowResponse> {
    const response = await fetch(`${this.baseUrl}/${id}/enviar`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${this.getToken()}`
      },
      body: JSON.stringify(data)
    });
    
    return await response.json();
  }
  
  // Implementar outros métodos de workflow...
  
  private getToken(): string {
    return localStorage.getItem('authToken') || '';
  }
}
```

### **Estado no React (useState/useReducer)**
```typescript
interface SubmissionState {
  submissions: SubmissionListItem[];
  currentSubmission: SubmissionDetails | null;
  loading: boolean;
  error: string | null;
  pagination: {
    currentPage: number;
    totalPages: number;
    totalItems: number;
  };
}

// Hook personalizado
function useSubmissions() {
  const [state, setState] = useState<SubmissionState>({
    submissions: [],
    currentSubmission: null,
    loading: false,
    error: null,
    pagination: { currentPage: 1, totalPages: 0, totalItems: 0 }
  });
  
  const loadSubmissions = async (query: ListSubmissionsQuery = {}) => {
    setState(prev => ({ ...prev, loading: true, error: null }));
    
    try {
      const response = await submissionService.listSubmissions(query);
      setState(prev => ({
        ...prev,
        submissions: response.itens,
        pagination: {
          currentPage: response.paginaAtual,
          totalPages: response.totalPaginas,
          totalItems: response.totalItens
        },
        loading: false
      }));
    } catch (error) {
      setState(prev => ({
        ...prev,
        error: 'Erro ao carregar submissões',
        loading: false
      }));
    }
  };
  
  return { state, loadSubmissions };
}
```

---

## 🎨 **COMPONENTES SUGERIDOS PARA O FRONT-END**

### **1. Lista de Submissões**
- Tabela com paginação
- Filtros por status, data, formulário
- Ações por linha (ver, editar, enviar, etc.)
- Status badges com cores

### **2. Formulário de Criação/Edição**
- FormEngine.io integration mantida
- Campo para comentários
- Validação de dados
- Controle de versão (hidden field)

### **3. Workflow Actions**
- Botões condicionais baseados no status
- Modais para confirmação
- Campos para observações/motivos

### **4. Histórico/Auditoria**
- Timeline de ações
- Detalhes de cada mudança
- Informações do usuário e timestamp

### **5. Dashboard/Estatísticas**
- Gráficos por status
- Métricas de performance
- Filtros de período

---

## 🚀 **PRÓXIMOS PASSOS PARA O FRONT-END**

1. **Implementar Service Layer** com todos os endpoints
2. **Criar Types/Interfaces** TypeScript
3. **Implementar Componentes Base** (lista, formulário, workflow)
4. **Adicionar Validações** client-side
5. **Implementar Tratamento de Erros** com feedback visual
6. **Adicionar Loading States** e UX melhorado
7. **Implementar Permissões** baseadas em roles
8. **Testes** unitários e de integração

---

## 📞 **SUPORTE**

Para dúvidas sobre implementação:
- Todos os endpoints foram testados via Swagger
- Sistema de auditoria funcionando 100%
- Controle de concorrência implementado
- Permissões por role validadas

**🎉 Sistema backend completo e pronto para integração!**