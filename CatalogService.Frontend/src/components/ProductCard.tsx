import React, { useState } from 'react';
import { Product } from '@/types';
import { useCart } from '@/hooks';

interface ProductCardProps {
  product: Product;
  onShowDetails: (product: Product) => void;
}

export const ProductCard: React.FC<ProductCardProps> = ({ product, onShowDetails }) => {
  const { addItem } = useCart();
  const [quantity, setQuantity] = useState(1);

  const handleAddToCart = (e: React.MouseEvent) => {
    e.stopPropagation();
    addItem(product, quantity);
    setQuantity(1);
    alert(`${product.name} agregado al carrito`);
  };

  return (
    <div
      className="bg-white rounded-lg shadow-md hover:shadow-lg transition cursor-pointer overflow-hidden"
      onClick={() => onShowDetails(product)}
    >
      {/* Imagen */}
      <div className="w-full h-48 bg-gray-200 overflow-hidden">
        <img
          src={product.images?.[0] ? `http://localhost:5290${product.images[0]}` : 'https://via.placeholder.com/400x300?text=Sin+Imagen'}
          alt={product.name}
          className="w-full h-full object-cover hover:scale-110 transition"
          onError={(e) => {
            const img = e.target as HTMLImageElement;
            img.src = 'https://via.placeholder.com/400x300?text=Sin+Imagen';
          }}
        />
      </div>

      {/* Contenido */}
      <div className="p-4">
        <h3 className="font-bold text-lg text-gray-800 truncate">{product.name}</h3>
        <p className="text-gray-600 text-sm mb-2 line-clamp-2">{product.description}</p>

        {/* Tags */}
        <div className="flex flex-wrap gap-1 mb-3">
          {product.tags?.slice(0, 3).map((tag, i) => (
            <span key={i} className="text-xs bg-gray-100 text-gray-600 px-2 py-1 rounded">
              {tag}
            </span>
          ))}
        </div>

        {/* Precio y Stock */}
        <div className="flex justify-between items-center mb-3">
          <span className="text-2xl font-bold text-primary">${product.price.toLocaleString()}</span>
          <span className={`text-sm font-semibold ${product.stock > 0 ? 'text-green-600' : 'text-red-600'}`}>
            {product.stock > 0 ? `${product.stock} disponibles` : 'Agotado'}
          </span>
        </div>

        {/* Cantidad y Carrito */}
        {product.stock > 0 && (
          <div className="flex gap-2">
            <input
              type="number"
              min="1"
              max={product.stock}
              value={quantity}
              onChange={(e) => setQuantity(Math.min(parseInt(e.target.value) || 1, product.stock))}
              onClick={(e) => e.stopPropagation()}
              className="w-16 px-2 py-2 border border-gray-300 rounded text-center"
            />
            <button
              onClick={handleAddToCart}
              className="flex-1 bg-primary text-white py-2 rounded hover:bg-blue-700 transition font-semibold"
            >
              Agregar al Carrito
            </button>
          </div>
        )}
      </div>
    </div>
  );
};
