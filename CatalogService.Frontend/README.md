# AutoCatalog Frontend

Aplicación React + TypeScript para la plataforma de compra y venta de vehículos.

## 🚀 Características

- ✅ Listado de productos con paginación
- ✅ Filtros avanzados (categoría, rango de precio)
- ✅ Búsqueda en tiempo real
- ✅ Vista detallada de productos con modal
- ✅ Carrito de compras persistente (localStorage)
- ✅ Integración con API backend (localhost:5290)
- ✅ Autenticación JWT
- ✅ Diseño responsivo con Tailwind CSS

## 📋 Requisitos Previos

- Node.js 16+ 
- npm o pnpm
- Backend CatalogService ejecutándose en http://localhost:5290

## 🛠️ Instalación y Ejecución

```bash
# 1. Navegar al directorio frontend
cd CatalogService.Frontend

# 2. Instalar dependencias
npm install

# 3. Ejecutar servidor de desarrollo
npm run dev

# 4. Acceder en navegador
http://localhost:3000
```

## 📦 Dependencias Principales

- **react** 18.2.0 - Framework UI
- **react-router-dom** 6.20.0 - Enrutamiento
- **axios** 1.6.0 - Cliente HTTP
- **@tanstack/react-query** 5.25.0 - Gestión de estado del servidor
- **zustand** 4.4.0 - Gestión de estado (carrito)
- **tailwindcss** 3.3.6 - Estilos CSS

## 🏗️ Estructura de Carpetas

```
src/
├── pages/          # Páginas principales (HomePage, CartPage)
├── components/     # Componentes reutilizables (Header, ProductCard, etc.)
├── services/       # Cliente API y servicios
├── hooks/          # Custom hooks (useProducts, useCart, useCategories)
├── types/          # Interfaces TypeScript
├── utils/          # Funciones utilitarias
├── context/        # React Context (si es necesario)
├── App.tsx         # Componente raíz
└── main.tsx        # Punto de entrada
```

## 🎨 Componentes Principales

### Header
- Búsqueda de productos
- Enlace al carrito con contador
- Navegación

### ProductList / HomePage
- Grid de productos con paginación
- Filtros por categoría y precio
- Modal de detalle del producto

### ProductCard
- Visualización de imagen
- Precio y stock
- Botón agregar al carrito
- Click para ver detalles

### CartPage
- Listado de items en carrito
- Modificar cantidades
- Eliminar productos
- Resumen y total

### Filters
- Filtro por categoría
- Rango de precio con sliders
- Botón limpiar filtros

## 🔌 API Integration

El cliente HTTP está configurado en `src/services/apiClient.ts`:

```typescript
// Base URL: http://localhost:5290/api/v1/

// Métodos disponibles:
- getProducts(page, pageSize)
- searchProducts(search, page, pageSize)
- getProductById(id)
- getCategories()
- importProducts(file)
- getAuditLogs(page, pageSize)
```

## 🛒 Carrito de Compras

Utiliza Zustand para manejo de estado persistente:

```typescript
const { items, addItem, removeItem, updateQuantity, getTotalPrice } = useCart();
```

Los datos se guardan en localStorage automáticamente.

## 🔐 Autenticación JWT

El cliente intercepta las requests para agregar el token:

```typescript
// Token guardado en localStorage bajo 'authToken'
Authorization: Bearer {token}
```

## 📊 Data Fetching

Utiliza TanStack Query (@tanstack/react-query) para:
- Cacheo automático
- Sincronización de datos
- Manejo de errores
- Loading states

## 🎯 Filtros y Paginación

**Características implementadas:**

- ✅ Paginación con navegación entre páginas
- ✅ Filtro por categoría (radio buttons)
- ✅ Filtro por rango de precio (sliders)
- ✅ Búsqueda global (searchParams)
- ✅ Reset de filtros

**Parámetros de FilterParams:**
```typescript
{
  search?: string;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
  page: number;
  pageSize: number;
  sortBy?: 'name' | 'price' | 'newest';
  sortOrder?: 'asc' | 'desc';
}
```

## 🚀 Build para Producción

```bash
npm run build
# Salida en carpeta 'dist/'
```

## 🧪 Testing

```bash
npm run lint
# Verificar errores de TypeScript y ESLint
```

## 📱 Responsivo

- Mobile: 1 columna
- Tablet: 2 columnas
- Desktop: 3 columnas en grid de productos
- Todos los componentes adaptados para diferentes pantallas

## 🔍 Variables de Entorno

Crear archivo `.env.local`:

```
VITE_API_URL=http://localhost:5290/api/v1
```

## 📝 Notas

- El carrito persiste en localStorage incluso después de cerrar el navegador
- Los tokens JWT se guardan en localStorage
- Las queries se cachean durante 5-10 minutos según el tipo
- La paginación comienza en página 1

## 🤝 Integración Backend

El frontend espera que el backend exponga:
- GET `/api/v1/catalog/products` (con paginación)
- GET `/api/v1/catalog/products/{id}`
- GET `/api/v1/categories`
- POST `/api/v1/catalog/products/import` (multipart)
- GET `/api/v1/audit-logs` (con paginación)

Todos disponibles en `http://localhost:5290/swagger`

## 📄 Licencia

MIT
