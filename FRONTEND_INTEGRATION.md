# Integração Front-end (React) - FormEngine API

Este documento mostra como integrar a API FormEngine com um front-end React.

## 🔧 Configuração Inicial

### Instalar Dependências

```bash
npm install axios
# ou
yarn add axios
```

### Criar Service de API

```javascript
// src/services/api.js
import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para adicionar token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor para tratamento de erros
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
```

---

## 🔐 Autenticação

### AuthService

```javascript
// src/services/authService.js
import api from './api';

export const authService = {
  async login(email, password) {
    const response = await api.post('/auth/login', { email, password });
    const { token, user } = response.data;
    
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));
    
    return { token, user };
  },

  async register(name, email, password, role = 'user') {
    const response = await api.post('/auth/register', {
      name,
      email,
      password,
      role,
    });
    const { token, user } = response.data;
    
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));
    
    return { token, user };
  },

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },

  getCurrentUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  isAuthenticated() {
    return !!localStorage.getItem('token');
  },
};
```

### Componente de Login

```jsx
// src/components/Login.jsx
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await authService.login(email, password);
      navigate('/dashboard');
    } catch (err) {
      setError(err.response?.data?.message || 'Erro ao fazer login');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <form onSubmit={handleSubmit}>
        <h2>Login</h2>
        {error && <div className="error">{error}</div>}
        
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        
        <input
          type="password"
          placeholder="Senha"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        
        <button type="submit" disabled={loading}>
          {loading ? 'Entrando...' : 'Entrar'}
        </button>
      </form>
    </div>
  );
}
```

---

## 📝 Formulários

### FormService

```javascript
// src/services/formService.js
import api from './api';

export const formService = {
  async getAllForms() {
    const response = await api.get('/forms');
    return response.data;
  },

  async getFormById(id) {
    const response = await api.get(`/forms/${id}`);
    return response.data;
  },

  async createForm(formData) {
    const response = await api.post('/forms', formData);
    return response.data;
  },

  async updateForm(id, formData) {
    const response = await api.put(`/forms/${id}`, formData);
    return response.data;
  },

  async deleteForm(id) {
    await api.delete(`/forms/${id}`);
  },
};
```

### Componente de Listagem de Formulários

```jsx
// src/components/FormList.jsx
import React, { useState, useEffect } from 'react';
import { formService } from '../services/formService';

export default function FormList() {
  const [forms, setForms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadForms();
  }, []);

  const loadForms = async () => {
    try {
      const data = await formService.getAllForms();
      setForms(data);
    } catch (err) {
      setError('Erro ao carregar formulários');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Deseja realmente excluir este formulário?')) return;

    try {
      await formService.deleteForm(id);
      setForms(forms.filter((f) => f.id !== id));
    } catch (err) {
      alert('Erro ao excluir formulário');
    }
  };

  if (loading) return <div>Carregando...</div>;
  if (error) return <div className="error">{error}</div>;

  return (
    <div className="form-list">
      <h2>Formulários</h2>
      <button onClick={() => navigate('/forms/new')}>
        Novo Formulário
      </button>
      
      <table>
        <thead>
          <tr>
            <th>Nome</th>
            <th>Versão</th>
            <th>Criado em</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {forms.map((form) => (
            <tr key={form.id}>
              <td>{form.name}</td>
              <td>{form.version}</td>
              <td>{new Date(form.createdAt).toLocaleDateString()}</td>
              <td>
                <button onClick={() => navigate(`/forms/${form.id}`)}>
                  Ver
                </button>
                <button onClick={() => navigate(`/forms/${form.id}/edit`)}>
                  Editar
                </button>
                <button onClick={() => handleDelete(form.id)}>
                  Excluir
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
```

### Componente de Criação/Edição de Formulário

