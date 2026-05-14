import React from 'react';
import { Category } from '@/types';

interface FiltersProps {
  categories: Category[];
  selectedCategory: string | null;
  minPrice: number;
  maxPrice: number;
  onCategoryChange: (categoryId: string | null) => void;
  onPriceChange: (min: number, max: number) => void;
}

export const Filters: React.FC<FiltersProps> = ({
  categories,
  selectedCategory,
  minPrice,
  maxPrice,
  onCategoryChange,
  onPriceChange,
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

      {/* Rango de Precio */}
      <div className="mb-6">
        <h3 className="font-semibold text-gray-800 mb-3">Rango de Precio</h3>
        <div className="space-y-2">
          <div>
            <label className="text-sm text-gray-600">Mínimo: ${minPrice.toLocaleString()}</label>
            <input
              type="range"
              min="0"
              max="1000000"
              step="10000"
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
              max="1000000"
              step="10000"
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
          onPriceChange(0, 1000000);
        }}
        className="w-full bg-secondary text-white py-2 rounded hover:bg-opacity-80 transition"
      >
        Limpiar Filtros
      </button>
    </div>
  );
};
