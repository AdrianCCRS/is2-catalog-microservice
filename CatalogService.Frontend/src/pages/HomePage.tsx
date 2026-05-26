import React, { useState, useEffect, useMemo } from 'react';
import { useSearchParams } from 'react-router-dom';
import { ProductCard, Filters, ProductDetailModal } from '@/components';
import { useProducts, useCategories, useElasticsearchSearch } from '@/hooks';
import { Product } from '@/types';

export const HomePage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const [page, setPage] = useState(1);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);
  const [selectedBrand, setSelectedBrand] = useState<string | null>(null);
  const [selectedYear, setSelectedYear] = useState<number | null>(null);
  const [minPrice, setMinPrice] = useState(0);
  const [maxPrice, setMaxPrice] = useState(100000000);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [isDetailOpen, setIsDetailOpen] = useState(false);

  const pageSize = 12;
  const { data: productsData, isLoading, error } = useProducts(page, pageSize);
  const { data: searchResults, isLoading: isSearchLoading } = useElasticsearchSearch(searchQuery);
  const { data: categoriesData } = useCategories();
  const categories = Array.isArray(categoriesData) ? categoriesData : [];

  const isSearching = searchQuery.length >= 2;

  const allProducts: Product[] = useMemo(() => {
    if (isSearching && searchResults) {
      return searchResults;
    }
    return productsData?.results || [];
  }, [isSearching, searchResults, productsData]);

  const loading = isSearching ? isSearchLoading : isLoading;

  // Función para extraer la marca del nombre del producto
  const extractBrand = (name: string): string => {
    const firstWord = name.split(' ')[0];
    return firstWord;
  };

  // Función para extraer el año del nombre del producto
  const extractYear = (name: string): number | null => {
    const match = name.match(/\b(20\d{2})\b/);
    return match ? parseInt(match[1]) : null;
  };

  // Extraer marcas y años únicos de los productos
  const brands = Array.from(
    new Set(allProducts.map(product => extractBrand(product.name)))
  ).filter(Boolean) as string[];

  const years = Array.from(
    new Set(
      allProducts
        .map(product => extractYear(product.name))
        .filter((year) => year !== null)
    )
  ).sort((a, b) => b - a) as number[];

  // Filtrar productos localmente
  const filteredProducts = allProducts.filter(product => {
    // Filtro por búsqueda
    const matchesSearch = !searchQuery || 
      product.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      product.description.toLowerCase().includes(searchQuery.toLowerCase());

    // Filtro por marca
    const matchesBrand = !selectedBrand || extractBrand(product.name) === selectedBrand;

    // Filtro por año
    const matchesYear = !selectedYear || extractYear(product.name) === selectedYear;

    // Filtro por precio
    const matchesPrice = product.price >= minPrice && product.price <= maxPrice;

    // Filtro por categoría
    const matchesCategory = !selectedCategory || product.categoryId === selectedCategory;

    return matchesSearch && matchesBrand && matchesYear && matchesPrice && matchesCategory;
  });

  // Efectos para manejo de parámetros
  useEffect(() => {
    const search = searchParams.get('search');
    if (search) {
      setSearchQuery(search);
      setPage(1);
    }
  }, [searchParams]);

  const handleCategoryChange = (categoryId: string | null) => {
    setSelectedCategory(categoryId);
    setPage(1);
  };

  const handleBrandChange = (brand: string | null) => {
    setSelectedBrand(brand);
    setPage(1);
  };

  const handleYearChange = (year: number | null) => {
    setSelectedYear(year);
    setPage(1);
  };

  const handlePriceChange = (min: number, max: number) => {
    setMinPrice(min);
    setMaxPrice(max);
    setPage(1);
  };

  const handleShowDetails = (product: Product) => {
    setSelectedProduct(product);
    setIsDetailOpen(true);
  };

  // Calcular paginación local
  const totalFilteredProducts = filteredProducts.length;
  const totalPages = Math.ceil(totalFilteredProducts / pageSize);
  const paginatedProducts = filteredProducts.slice(
    (page - 1) * pageSize,
    page * pageSize
  );

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-8">
        <h1 className="text-4xl font-bold mb-2">Nuestro Catálogo de Vehículos</h1>
        <p className="text-gray-600 mb-8">Encuentra el auto perfecto para ti</p>

        {/* Buscador */}
        <div className="mb-8">
          <input
            type="text"
            placeholder="Buscar vehículos por nombre, marca o características..."
            value={searchQuery}
            onChange={(e) => {
              setSearchQuery(e.target.value);
              setPage(1);
            }}
            className="w-full px-4 py-3 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
          {/* Filtros */}
          <div>
            <Filters
              categories={categories}
              selectedCategory={selectedCategory}
              minPrice={minPrice}
              maxPrice={maxPrice}
              brands={brands}
              selectedBrand={selectedBrand}
              years={years}
              selectedYear={selectedYear}
              onCategoryChange={handleCategoryChange}
              onPriceChange={handlePriceChange}
              onBrandChange={handleBrandChange}
              onYearChange={handleYearChange}
            />
          </div>

          {/* Grid de Productos */}
          <div className="lg:col-span-3">
            {loading ? (
              <div className="text-center py-12">
                <p className="text-gray-600 text-lg">Cargando productos...</p>
              </div>
            ) : error ? (
              <div className="text-center py-12">
                <p className="text-red-600 text-lg">Error al cargar productos</p>
              </div>
            ) : paginatedProducts.length > 0 ? (
              <>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
                  {paginatedProducts.map((product) => (
                    <ProductCard
                      key={product.id}
                      product={product}
                      onShowDetails={handleShowDetails}
                    />
                  ))}
                </div>

                {/* Paginación */}
                <div className="flex justify-center gap-2">
                  <button
                    onClick={() => setPage(Math.max(1, page - 1))}
                    disabled={page === 1}
                    className="px-4 py-2 border border-gray-300 rounded hover:bg-gray-100 disabled:opacity-50"
                  >
                    Anterior
                  </button>
                  
                  {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                    const pageNum = page > 3 ? page - 2 + i : i + 1;
                    return (
                      <button
                        key={pageNum}
                        onClick={() => setPage(pageNum)}
                        className={`px-4 py-2 rounded border ${
                          pageNum === page
                            ? 'bg-primary text-white border-primary'
                            : 'border-gray-300 hover:bg-gray-100'
                        }`}
                      >
                        {pageNum}
                      </button>
                    );
                  })}

                  <button
                    onClick={() => setPage(Math.min(totalPages, page + 1))}
                    disabled={page >= totalPages}
                    className="px-4 py-2 border border-gray-300 rounded hover:bg-gray-100 disabled:opacity-50"
                  >
                    Siguiente
                  </button>
                </div>

                {/* Info de Paginación */}
                <div className="text-center mt-4 text-gray-600">
                  Página {page} de {totalPages} | Total: {totalFilteredProducts} productos
                </div>
              </>
            ) : (
              <div className="text-center py-12">
                <p className="text-gray-600 text-lg">No se encontraron productos</p>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Modal de Detalle */}
      <ProductDetailModal
        product={selectedProduct}
        isOpen={isDetailOpen}
        onClose={() => {
          setIsDetailOpen(false);
          setSelectedProduct(null);
        }}
      />
    </div>
  );
};
