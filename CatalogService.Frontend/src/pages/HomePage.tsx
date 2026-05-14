import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { ProductCard, Filters, ProductDetailModal } from '@/components';
import { useProducts, useCategories } from '@/hooks';
import { Product } from '@/types';

export const HomePage: React.FC = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const [page, setPage] = useState(1);
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);
  const [minPrice, setMinPrice] = useState(0);
  const [maxPrice, setMaxPrice] = useState(1000000);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [isDetailOpen, setIsDetailOpen] = useState(false);

  const pageSize = 12;
  const { data: productsData, isLoading, error } = useProducts(page, pageSize);
  const { data: categoriesData } = useCategories();
  const categories = Array.isArray(categoriesData) ? categoriesData : [];

  // Efectos para manejo de parámetros
  useEffect(() => {
    const search = searchParams.get('search');
    if (search) {
      // Aquí podrías implementar búsqueda
      console.log('Buscando:', search);
    }
  }, [searchParams]);

  const handleCategoryChange = (categoryId: string | null) => {
    setSelectedCategory(categoryId);
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

  const totalPages = productsData?.total ? Math.ceil(productsData.total / pageSize) : 1;

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-8">
        <h1 className="text-4xl font-bold mb-2">Nuestro Catálogo de Vehículos</h1>
        <p className="text-gray-600 mb-8">Encuentra el auto perfecto para ti</p>

        <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
          {/* Filtros */}
          <div>
            <Filters
              categories={categories}
              selectedCategory={selectedCategory}
              minPrice={minPrice}
              maxPrice={maxPrice}
              onCategoryChange={handleCategoryChange}
              onPriceChange={handlePriceChange}
            />
          </div>

          {/* Grid de Productos */}
          <div className="lg:col-span-3">
            {isLoading ? (
              <div className="text-center py-12">
                <p className="text-gray-600 text-lg">Cargando productos...</p>
              </div>
            ) : error ? (
              <div className="text-center py-12">
                <p className="text-red-600 text-lg">Error al cargar productos</p>
              </div>
            ) : productsData?.results && productsData.results.length > 0 ? (
              <>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
                  {productsData.results.map((product) => (
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
                  Página {page} de {totalPages} | Total: {productsData.total} productos
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
