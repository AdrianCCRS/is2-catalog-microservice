# CatalogService - Microservicio de Catálogo de Productos

## 📌 Descripción

**CatalogService** es un microservicio de gestión de catálogo de productos para una plataforma de e-commerce de vehículos. Proporciona APIs REST para operaciones CRUD de productos, gestión de categorías, búsqueda de texto, imágenes de productos e importación masiva.

**Stack Tecnológico:**
- **Backend:** .NET 10.0 con C# 13, MongoDB 6.0, RabbitMQ, gRPC
- **Frontend:** React 18.2 con TypeScript, Tailwind CSS, Vite, Zustand
- **Arquitectura:** Microservicios, DDD, Event-Driven, JWT Authentication

---

## 🚀 Inicio Rápido

### 1. Requisitos Previos
```bash
# Verificar instalaciones
dotnet --version        # .NET 10.0+
node --version         # Node.js 18+
npm --version          # npm 9+
mongod --version       # MongoDB 6.0+
```

### 2. Configurar Backend

```bash
# Navegar a carpeta del API
cd CatalogService.API

# Restaurar dependencias NuGet
dotnet restore

# Aplicar migraciones (si aplica)
dotnet ef database update

# Ejecutar en modo desarrollo
dotnet run --launch-profile Development
```

**API estará disponible en:** `http://localhost:5290`
**Health check:** `http://localhost:5290/health`
**Swagger:** `http://localhost:5290/swagger`

### 3. Configurar Frontend

```bash
# Navegar a carpeta del Frontend
cd CatalogService.Frontend

# Instalar dependencias
npm install

# Crear archivo .env.local si no existe
echo "VITE_API_URL=http://localhost:5290/api/v1" > .env.local

# Ejecutar servidor de desarrollo
npm run dev
```

**Frontend estará disponible en:** `http://localhost:5173`

### 4. Cargar Datos Iniciales (20 Vehículos)

```powershell
# Desde la raíz del proyecto
.\import-cars.ps1

# O manualmente con curl:
curl -X POST http://localhost:5290/api/v1/catalog/products/import `
  -H "Authorization: Bearer YOUR_JWT_TOKEN" `
  -F "file=@seed-cars.json"
```

---

## 📋 API REST Endpoints

### Productos

#### GET `/api/v1/catalog/products`
Lista todos los productos con paginación y filtros
```bash
# Listar todos
GET http://localhost:5290/api/v1/catalog/products

# Con paginación
GET http://localhost:5290/api/v1/catalog/products?page=1&pageSize=10

# Buscar
GET http://localhost:5290/api/v1/catalog/products?search=Honda

# Por categoría
GET http://localhost:5290/api/v1/catalog/products?categoryId=123
```

**Response:**
```json
{
  "data": [
    {
      "id": "507f1f77bcf86cd799439011",
      "name": "Honda Accord 2024",
      "description": "Sedan premium...",
      "price": 32900,
      "stock": 8,
      "images": ["/images/Honda accord 2024.jpg"],
      "categoryId": null,
      "tags": ["sedan", "premium"],
      "createdAt": "2024-05-13T10:30:00Z",
      "updatedAt": "2024-05-13T10:30:00Z"
    }
  ],
  "totalCount": 20,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 2
}
```

#### GET `/api/v1/catalog/products/{id}`
Obtener detalle de un producto
```bash
GET http://localhost:5290/api/v1/catalog/products/507f1f77bcf86cd799439011
```

#### POST `/api/v1/catalog/products` ⚡ Requiere JWT Admin
Crear nuevo producto
```bash
POST http://localhost:5290/api/v1/catalog/products
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "name": "Nuevo Vehículo",
  "description": "Descripción...",
  "price": 25000,
  "stock": 5,
  "categoryId": null,
  "images": ["/images/nuevo.jpg"],
  "tags": ["sedan", "nuevo"]
}
```

#### PUT `/api/v1/catalog/products/{id}` ⚡ Requiere JWT Admin
Actualizar producto completo
```bash
PUT http://localhost:5290/api/v1/catalog/products/507f1f77bcf86cd799439011
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{ /* mismo formato que POST */ }
```

#### PATCH `/api/v1/catalog/products/{id}` ⚡ Requiere JWT Admin
Actualizar campos parciales
```bash
PATCH http://localhost:5290/api/v1/catalog/products/507f1f77bcf86cd799439011
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{ "price": 27000 }
```

#### DELETE `/api/v1/catalog/products/{id}` ⚡ Requiere JWT Admin
Eliminar producto
```bash
DELETE http://localhost:5290/api/v1/catalog/products/507f1f77bcf86cd799439011
Authorization: Bearer <JWT_TOKEN>
```

#### POST `/api/v1/catalog/products/import` ⚡ Requiere JWT Admin
Importar productos en lote
```bash
POST http://localhost:5290/api/v1/catalog/products/import
Authorization: Bearer <JWT_TOKEN>
Content-Type: multipart/form-data

file: seed-cars.json
```

### Categorías

```bash
GET    /api/v1/catalog/categories              # Listar
GET    /api/v1/catalog/categories/{id}         # Obtener
POST   /api/v1/catalog/categories              # Crear (requiere JWT admin)
PUT    /api/v1/catalog/categories/{id}         # Actualizar (requiere JWT admin)
DELETE /api/v1/catalog/categories/{id}         # Eliminar (requiere JWT admin)
```

### Health & Monitoring

```bash
# Health check
GET http://localhost:5290/health

# Response cuando está saludable
{
  "status": "Healthy",
  "checks": {
    "mongodb": "Healthy"
  }
}
```

