import React from 'react';
import { Link } from 'react-router-dom';
import { useCart } from '@/hooks';

export const CartPage: React.FC = () => {
  const { items, removeItem, updateQuantity, getTotalPrice, clear } = useCart();
  const total = getTotalPrice();

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-8">
        <h1 className="text-4xl font-bold mb-8">Carrito de Compras</h1>

        {items.length === 0 ? (
          <div className="text-center py-12 bg-white rounded-lg">
            <p className="text-gray-600 text-lg mb-6">Tu carrito está vacío</p>
            <Link
              to="/"
              className="inline-block bg-primary text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition"
            >
              Volver al Catálogo
            </Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Lista de Items */}
            <div className="lg:col-span-2 bg-white rounded-lg p-6">
              {items.map((item) => (
                <div
                  key={item.productId}
                  className="flex gap-6 pb-6 border-b last:border-b-0 last:pb-0"
                >
                  {/* Imagen */}
                  <div className="w-24 h-24 bg-gray-200 rounded overflow-hidden flex-shrink-0">
                    {item.product.images?.[0] ? (
                      <img
                        src={item.product.images[0]}
                        alt={item.product.name}
                        className="w-full h-full object-cover"
                        onError={(e) => {
                          (e.target as HTMLImageElement).style.display = 'none';
                        }}
                      />
                    ) : (
                      <div className="w-full h-full flex items-center justify-center text-gray-400 text-xs">
                        Sin Imagen
                      </div>
                    )}
                  </div>

                  {/* Información */}
                  <div className="flex-1">
                    <h3 className="font-bold text-lg text-gray-800 mb-2">{item.product.name}</h3>
                    <p className="text-gray-600 text-sm mb-4">{item.product.description}</p>
                    <p className="text-primary font-bold text-lg">
                      ${item.product.price.toLocaleString()}
                    </p>
                  </div>

                  {/* Cantidad */}
                  <div className="flex flex-col items-center gap-2">
                    <div className="flex border border-gray-300 rounded">
                      <button
                        onClick={() =>
                          updateQuantity(
                            item.productId,
                            Math.max(1, item.quantity - 1)
                          )
                        }
                        className="px-3 py-1 hover:bg-gray-100"
                      >
                        −
                      </button>
                      <span className="px-4 py-1 border-l border-r">
                        {item.quantity}
                      </span>
                      <button
                        onClick={() =>
                          updateQuantity(
                            item.productId,
                            Math.min(item.product.stock, item.quantity + 1)
                          )
                        }
                        className="px-3 py-1 hover:bg-gray-100"
                      >
                        +
                      </button>
                    </div>
                    <p className="text-gray-600 text-sm">
                      ${(item.product.price * item.quantity).toLocaleString()}
                    </p>
                  </div>

                  {/* Eliminar */}
                  <button
                    onClick={() => removeItem(item.productId)}
                    className="text-red-600 hover:text-red-800 font-semibold"
                  >
                    Eliminar
                  </button>
                </div>
              ))}
            </div>

            {/* Resumen */}
            <div className="bg-white rounded-lg p-6 h-fit sticky top-4">
              <h2 className="text-2xl font-bold mb-6">Resumen de Compra</h2>

              <div className="space-y-4 pb-6 border-b">
                <div className="flex justify-between text-gray-600">
                  <span>Subtotal ({items.length} productos):</span>
                  <span>${total.toLocaleString()}</span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Envío:</span>
                  <span>Gratis</span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Impuestos:</span>
                  <span>${Math.round(total * 0.16).toLocaleString()}</span>
                </div>
              </div>

              <div className="flex justify-between items-center text-2xl font-bold mb-6 pt-6">
                <span>Total:</span>
                <span className="text-primary">
                  ${Math.round(total * 1.16).toLocaleString()}
                </span>
              </div>

              <button className="w-full bg-primary text-white py-3 rounded-lg hover:bg-blue-700 transition font-bold mb-3">
                Proceder al Pago
              </button>

              <button
                onClick={() => clear()}
                className="w-full bg-red-600 text-white py-2 rounded-lg hover:bg-red-700 transition font-semibold mb-3"
              >
                Vaciar Carrito
              </button>

              <Link
                to="/"
                className="block text-center text-primary hover:underline"
              >
                Continuar Comprando
              </Link>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