```jsx
// src/components/FormEditor.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { formService } from '../services/formService';

export default function FormEditor() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({
    name: '',
    schemaJson: '{}',
    rolesAllowed: '["admin","gestor","user"]',
    version: '1.0.0',
  });

  useEffect(() => {
    if (id) {
      loadForm();
    }
  }, [id]);

  const loadForm = async () => {
    try {
      const data = await formService.getFormById(id);
      setForm(data);
    } catch (err) {
      alert('Erro ao carregar formulário');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      // Valida JSON
      JSON.parse(form.schemaJson);
      JSON.parse(form.rolesAllowed);

      if (id) {
        await formService.updateForm(id, form);
      } else {
        await formService.createForm(form);
      }
      navigate('/forms');
    } catch (err) {
      alert(err.response?.data?.message || 'Erro ao salvar formulário');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-editor">
      <h2>{id ? 'Editar' : 'Novo'} Formulário</h2>
      
      <form onSubmit={handleSubmit}>
        <div>
          <label>Nome:</label>
          <input
            type="text"
            value={form.name}
            onChange={(e) => setForm({ ...form, name: e.target.value })}
            required
          />
        </div>

        <div>
          <label>Versão:</label>
          <input
            type="text"
            value={form.version}
            onChange={(e) => setForm({ ...form, version: e.target.value })}
            required
          />
        </div>

        <div>
          <label>Roles Permitidas (JSON):</label>
          <input
            type="text"
            value={form.rolesAllowed}
            onChange={(e) => setForm({ ...form, rolesAllowed: e.target.value })}
            placeholder='["admin","gestor","user"]'
          />
        </div>

        <div>
          <label>Schema JSON:</label>
          <textarea
            rows="10"
            value={form.schemaJson}
            onChange={(e) => setForm({ ...form, schemaJson: e.target.value })}
            required
          />
        </div>

        <button type="submit" disabled={loading}>
          {loading ? 'Salvando...' : 'Salvar'}
        </button>
        <button type="button" onClick={() => navigate('/forms')}>
          Cancelar
        </button>
      </form>
    </div>
  );
}
```

---

## 📤 Submissões

### SubmissionService

```javascript
// src/services/submissionService.js
import api from './api';

export const submissionService = {
  async createSubmission(formId, dataJson) {
    const response = await api.post('/submissions', {
      formId,
      dataJson,
    });
    return response.data;
  },

  async getSubmissionsByForm(formId) {
    const response = await api.get(`/submissions/form/${formId}`);
    return response.data;
  },

  async getMySubmissions() {
    const response = await api.get('/submissions/my-submissions');
    return response.data;
  },
};
```

### Componente de Submissão de Formulário

```jsx
// src/components/FormSubmission.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { formService } from '../services/formService';
import { submissionService } from '../services/submissionService';

export default function FormSubmission() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [form, setForm] = useState(null);
  const [formData, setFormData] = useState({});
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadForm();
  }, [id]);

  const loadForm = async () => {
    try {
      const data = await formService.getFormById(id);
      setForm(data);
      
      // Inicializa formData com campos vazios
      const schema = JSON.parse(data.schemaJson);
      const initialData = {};
      schema.fields?.forEach(field => {
        initialData[field.label] = '';
      });
      setFormData(initialData);
    } catch (err) {
      alert('Erro ao carregar formulário');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (fieldLabel, value) => {
    setFormData({ ...formData, [fieldLabel]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    try {
      await submissionService.createSubmission(
        parseInt(id),
        JSON.stringify(formData)
      );
      alert('Formulário enviado com sucesso!');
      navigate('/my-submissions');
    } catch (err) {
      alert('Erro ao enviar formulário');
    }
  };

  if (loading) return <div>Carregando...</div>;
  if (!form) return <div>Formulário não encontrado</div>;

  const schema = JSON.parse(form.schemaJson);

  return (
    <div className="form-submission">
      <h2>{form.name}</h2>
      <form onSubmit={handleSubmit}>
        {schema.fields?.map((field, index) => (
          <div key={index} className="form-field">
            <label>
              {field.label}
              {field.required && <span className="required">*</span>}
            </label>
            <input
              type={field.type}
              value={formData[field.label] || ''}
              onChange={(e) => handleChange(field.label, e.target.value)}
              required={field.required}
            />
          </div>
        ))}
        
        <button type="submit">Enviar</button>
        <button type="button" onClick={() => navigate('/forms')}>
          Cancelar
        </button>
      </form>
    </div>
  );
}
```

