import React from 'react';
import { Category } from '@/types';

interface FiltersProps {
  categories: Category[];
  selectedCategory: string | null;
  minPrice: number;
  maxPrice: number;
  brands?: string[];
  selectedBrand?: string | null;
  years?: number[];
  selectedYear?: number | null;
  onCategoryChange: (categoryId: string | null) => void;
  onPriceChange: (min: number, max: number) => void;
  onBrandChange?: (brand: string | null) => void;
  onYearChange?: (year: number | null) => void;
}

export const Filters: React.FC<FiltersProps> = ({
  categories,
  selectedCategory,
  minPrice,
  maxPrice,
  brands = [],
  selectedBrand,
  years = [],
  selectedYear,
  onCategoryChange,
  onPriceChange,
  onBrandChange,
  onYearChange,
}) => {
  return (
    <div className="bg-white rounded-lg shadow-md p-6 h-fit sticky top-4">
      <h2 className="text-xl font-bold mb-6">Filtros</h2>

      {/* Categorías */}
      <div className="mb-6">
        <h3 className="font-semibold text-gray-800 mb-3">Categoría</h3>
        <div className="space-y-2">
          <label className="flex items-center cursor-pointer">
            <input
              type="radio"
              name="category"
              checked={selectedCategory === null}
              onChange={() => onCategoryChange(null)}
              className="mr-2"
            />
            <span className="text-gray-700">Todas</span>
          </label>
          {categories.map((cat) => (
            <label key={cat.id} className="flex items-center cursor-pointer">
              <input
                type="radio"
                name="category"
                checked={selectedCategory === cat.id}
                onChange={() => onCategoryChange(cat.id)}
                className="mr-2"
              />
              <span className="text-gray-700">{cat.name}</span>
            </label>
          ))}
        </div>
      </div>

      {/* Marca */}
      {brands.length > 0 && (
        <div className="mb-6">
          <h3 className="font-semibold text-gray-800 mb-3">Marca</h3>
          <div className="space-y-2">
            <label className="flex items-center cursor-pointer">
              <input
                type="radio"
                name="brand"
                checked={selectedBrand === null}
                onChange={() => onBrandChange?.(null)}
                className="mr-2"
              />
              <span className="text-gray-700">Todas</span>
            </label>
            {brands.sort().map((brand) => (
              <label key={brand} className="flex items-center cursor-pointer">
                <input
                  type="radio"
                  name="brand"
                  checked={selectedBrand === brand}
                  onChange={() => onBrandChange?.(brand)}
                  className="mr-2"
                />
                <span className="text-gray-700">{brand}</span>
              </label>
            ))}
          </div>
        </div>
      )}

      {/* Año */}
      {years.length > 0 && (
        <div className="mb-6">
          <h3 className="font-semibold text-gray-800 mb-3">Año</h3>
          <div className="space-y-2">
            <label className="flex items-center cursor-pointer">
              <input
                type="radio"
                name="year"
                checked={selectedYear === null}
                onChange={() => onYearChange?.(null)}
                className="mr-2"
              />
              <span className="text-gray-700">Todos</span>
            </label>
            {years.sort((a, b) => b - a).map((year) => (
              <label key={year} className="flex items-center cursor-pointer">
                <input
                  type="radio"
                  name="year"
                  checked={selectedYear === year}
                  onChange={() => onYearChange?.(year)}
                  className="mr-2"
                />
                <span className="text-gray-700">{year}</span>
              </label>
            ))}
          </div>
        </div>
      )}

      {/* Rango de Precio */}
      <div className="mb-6">
        <h3 className="font-semibold text-gray-800 mb-3">Rango de Precio</h3>
        <div className="space-y-2">
          <div>
            <label className="text-sm text-gray-600">Mínimo: ${minPrice.toLocaleString()}</label>
            <input
              type="range"
              min="0"
              max="100000000"
              step="100000"
              value={minPrice}
              onChange={(e) => onPriceChange(parseInt(e.target.value), maxPrice)}
              className="w-full"
            />
          </div>
          <div>
            <label className="text-sm text-gray-600">Máximo: ${maxPrice.toLocaleString()}</label>
            <input
              type="range"
              min="0"
              max="100000000"
              step="100000"
              value={maxPrice}
              onChange={(e) => onPriceChange(minPrice, parseInt(e.target.value))}
              className="w-full"
            />
          </div>
        </div>
      </div>

      {/* Botón Limpiar */}
      <button
        onClick={() => {
          onCategoryChange(null);
          onBrandChange?.(null);
          onYearChange?.(null);
          onPriceChange(0, 100000000);
        }}
        className="w-full bg-secondary text-white py-2 rounded hover:bg-opacity-80 transition"
      >
        Limpiar Filtros
      </button>
    </div>
  );
};
