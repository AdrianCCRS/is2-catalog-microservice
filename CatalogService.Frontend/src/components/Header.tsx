import React from 'react';
import { Link } from 'react-router-dom';
import { useCart } from '@/hooks';

export const Header: React.FC = () => {
  const { getTotalItems } = useCart();
  const cartItems = getTotalItems();

  return (
    <header className="bg-white shadow-md">
      <nav className="max-w-7xl mx-auto px-4 py-4 flex justify-between items-center">
        {/* Logo */}
        <Link to="/" className="text-2xl font-bold text-primary">
          🚗 AutoCatalog
        </Link>

        {/* Búsqueda */}

        

        {/* Carrito */}
        <Link
          to="/cart"
          className="relative ml-4 px-4 py-2 bg-primary text-white rounded-lg hover:bg-blue-700 transition"
        >
          🛒 Carrito
          {cartItems > 0 && (
            <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full w-6 h-6 flex items-center justify-center">
              {cartItems}
            </span>
          )}
        </Link>
      </nav>
    </header>
  );
};
