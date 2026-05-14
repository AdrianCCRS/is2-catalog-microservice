import React, { useState } from 'react';
import { Product } from '@/types';
import { useCart } from '@/hooks';

interface ProductDetailModalProps {
  product: Product | null;
  isOpen: boolean;
  onClose: () => void;
}

export const ProductDetailModal: React.FC<ProductDetailModalProps> = ({
  product,
  isOpen,
  onClose,
}) => {
  const { addItem } = useCart();
  const [quantity, setQuantity] = useState(1);
  const [activeImageIndex, setActiveImageIndex] = useState(0);

  if (!isOpen || !product) return null;

  const handleAddToCart = () => {
    addItem(product, quantity);
    alert(`${product.name} agregado al carrito`);
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg max-w-4xl w-full max-h-screen overflow-y-auto">
        <div className="sticky top-0 bg-white border-b p-4 flex justify-between items-center">
          <h2 className="text-2xl font-bold">{product.name}</h2>
          <button
            onClick={onClose}
            className="text-2xl font-bold text-gray-500 hover:text-gray-700"
          >
            ✕
          </button>
        </div>

        <div className="p-6 grid grid-cols-2 gap-8">
          {/* Imágenes */}
          <div>
            <div className="w-full h-96 bg-gray-200 rounded-lg overflow-hidden mb-4">
              <img
                src={product.images?.[activeImageIndex] ? `http://localhost:5290${product.images[activeImageIndex]}` : 'https://via.placeholder.com/500x400'}
                alt={product.name}
                className="w-full h-full object-cover"
                onError={(e) => {
                  const img = e.target as HTMLImageElement;
                  img.src = 'https://via.placeholder.com/500x400';
                }}
              />
            </div>
            {product.images && product.images.length > 1 && (
              <div className="flex gap-2 overflow-x-auto">
                {product.images.map((img, idx) => (
                  <button
                    key={idx}
                    onClick={() => setActiveImageIndex(idx)}
                    className={`w-20 h-20 rounded border-2 overflow-hidden ${
                      idx === activeImageIndex ? 'border-primary' : 'border-gray-300'
                    }`}
                  >
                    <img src={`http://localhost:5290${img}`} alt={`Imagen ${idx + 1}`} className="w-full h-full object-cover" onError={(e) => {
                      const img = e.target as HTMLImageElement;
                      img.src = 'https://via.placeholder.com/100x100';
                    }} />
                  </button>
                ))}
              </div>
            )}
          </div>

          {/* Información */}
          <div>
            <div className="mb-6">
              <p className="text-gray-600 text-lg mb-4">{product.description}</p>
              
              {/* Tags */}
              <div className="flex flex-wrap gap-2 mb-4">
                {product.tags?.map((tag, i) => (
                  <span key={i} className="bg-primary text-white px-3 py-1 rounded-full text-sm">
                    {tag}
                  </span>
                ))}
              </div>
            </div>

            {/* Precio */}
            <div className="mb-6 pb-6 border-b">
              <p className="text-gray-600 text-sm mb-2">Precio</p>
              <p className="text-4xl font-bold text-primary">${product.price.toLocaleString()}</p>
            </div>

            {/* Stock */}
            <div className="mb-6">
              <p className={`text-lg font-semibold ${product.stock > 0 ? 'text-green-600' : 'text-red-600'}`}>
                {product.stock > 0 ? `${product.stock} disponibles` : 'Agotado'}
              </p>
            </div>

            {/* Cantidad */}
            {product.stock > 0 && (
              <div className="mb-6">
                <label className="block text-sm font-semibold text-gray-700 mb-2">Cantidad</label>
                <input
                  type="number"
                  min="1"
                  max={product.stock}
                  value={quantity}
                  onChange={(e) => setQuantity(Math.min(parseInt(e.target.value) || 1, product.stock))}
                  className="w-20 px-4 py-2 border border-gray-300 rounded"
                />
              </div>
            )}

            {/* Botones */}
            <div className="flex gap-3">
              {product.stock > 0 && (
                <button
                  onClick={handleAddToCart}
                  className="flex-1 bg-primary text-white py-3 rounded-lg hover:bg-blue-700 transition font-bold"
                >
                  Agregar al Carrito
                </button>
              )}
              <button
                onClick={onClose}
                className="flex-1 bg-gray-300 text-gray-800 py-3 rounded-lg hover:bg-gray-400 transition font-bold"
              >
                Cerrar
              </button>
            </div>

            {/* Información Adicional */}
            <div className="mt-8 pt-8 border-t">
              <p className="text-sm text-gray-600 mb-2">
                <span className="font-semibold">Creado por:</span> {product.createdBy}
              </p>
              <p className="text-sm text-gray-600">
                <span className="font-semibold">Última actualización:</span> {new Date(product.updatedAt).toLocaleDateString()}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
