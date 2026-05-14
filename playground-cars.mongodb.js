// Script para insertar 20 carros reales con precios en COP
// Ejecutar en MongoDB Compass o mongodb shell

const cars = [
  {
    "name": "Chevrolet Spark GT",
    "description": "Auto económico, perfecto para ciudad con motor 1.2L y 94 HP",
    "price": 28500000,
    "stock": 5,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d0/2016_Chevrolet_Spark_EV_%28facelift%2C_US%29%2C_front_4.30.18.jpg/1280px-2016_Chevrolet_Spark_EV_%28facelift%2C_US%29%2C_front_4.30.18.jpg"
    ],
    "tags": ["económico", "ciudad", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Hyundai i10 GLS",
    "description": "Compacto confiable, excelente consumo de combustible, 1.2L 82 HP",
    "price": 32000000,
    "stock": 3,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3f/Hyundai_i10_Front_20080718.jpg/1280px-Hyundai_i10_Front_20080718.jpg"
    ],
    "tags": ["económico", "confiable", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Renault Twingo Zen",
    "description": "Urbano y versátil, motor 1.0L, perfecto para desplazamientos diarios",
    "price": 30500000,
    "stock": 4,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/5/55/Renault_Twingo_II.JPG/1280px-Renault_Twingo_II.JPG"
    ],
    "tags": ["urbano", "económico", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Toyota Yaris 2024",
    "description": "Sedán compacto híbrido, motor 1.5L, tecnología Toyota confiable",
    "price": 42000000,
    "stock": 6,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/7/73/Toyota_Yaris_XLE_%282017%29.jpg/1280px-Toyota_Yaris_XLE_%282017%29.jpg"
    ],
    "tags": ["confiable", "híbrido", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Kia Picanto 2024",
    "description": "City car deportivo, 1.2L, diseño moderno y espacioso",
    "price": 35000000,
    "stock": 5,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/1/1a/Kia_Picanto_second_generation.JPG/1280px-Kia_Picanto_second_generation.JPG"
    ],
    "tags": ["moderno", "ciudad", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Honda City 2023",
    "description": "Sedán ágil y confiable, motor 1.5L, excelente desempeño en ciudad",
    "price": 48000000,
    "stock": 4,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f5/2009_Honda_City_front_4.19.jpg/1280px-2009_Honda_City_front_4.19.jpg"
    ],
    "tags": ["confiable", "ciudad", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Mazda 3 2024",
    "description": "Sedán deportivo, motor 2.0L, dinámica superior y diseño premium",
    "price": 62000000,
    "stock": 3,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2f/2014-2018_Mazda_3_%28BM%29_SP25_hatchback_%282018-10-25%29_01.jpg/1280px-2014-2018_Mazda_3_%28BM%29_SP25_hatchback_%282018-10-25%29_01.jpg"
    ],
    "tags": ["deportivo", "premium", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Volkswagen Polo 2024",
    "description": "Sedán alemán compacto, motor 1.6L, tecnología y calidad VW",
    "price": 58000000,
    "stock": 4,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a6/VW_Polo_Vivo_2014.JPG/1280px-VW_Polo_Vivo_2014.JPG"
    ],
    "tags": ["alemán", "premium", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Ford Fiesta 2023",
    "description": "Sedán ágil americano, motor 1.6L, excelente relación precio-rendimiento",
    "price": 45000000,
    "stock": 5,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0c/2011_Ford_Fiesta_%28WS%29_Zetec_hatchback_%282015-07-14%29_01.jpg/1280px-2011_Ford_Fiesta_%28WS%29_Zetec_hatchback_%282015-07-14%29_01.jpg"
    ],
    "tags": ["ágil", "económico", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Chevrolet Cruze 2024",
    "description": "Sedán mediano, motor 1.8L, conectividad y seguridad avanzada",
    "price": 55000000,
    "stock": 3,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b7/2009-2014_Chevrolet_Cruze_sedan_%28first_generation%29.JPG/1280px-2009-2014_Chevrolet_Cruze_sedan_%28first_generation%29.JPG"
    ],
    "tags": ["mediano", "tecnología", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Nissan Versa 2024",
    "description": "Sedán espacioso y confiable, motor 1.6L, excelente maletero",
    "price": 46000000,
    "stock": 4,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/3/35/2012_Nissan_Versa_sedan_%28US%29.jpg/1280px-2012_Nissan_Versa_sedan_%28US%29.jpg"
    ],
    "tags": ["espacioso", "confiable", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Hyundai Elantra 2024",
    "description": "Sedán mediano moderno, motor 2.0L, garantía extendida, diseño elegante",
    "price": 52000000,
    "stock": 4,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/1/1e/2011-2016_Hyundai_Elantra_sedan.JPG/1280px-2011-2016_Hyundai_Elantra_sedan.JPG"
    ],
    "tags": ["moderno", "garantía", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Toyota Corolla 2024",
    "description": "Sedán legendario, motor 1.8L híbrido, confiabilidad garantizada",
    "price": 68000000,
    "stock": 2,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c8/11th_Toyota_Corolla.jpg/1280px-11th_Toyota_Corolla.jpg"
    ],
    "tags": ["legendario", "híbrido", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Honda Accord 2024",
    "description": "Sedán ejecutivo, motor 2.0L turbo, confort y tecnología premium",
    "price": 85000000,
    "stock": 2,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3f/10th_Honda_Accord.jpg/1280px-10th_Honda_Accord.jpg"
    ],
    "tags": ["ejecutivo", "premium", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Nissan Sentra 2024",
    "description": "Sedán mediano eficiente, motor 2.0L, tecnología inteligente integrada",
    "price": 51000000,
    "stock": 3,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/f/fa/2013_Nissan_Sentra_sedan.JPG/1280px-2013_Nissan_Sentra_sedan.JPG"
    ],
    "tags": ["eficiente", "inteligente", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Volkswagen Passat 2024",
    "description": "Sedán premium alemán, motor 2.0L turbo, lujo y dinamismo",
    "price": 95000000,
    "stock": 2,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/4/46/VW_Passat_B8_IMG_3920.jpg/1280px-VW_Passat_B8_IMG_3920.jpg"
    ],
    "tags": ["premium", "alemán", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Ford Fusion 2023",
    "description": "Sedán híbrido americano, motor 2.0L, eficiencia y confiabilidad",
    "price": 72000000,
    "stock": 3,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/c/cf/2014_Ford_Fusion.jpg/1280px-2014_Ford_Fusion.jpg"
    ],
    "tags": ["híbrido", "americano", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Kia Optima 2024",
    "description": "Sedán deportivo de lujo, motor 2.0L, diseño sofisticado y dinámico",
    "price": 78000000,
    "stock": 2,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/8/85/Kia_Optima_front_20.08.12.jpg/1280px-Kia_Optima_front_20.08.12.jpg"
    ],
    "tags": ["deportivo", "lujo", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Hyundai Sonata 2024",
    "description": "Sedán premium coreano, motor 2.4L, confort excepcional, tecnología 5G",
    "price": 98000000,
    "stock": 1,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f0/2020_Hyundai_Sonata_--_2020_DC_auto_show_1.jpg/1280px-2020_Hyundai_Sonata_--_2020_DC_auto_show_1.jpg"
    ],
    "tags": ["premium", "confort", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  },
  {
    "name": "Chevrolet Aveo 2023",
    "description": "Sedán compacto americano, motor 1.6L, precio accesible, espacioso",
    "price": 38000000,
    "stock": 5,
    "categoryId": "sedan",
    "images": [
      "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e2/2011_Chevrolet_Aveo_sedan.JPG/1280px-2011_Chevrolet_Aveo_sedan.JPG"
    ],
    "tags": ["accesible", "espacioso", "nuevo"],
    "createdBy": "admin@autocatalog.com",
    "createdAt": new Date().toISOString(),
    "updatedAt": new Date().toISOString(),
    "isActive": true
  }
];

// Para insertar en MongoDB usando mongosh:
// db.products.insertMany(cars);

console.log("20 carros listos para insertar:");
console.log(JSON.stringify(cars, null, 2));
