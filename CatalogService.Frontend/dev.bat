@echo off
cd /d "c:\Users\ASUS\OneDrive\Escritorio\Estudio\CatalogService\CatalogService.Frontend"
echo === Instalando dependencias ===
npm install
echo.
echo === Iniciando servidor de desarrollo ===
echo Abre tu navegador en: http://localhost:3000
npm run dev
pause
