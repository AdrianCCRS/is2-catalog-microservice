#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Script para importar vehículos de prueba en el API del CatalogService
.DESCRIPTION
    Carga el archivo seed-cars.json en MongoDB mediante el endpoint /products/import
.PARAMETER ApiUrl
    URL base del API (default: http://localhost:5290/api/v1)
.PARAMETER JwtToken
    Token JWT para autenticación (default: usa token de admin demo)
.EXAMPLE
    .\import-cars.ps1
    .\import-cars.ps1 -ApiUrl "http://prod.example.com/api/v1" -JwtToken "eyJ..."
#>

param(
    [string]$ApiUrl = "http://localhost:5290/api/v1",
    [string]$JwtToken = "demo-admin-token"
)

Write-Host "🚗 CatalogService - Importador de Vehículos" -ForegroundColor Cyan
Write-Host "==========================================`n" -ForegroundColor Cyan

# Verificar que el archivo seed existe
$seedFile = ".\seed-cars.json"
if (-not (Test-Path $seedFile)) {
    Write-Host "❌ Error: No se encontró $seedFile" -ForegroundColor Red
    exit 1
}

Write-Host "📝 Archivo de datos encontrado: $seedFile"
Write-Host "📡 API URL: $ApiUrl`n"

# Leer el archivo JSON
$carsJson = Get-Content $seedFile -Raw
$carsData = $carsJson | ConvertFrom-Json

Write-Host "📊 Vehículos a importar: $($carsData.Count)" -ForegroundColor Yellow

# Mostrar lista de vehículos
Write-Host "`n🚙 Vehículos a cargar:" -ForegroundColor Cyan
$carsData | ForEach-Object { Write-Host "   ✓ $($_.name) - `$$($_.price)" }

# Confirmar continuación
$confirm = Read-Host "`n¿Continuar con la importación? (s/n)"
if ($confirm -ne "s" -and $confirm -ne "S") {
    Write-Host "❌ Importación cancelada" -ForegroundColor Yellow
    exit 0
}

# Intentar subir el archivo
Write-Host "`n⏳ Enviando archivo al servidor..." -ForegroundColor Cyan

try {
    $headers = @{
        "Authorization" = "Bearer $JwtToken"
    }
    
    $response = Invoke-WebRequest `
        -Uri "$ApiUrl/products/import" `
        -Method Post `
        -Headers $headers `
        -InFile $seedFile `
        -ContentType "application/json" `
        -ErrorAction Stop
    
    Write-Host "✅ Importación exitosa!" -ForegroundColor Green
    Write-Host "📦 Respuesta del servidor: $($response.StatusCode)`n"
    
    if ($response.Content) {
        $responseData = $response.Content | ConvertFrom-Json
        Write-Host "📋 Detalles:" -ForegroundColor Cyan
        Write-Host $responseData | Format-List
    }
}
catch {
    Write-Host "❌ Error en la importación:" -ForegroundColor Red
    Write-Host $_.Exception.Message
    Write-Host "`n💡 Posibles causas:" -ForegroundColor Yellow
    Write-Host "   • El servidor no está corriendo (http://localhost:5290)"
    Write-Host "   • El token JWT es inválido o no tiene permisos admin"
    Write-Host "   • El archivo seed-cars.json tiene formato inválido"
    exit 1
}

Write-Host "`n🎉 Vehículos cargados exitosamente en el sistema" -ForegroundColor Green
Write-Host "Accede a http://localhost:5173 para ver los productos" -ForegroundColor Cyan
