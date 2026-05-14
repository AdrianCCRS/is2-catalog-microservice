import React from 'react';

export const Footer: React.FC = () => {
  return (
    <footer className="bg-gray-800 text-white mt-16">
      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="grid grid-cols-4 gap-8 mb-8">
          <div>
            <h3 className="font-bold mb-4">Sobre Nosotros</h3>
            <p className="text-gray-400 text-sm">La mejor plataforma de compra y venta de vehículos.</p>
          </div>
          <div>
            <h3 className="font-bold mb-4">Contacto</h3>
            <p className="text-gray-400 text-sm">Email: info@autocatalog.com</p>
            <p className="text-gray-400 text-sm">Teléfono: +1 234 567 8900</p>
          </div>
          <div>
            <h3 className="font-bold mb-4">Enlaces</h3>
            <ul className="text-gray-400 text-sm space-y-2">
              <li><a href="#" className="hover:text-white">Términos de Servicio</a></li>
              <li><a href="#" className="hover:text-white">Política de Privacidad</a></li>
            </ul>
          </div>
          <div>
            <h3 className="font-bold mb-4">Síguenos</h3>
            <p className="text-gray-400 text-sm">Facebook | Twitter | Instagram</p>
          </div>
        </div>
        <div className="border-t border-gray-700 pt-8 text-center text-gray-400">
          <p>&copy; 2026 AutoCatalog. Todos los derechos reservados.</p>
        </div>
      </div>
    </footer>
  );
};
