import axios, { AxiosInstance, AxiosError } from 'axios';

const API_BASE_URL = (import.meta as any).env.VITE_API_URL || 'http://localhost:5290/api/v1';

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Interceptor para agregar token JWT
    this.client.interceptors.request.use((config) => {
      const token = localStorage.getItem('authToken');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });

    // Interceptor para manejar errores HTTP
    this.client.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        if (error.response?.status === 401) {
          // Token expirado, limpiar y redirigir a login
          localStorage.removeItem('authToken');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Productos
  async getProducts(page: number = 1, pageSize: number = 12) {
    const response = await this.client.get('/catalog/products', {
      params: { page, pageSize },
    });
    return response.data;
  }

  async searchProducts(search: string, page: number = 1, pageSize: number = 12) {
    const response = await this.client.get('/catalog/products', {
      params: { search, page, pageSize },
    });
    return response.data;
  }

  async getProductById(id: string) {
    const response = await this.client.get(`/catalog/products/${id}`);
    return response.data;
  }

  async createProduct(product: any) {
    const response = await this.client.post('/catalog/products', product);
    return response.data;
  }

  async importProducts(file: File) {
    const formData = new FormData();
    formData.append('file', file);

    const response = await this.client.post('/catalog/products/import', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  }

  // Categorías
  async getCategories() {
    const response = await this.client.get('/catalog/categories');
    return response.data;
  }

  // Auditoría
  async getAuditLogs(page: number = 1, pageSize: number = 20) {
    const response = await this.client.get('/audit-logs', {
      params: { page, pageSize },
    });
    return response.data;
  }

  async getAuditLogsByEntity(entityId: string, page: number = 1, pageSize: number = 20) {
    const response = await this.client.get(`/audit-logs/entity/${entityId}`, {
      params: { page, pageSize },
    });
    return response.data;
  }
}

export const apiClient = new ApiClient();