---

## 🔐 Autenticación y Autorización

### JWT Token
El sistema valida tokens JWT en endpoints protegidos.

**Endpoints protegidos (requieren JWT con rol "admin"):**
- ✅ POST /products
- ✅ PUT /products/{id}
- ✅ PATCH /products/{id}
- ✅ DELETE /products/{id}
- ✅ POST /products/import
- ✅ POST /categories
- ✅ PUT /categories/{id}
- ✅ DELETE /categories/{id}

**Obtener token (desde tu Identity Server/Keycloak):**
```bash
curl -X POST https://your-auth-server/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=catalog-api&client_secret=secret&scope=catalog-api"
```

Luego usar en headers:
```bash
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 📊 Estructura de la Base de Datos (MongoDB)

### Colección: `products`
```javascript
{
  _id: ObjectId("507f1f77bcf86cd799439011"),
  name: "Honda Accord 2024",
  description: "Sedan premium...",
  price: 32900,
  stock: 8,
  categoryId: null,
  images: ["/images/Honda accord 2024.jpg"],
  tags: ["sedan", "premium", "lujo"],
  createdAt: ISODate("2024-05-13T10:30:00Z"),
  updatedAt: ISODate("2024-05-13T10:30:00Z"),
  createdBy: "admin@tienda.com"
}
```

**Índices:**
- `_id` (primario)
- `name` (búsqueda)
- `text` en fields: name, description, tags
- `categoryId` (filtros)
- `createdAt` (ordenamiento)

### Colección: `categories`
```javascript
{
  _id: ObjectId("..."),
  name: "Sedanes",
  description: "Vehículos sedán",
  imageUrl: "/images/sedans.jpg",
  parentId: null,
  isActive: true,
  createdAt: ISODate(...),
  updatedAt: ISODate(...)
}
```

### Colección: `auditlogs`
```javascript
{
  _id: ObjectId("..."),
  entityType: "Product",
  entityId: "507f1f77bcf86cd799439011",
  action: "Create",
  oldValues: {},
  newValues: { name: "Honda Accord 2024", price: 32900 },
  performedBy: "admin@tienda.com",
  timestamp: ISODate("2024-05-13T10:30:00Z")
}
```

---

## 🎨 Frontend - Componentes Principales

### Pages
- **HomePage** - Listado de productos con filtros y búsqueda
- **NotFoundPage** - Página de error 404

### Components
- **ProductCard** - Card individual de producto con imagen
- **ProductDetailModal** - Modal con detalle y galería de imágenes
- **Filters** - Sidebar de filtros por categoría y precio
- **Header** - Navegación y barra de búsqueda
- **Cart** - Carrito de compras (Zustand store)

### Hooks
- **useProducts** - Listado con React Query
- **useCategories** - Categorías
- **useCart** - Gestión de carrito con Zustand
- **useSearch** - Búsqueda de productos

---

## 🧪 Testing

### Backend
```bash
cd CatalogService.API
dotnet test --logger "console;verbosity=detailed"
```

### Frontend
```bash
cd CatalogService.Frontend
npm run test
npm run test:coverage
```

---

## 📝 Logs y Monitoreo

### Backend Logs
Ubicación: `CatalogService.API/Logs/`

**Formato JSON:**
```json
{
  "timestamp": "2024-05-13T10:30:00Z",
  "level": "Information",
  "message": "Evento ProductUpdated publicado para producto 507f1f77bcf86cd799439011",
  "logger": "CatalogService.API.Controllers.ProductsController"
}
```

### Ver Health Status
```bash
curl http://localhost:5290/health -i

# Output
HTTP/1.1 200 OK
{
  "status": "Healthy",
  "checks": {
    "mongodb": "Healthy"
  }
}
```

---

## 🔧 Configuración

### Backend (appsettings.json)
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "CatalogServiceDB"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  },
  "Jwt": {
    "ValidIssuer": "your-issuer",
    "ValidAudience": "your-audience",
    "Secret": "your-secret-key"
  }
}
```

### Frontend (.env.local)
```
VITE_API_URL=http://localhost:5290/api/v1
VITE_ENV=development
```

---

## 🐳 Docker (Opcional)

```bash
# Ver docker-compose.yml para MongoDB, RabbitMQ, etc.
docker-compose up -d

# Logs en tiempo real
docker-compose logs -f catalog-api
```

---

## 📚 Documentación Adicional

- [openapi.json](./CatalogService.API/wwwroot/swagger-ui) - API completa
- [sprint-planning.md](./sprint_4_informe.tex) - Planificación del proyecto
- [IMPLEMENTACION_COMPLETADA.md](./IMPLEMENTACION_COMPLETADA.md) - Detalles de implementación

---

## 🤝 Contribuir

El proyecto sigue:
- **C# Coding Guidelines**: Microsoft Style Guide
- **TypeScript**: Strict mode
- **Commits**: Conventional Commits (feat:, fix:, docs:, etc.)
- **Branch Strategy**: Feature branches con PR reviews

---

## 📞 Soporte

Para problemas:
1. Revisa los logs en `CatalogService.API/Logs/`
2. Verifica que MongoDB esté corriendo: `mongod --version`
3. Revisa que los ports (5290, 5173, 27017) estén disponibles
4. Valida el JWT token si hay errores 401/403

---

## ✨ Versión

**CatalogService v1.0.0-sprint0**
- Fecha: 13 de Mayo de 2024
- Estado: ✅ Listo para Sprint 1
- Cobertura HU: 100% (50 SP completados)

---

**¡Comienza a desarrollar! 🚀**