---

## 🗂️ Menus

### MenuService

```javascript
// src/services/menuService.js
import api from './api';

export const menuService = {
  async getAllMenus() {
    const response = await api.get('/menus');
    return response.data;
  },

  async createMenu(menuData) {
    const response = await api.post('/menus', menuData);
    return response.data;
  },

  async updateMenu(id, menuData) {
    const response = await api.put(`/menus/${id}`, menuData);
    return response.data;
  },

  async deleteMenu(id) {
    await api.delete(`/menus/${id}`);
  },
};
```

### Componente de Menu

```jsx
// src/components/Sidebar.jsx
import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { menuService } from '../services/menuService';

export default function Sidebar() {
  const [menus, setMenus] = useState([]);

  useEffect(() => {
    loadMenus();
  }, []);

  const loadMenus = async () => {
    try {
      const data = await menuService.getAllMenus();
      setMenus(data);
    } catch (err) {
      console.error('Erro ao carregar menus', err);
    }
  };

  const renderMenu = (menu) => (
    <li key={menu.id}>
      <Link to={menu.urlOrPath}>
        {menu.icon && <i className={`icon-${menu.icon}`} />}
        {menu.name}
      </Link>
      {menu.children?.length > 0 && (
        <ul className="submenu">
          {menu.children.map(renderMenu)}
        </ul>
      )}
    </li>
  );

  return (
    <nav className="sidebar">
      <ul>
        {menus.map(renderMenu)}
      </ul>
    </nav>
  );
}
```

---

## 🔒 Rotas Protegidas

```jsx
// src/components/ProtectedRoute.jsx
import React from 'react';
import { Navigate } from 'react-router-dom';
import { authService } from '../services/authService';

export default function ProtectedRoute({ children, roles = [] }) {
  const isAuthenticated = authService.isAuthenticated();
  const user = authService.getCurrentUser();

  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }

  if (roles.length > 0 && !roles.includes(user?.role)) {
    return <Navigate to="/unauthorized" />;
  }

  return children;
}
```

### Uso nas Rotas

```jsx
// src/App.jsx
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './components/Login';
import FormList from './components/FormList';
import FormEditor from './components/FormEditor';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        
        <Route
          path="/forms"
          element={
            <ProtectedRoute>
              <FormList />
            </ProtectedRoute>
          }
        />
        
        <Route
          path="/forms/new"
          element={
            <ProtectedRoute roles={['admin', 'gestor']}>
              <FormEditor />
            </ProtectedRoute>
          }
        />
        
        {/* Mais rotas... */}
      </Routes>
    </BrowserRouter>
  );
}
```

---

## 🎨 Context API para Estado Global

```jsx
// src/contexts/AuthContext.jsx
import React, { createContext, useState, useContext, useEffect } from 'react';
import { authService } from '../services/authService';

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const currentUser = authService.getCurrentUser();
    setUser(currentUser);
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    const { user } = await authService.login(email, password);
    setUser(user);
  };

  const logout = () => {
    authService.logout();
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
```

---

## 📦 Estrutura de Pastas Sugerida

```
src/
├── components/
│   ├── Login.jsx
│   ├── FormList.jsx
│   ├── FormEditor.jsx
│   ├── FormSubmission.jsx
│   ├── Sidebar.jsx
│   └── ProtectedRoute.jsx
├── services/
│   ├── api.js
│   ├── authService.js
│   ├── formService.js
│   ├── submissionService.js
│   └── menuService.js
├── contexts/
│   └── AuthContext.jsx
├── styles/
│   └── main.css
├── App.jsx
└── main.jsx
```

---

Para mais informações sobre React, consulte:
- [React Documentation](https://react.dev/)
- [Axios Documentation](https://axios-http.com/)
