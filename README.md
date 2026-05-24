# 🚗 AutoCatalog Service

AutoCatalog Service es una plataforma completa para la gestión y venta de vehículos, construida con una arquitectura de microservicios moderna. La solución incluye un backend robusto con .NET 10, un frontend interactivo con React y Vite, y se integra con MongoDB y RabbitMQ para la persistencia de datos y la comunicación basada en eventos.

## 📋 Tabla de Contenidos
1. [Arquitectura](#-arquitectura)
2. [Prerrequisitos](#-prerrequisitos)
3. [Configuración del Backend (.NET)](#-configuración-del-backend-net)
4. [Configuración del Frontend (React)](#-configuración-del-frontend-react)
5. [Ejecución del Proyecto](#-ejecución-del-proyecto)
6. [Ejecución con Docker Compose](#-ejecución-con-docker-compose)
7. [Endpoints Principales de la API](#-endpoints-principales-de-la-api)

---

## 🏛️ Arquitectura

La aplicación está dividida en dos componentes principales:

-   **`CatalogService` (Backend):** Una API RESTful construida con **.NET 10** siguiendo los principios de Clean Architecture. Es responsable de la lógica de negocio, la gestión de productos, la autenticación y la comunicación con la base de datos.
-   **`CatalogService.Frontend`:** Una Single Page Application (SPA) construida con **React 18** y **Vite**. Proporciona la interfaz de usuario para que los clientes exploren el catálogo, busquen vehículos y gestionen su carrito de compras.
-   **Base de Datos:** **MongoDB** se utiliza como base de datos principal para almacenar el catálogo de productos, categorías y otros datos relevantes.
-   **Mensajería:** **RabbitMQ** se utiliza para la comunicación asíncrona entre servicios mediante eventos (ej. `ProductCreated`, `ProductUpdated`).

---

## 🛠️ Prerrequisitos

Asegúrate de tener instalado el siguiente software antes de continuar:

-   **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)**
-   **[Node.js](https://nodejs.org/)** (versión 18.x o superior)
-   **[MongoDB](https://www.mongodb.com/try/download/community)** (o una instancia en la nube como MongoDB Atlas)
-   **[RabbitMQ](https://www.rabbitmq.com/download.html)**
-   **[Docker](https://www.docker.com/products/docker-desktop/)** (Recomendado, para una fácil configuración de MongoDB y RabbitMQ)

---

## ⚙️ Configuración del Backend (.NET)

1.  **Abrir la Solución:**
    Abre el archivo `CatalogService.slnx` con tu IDE de preferencia (Visual Studio 2022+, JetBrains Rider).

2.  **Restaurar Dependencias:**
    Ejecuta el siguiente comando en la raíz del proyecto para restaurar los paquetes NuGet:
    ```bash
    dotnet restore CatalogService.slnx
    ```

3.  **Configurar Conexiones (`appsettings.json`):**
    Navega a `CatalogService.API/` y edita el archivo `appsettings.Development.json`. Asegúrate de que las cadenas de conexión apunten a tus instancias locales de MongoDB y RabbitMQ.

    ```json
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "MongoDbSettings": {
        "ConnectionString": "mongodb://localhost:27017",
        "DatabaseName": "CatalogDb"
      },
      "RabbitMQSettings": {
        "HostName": "localhost"
      }
    }
    ```

---

## 🎨 Configuración del Frontend (React)

1.  **Navegar al Directorio del Frontend:**
    ```bash
    cd CatalogService.Frontend
    ```

2.  **Instalar Dependencias:**
    Ejecuta el siguiente comando para instalar todos los paquetes de Node.js:
    ```bash
    npm install
    ```

3.  **Configurar Variables de Entorno:**
    Crea un archivo llamado `.env` en la raíz de la carpeta `CatalogService.Frontend/` y añade la siguiente variable. Esta debe apuntar a la URL donde se ejecutará tu backend.

    ```env
    VITE_API_URL=http://localhost:5290/api/v1
    ```

---

## ▶️ Ejecución del Proyecto

Debes ejecutar el backend y el frontend en terminales separadas.

1.  **Ejecutar el Backend:**
    Navega al directorio de la API y ejecuta la aplicación.
    ```bash
    cd CatalogService.API
    dotnet run
    ```
    La API estará disponible en `http://localhost:5290`.

2.  **Ejecutar el Frontend:**
    En otra terminal, navega al directorio del frontend y ejecuta el servidor de desarrollo.
    ```bash
    cd CatalogService.Frontend
    npm run dev
    ```
    La aplicación web estará disponible en `http://localhost:3000` (o el puerto que indique Vite).

---

## 🐳 Ejecución con Docker Compose

Si tienes Docker instalado, puedes levantar las dependencias (MongoDB y RabbitMQ) fácilmente.

1.  **Iniciar Contenedores:**
    En la raíz del proyecto, ejecuta:
    ```bash
    docker-compose up -d
    ```
    Este comando iniciará contenedores para MongoDB (en el puerto `27017`) y RabbitMQ (con la UI de gestión en `http://localhost:15672`).

2.  **Ejecutar Aplicaciones:**
    Después de que los contenedores estén en funcionamiento, sigue los pasos de la sección [Ejecución del Proyecto](#-ejecución-del-proyecto) para iniciar el backend y el frontend.

---

## 🔗 Endpoints Principales de la API

La API expone varios endpoints bajo el prefijo `/api/v1`.

-   `GET /catalog/products`: Obtiene una lista paginada de productos.
-   `GET /catalog/products/{id}`: Obtiene un producto por su ID.
-   `POST /catalog/products`: Crea un nuevo producto (requiere autenticación de administrador).
-   `GET /catalog/categories`: Obtiene todas las categorías de productos.
-   `POST /catalog/products/bulk-insert-demo`: Carga 20 vehículos de demostración en la base de datos (borra los datos anteriores).
-   `POST /auth/login`: Endpoint para autenticación de usuarios.
