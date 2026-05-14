using CatalogService.API.DTOs;
using CatalogService.API.Filters;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using CatalogService.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace CatalogService.API.Controllers
{
    [ApiController]
    [Route("api/v1/catalog/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IImageValidator _imageValidator;
        private readonly IImportService _importService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductRepository repository,
            IImageValidator imageValidator,
            IImportService importService,
            IEventPublisher eventPublisher,
            ILogger<ProductsController> logger)
        {
            _repository = repository;
            _imageValidator = imageValidator;
            _importService = importService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        // HU-01 — Listar productos
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                // HU-05 — Búsqueda por texto
                var searchResults = await _repository.SearchAsync(search, page, pageSize);
                return Ok(new
                {
                    page,
                    pageSize,
                    search,
                    results = searchResults
                });
            }

            // Listado normal
            var products = await _repository.GetAllAsync(page, pageSize);
            return Ok(new
            {
                page,
                pageSize,
                results = products
            });
        }

        // HU-02 — Ver detalle de un producto
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return NotFound(new { message = $"Producto con id '{id}' no encontrado." });

            return Ok(product);
        }

        // HU-03 — Crear producto (POST)
        [HttpPost]
        [AdminAuthorize]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            try
            {
                // Validación básica
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { message = "El nombre del producto es obligatorio." });

                if (request.Price <= 0)
                    return BadRequest(new { message = "El precio debe ser mayor a 0." });

                if (request.Stock < 0)
                    return BadRequest(new { message = "El stock no puede ser negativo." });

                // HU-08 — Validar imágenes
                if (request.Images != null && request.Images.Count > 0)
                {
                    var (isValid, errorMessage) = _imageValidator.ValidateImageUrls(request.Images);
                    if (!isValid)
                        return BadRequest(new { message = errorMessage });
                }

                // Crear objeto Product
                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Stock = request.Stock,
                    CategoryId = request.CategoryId,
                    Images = request.Images,
                    Tags = request.Tags,
                    CreatedBy = request.CreatedBy,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _repository.CreateAsync(product);

                // RF-10: Publicar evento ProductCreated
                try
                {
                    var productCreatedEvent = new ProductCreatedEvent
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        CreatedBy = product.CreatedBy
                    };
                    await _eventPublisher.PublishAsync(productCreatedEvent);
                    _logger.LogInformation("Evento ProductCreated publicado para producto {ProductId}", product.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publicando evento ProductCreated para producto {ProductId}", product.Id);
                    // No fallar la operación si el evento falla
                }

                _logger.LogInformation("Producto creado: {ProductId} - {ProductName}", product.Id, product.Name);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = product.Id },
                    product
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // HU-04 — Actualizar producto (PUT - actualización completa)
        [HttpPut("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductRequest request)
        {
            try
            {
                var existingProduct = await _repository.GetByIdAsync(id);

                if (existingProduct == null)
                    return NotFound(new { message = $"Producto con id '{id}' no encontrado." });

                // Guardar valores anteriores para auditoría
                var previousPrice = existingProduct.Price;
                var previousStock = existingProduct.Stock;

                // Actualizar campos proporcionados
                if (!string.IsNullOrWhiteSpace(request.Name))
                    existingProduct.Name = request.Name;

                if (!string.IsNullOrWhiteSpace(request.Description))
                    existingProduct.Description = request.Description;

                if (request.Price.HasValue && request.Price > 0)
                    existingProduct.Price = request.Price.Value;

                if (request.Stock.HasValue && request.Stock >= 0)
                    existingProduct.Stock = request.Stock.Value;

                if (!string.IsNullOrWhiteSpace(request.CategoryId))
                    existingProduct.CategoryId = request.CategoryId;

                if (request.Images != null)
                    existingProduct.Images = request.Images;

                if (request.Tags != null)
                    existingProduct.Tags = request.Tags;

                existingProduct.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(existingProduct);

                // RF-10: Publicar evento ProductUpdated
                try
                {
                    var productUpdatedEvent = new ProductUpdatedEvent
                    {
                        ProductId = existingProduct.Id,
                        Name = existingProduct.Name,
                        Price = existingProduct.Price,
                        UpdatedBy = "admin"
                    };
                    await _eventPublisher.PublishAsync(productUpdatedEvent);
                    _logger.LogInformation("Evento ProductUpdated publicado para producto {ProductId}", existingProduct.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publicando evento ProductUpdated para producto {ProductId}", existingProduct.Id);
                }

                _logger.LogInformation("Producto actualizado: {ProductId}", id);

                return Ok(new { message = "Producto actualizado exitosamente.", product = existingProduct });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {ProductId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // HU-04 — Actualizar producto (PATCH - actualización parcial)
        [HttpPatch("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> PartialUpdate(string id, [FromBody] UpdateProductRequest request)
        {
            var existingProduct = await _repository.GetByIdAsync(id);

            if (existingProduct == null)
                return NotFound(new { message = $"Producto con id '{id}' no encontrado." });

            // Solo actualizar campos que fueron proporcionados
            if (!string.IsNullOrWhiteSpace(request.Name))
                existingProduct.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Description))
                existingProduct.Description = request.Description;

            if (request.Price.HasValue && request.Price > 0)
                existingProduct.Price = request.Price.Value;

            if (request.Stock.HasValue && request.Stock >= 0)
                existingProduct.Stock = request.Stock.Value;

            if (!string.IsNullOrWhiteSpace(request.CategoryId))
                existingProduct.CategoryId = request.CategoryId;

            if (request.Images != null)
                existingProduct.Images = request.Images;

            if (request.Tags != null)
                existingProduct.Tags = request.Tags;

            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existingProduct);

            return Ok(new { message = "Producto actualizado parcialmente.", product = existingProduct });
        }

        // HU-06 — Eliminar producto (DELETE - borrado lógico)
        [HttpDelete("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existingProduct = await _repository.GetByIdAsync(id);

                if (existingProduct == null)
                    return NotFound(new { message = $"Producto con id '{id}' no encontrado." });

                await _repository.DeleteAsync(id);

                // RF-10: Publicar evento ProductDeleted
                try
                {
                    var productDeletedEvent = new ProductDeletedEvent
                    {
                        ProductId = existingProduct.Id,
                        Name = existingProduct.Name,
                        DeletedBy = "admin"
                    };
                    await _eventPublisher.PublishAsync(productDeletedEvent);
                    _logger.LogInformation("Evento ProductDeleted publicado para producto {ProductId}", existingProduct.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publicando evento ProductDeleted para producto {ProductId}", existingProduct.Id);
                }

                _logger.LogInformation("Producto eliminado: {ProductId}", id);

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {ProductId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // HU-11 — Importación masiva de productos (CSV/JSON)
        [HttpPost("import")]
        [AdminAuthorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ImportProducts([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "El archivo es requerido" });

                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (fileExtension == ".csv")
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var result = await _importService.ImportFromCsvAsync(stream);
                        _logger.LogInformation("Importación CSV completada: {Success} exitosos, {Failed} fallos", 
                            result.SuccessfulImports, result.FailedImports);

                        var response = new ImportResponseDto
                        {
                            TotalRows = result.TotalRows,
                            SuccessfulImports = result.SuccessfulImports,
                            FailedImports = result.FailedImports,
                            Errors = result.Errors.Select(e => new ImportErrorDto
                            {
                                RowNumber = e.RowNumber,
                                Message = e.Message,
                                Data = e.Data
                            }).ToList(),
                            ImportedProducts = result.ImportedProducts.Select(p => new ImportedProductDto
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                Price = p.Price,
                                Stock = p.Stock
                            }).ToList()
                        };

                        return Ok(response);
                    }
                }
                else if (fileExtension == ".json")
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var result = await _importService.ImportFromJsonAsync(stream);
                        _logger.LogInformation("Importación JSON completada: {Success} exitosos, {Failed} fallos", 
                            result.SuccessfulImports, result.FailedImports);

                        var response = new ImportResponseDto
                        {
                            TotalRows = result.TotalRows,
                            SuccessfulImports = result.SuccessfulImports,
                            FailedImports = result.FailedImports,
                            Errors = result.Errors.Select(e => new ImportErrorDto
                            {
                                RowNumber = e.RowNumber,
                                Message = e.Message,
                                Data = e.Data
                            }).ToList(),
                            ImportedProducts = result.ImportedProducts.Select(p => new ImportedProductDto
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                Price = p.Price,
                                Stock = p.Stock
                            }).ToList()
                        };

                        return Ok(response);
                    }
                }
                else
                {
                    return BadRequest(new { message = "Solo se permiten archivos CSV o JSON" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante importación de productos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // Endpoint para cargar 20 carros de demostración
        [HttpPost("bulk/demo")]
        public async Task<IActionResult> BulkInsertDemoCars()
        {
            // Limpiar datos antiguos
            await _repository.DeleteAllAsync();

            var cars = new List<CreateProductRequest>
            {
                new() { Name = "Chevrolet Spark GT", Description = "Auto económico, perfecto para ciudad con motor 1.2L y 94 HP", Price = 28500000, Stock = 5, CategoryId = "", Images = new List<string> { "/images/Chevrolet%20Spark%20GT.jpg" }, Tags = new List<string> { "económico", "ciudad", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Hyundai i10 GLS", Description = "Compacto confiable, excelente consumo de combustible, 1.2L 82 HP", Price = 32000000, Stock = 3, CategoryId = "", Images = new List<string> { "/images/Hyundai%20i10%20GLS.jpg" }, Tags = new List<string> { "económico", "confiable", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Renault Twingo Zen", Description = "Urbano y versátil, motor 1.0L, perfecto para desplazamientos diarios", Price = 30500000, Stock = 4, CategoryId = "", Images = new List<string> { "/images/renault%20twingo%20zen.jpeg" }, Tags = new List<string> { "urbano", "económico", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Toyota Yaris 2024", Description = "Sedán compacto híbrido, motor 1.5L, tecnología Toyota confiable", Price = 42000000, Stock = 6, CategoryId = "", Images = new List<string> { "/images/toyota%20yaris%202024.jpg" }, Tags = new List<string> { "confiable", "híbrido", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Kia Picanto 2024", Description = "City car deportivo, 1.2L, diseño moderno y espacioso", Price = 35000000, Stock = 5, CategoryId = "", Images = new List<string> { "/images/Kia-Picanto-2024.jpg" }, Tags = new List<string> { "moderno", "ciudad", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Honda City 2023", Description = "Sedán ágil y confiable, motor 1.5L, excelente desempeño en ciudad", Price = 48000000, Stock = 4, CategoryId = "", Images = new List<string> { "/images/Honda%20City%202023.jpg" }, Tags = new List<string> { "confiable", "ciudad", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Mazda 3 2024", Description = "Sedán deportivo, motor 2.0L, dinámica superior y diseño premium", Price = 62000000, Stock = 3, CategoryId = "", Images = new List<string> { "/images/Mazda%203%202024.jpeg" }, Tags = new List<string> { "deportivo", "premium", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Volkswagen Polo Track 2024", Description = "Sedán alemán compacto, motor 1.6L, tecnología y calidad VW", Price = 58000000, Stock = 4, CategoryId = "", Images = new List<string> { "/images/Volkswagen-Polo-Track-2024.jpg" }, Tags = new List<string> { "alemán", "premium", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Ford Fiesta 2023", Description = "Sedán ágil americano, motor 1.6L, excelente relación precio-rendimiento", Price = 45000000, Stock = 5, CategoryId = "", Images = new List<string> { "/images/Ford-Fiesta-2023.jpg" }, Tags = new List<string> { "ágil", "económico", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Chevrolet Cruze 2024", Description = "Sedán mediano, motor 1.8L, conectividad y seguridad avanzada", Price = 55000000, Stock = 3, CategoryId = "", Images = new List<string> { "/images/Chevrolet-Cruze%202024.jpg" }, Tags = new List<string> { "mediano", "tecnología", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Nissan Versa 2024", Description = "Sedán espacioso y confiable, motor 1.6L, excelente maletero", Price = 46000000, Stock = 4, CategoryId = "", Images = new List<string> { "/images/Nissan-Versa%202024.jpg" }, Tags = new List<string> { "espacioso", "confiable", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Hyundai Elantra 2024", Description = "Sedán mediano moderno, motor 2.0L, garantía extendida, diseño elegante", Price = 52000000, Stock = 4, CategoryId = "", Images = new List<string> { "/images/Hyundai%20Elantra%202024.jpg" }, Tags = new List<string> { "moderno", "garantía", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Toyota Corolla 2024", Description = "Sedán legendario, motor 1.8L híbrido, confiabilidad garantizada", Price = 68000000, Stock = 2, CategoryId = "", Images = new List<string> { "/images/Toyota%20Corolla%202024.jpg" }, Tags = new List<string> { "legendario", "híbrido", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Honda Accord 2024", Description = "Sedán ejecutivo, motor 2.0L turbo, confort y tecnología premium", Price = 85000000, Stock = 2, CategoryId = "", Images = new List<string> { "/images/Honda%20accord%202024.jpg" }, Tags = new List<string> { "ejecutivo", "premium", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Nissan Sentra 2024", Description = "Sedán mediano eficiente, motor 2.0L, tecnología inteligente integrada", Price = 51000000, Stock = 3, CategoryId = "", Images = new List<string> { "/images/Nissan-Sentra-2024.jpg" }, Tags = new List<string> { "eficiente", "inteligente", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Volkswagen Passat 2024", Description = "Sedán premium alemán, motor 2.0L turbo, lujo y dinamismo", Price = 95000000, Stock = 2, CategoryId = "", Images = new List<string> { "/images/Volswagen%20Passat%202024.jpg" }, Tags = new List<string> { "premium", "alemán", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Ford Fusion 2023", Description = "Sedán híbrido americano, motor 2.0L, eficiencia y confiabilidad", Price = 72000000, Stock = 3, CategoryId = "", Images = new List<string> { "/images/Ford%20Fusion%202023.jpg" }, Tags = new List<string> { "híbrido", "americano", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Kia Optima 2024", Description = "Sedán deportivo de lujo, motor 2.0L, diseño sofisticado y dinámico", Price = 78000000, Stock = 2, CategoryId = "", Images = new List<string> { "/images/Kia%20Optima%202024.jpg" }, Tags = new List<string> { "deportivo", "lujo", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Hyundai Sonata 2024", Description = "Sedán premium coreano, motor 2.4L, confort excepcional, tecnología 5G", Price = 98000000, Stock = 1, CategoryId = "", Images = new List<string> { "/images/Hyundai%20Sonata%202024.jpg" }, Tags = new List<string> { "premium", "confort", "nuevo" }, CreatedBy = "admin@autocatalog.com" },
                new() { Name = "Chevrolet Aveo 2023", Description = "Sedán compacto americano, motor 1.6L, precio accesible, espacioso", Price = 38000000, Stock = 5, CategoryId = "", Images = new List<string> { "/images/Chevrolet-Aveo-2023.jpg" }, Tags = new List<string> { "accesible", "espacioso", "nuevo" }, CreatedBy = "admin@autocatalog.com" }
            };

            try
            {
                int createdCount = 0;
                foreach (var carRequest in cars)
                {
                    // Crear objeto Product
                    var product = new Product
                    {
                        Name = carRequest.Name,
                        Description = carRequest.Description,
                        Price = carRequest.Price,
                        Stock = carRequest.Stock,
                        CategoryId = carRequest.CategoryId,
                        Images = carRequest.Images,
                        Tags = carRequest.Tags,
                        CreatedBy = carRequest.CreatedBy,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _repository.CreateAsync(product);
                    createdCount++;
                }

                _logger.LogInformation("Se cargaron {Count} carros de demostración exitosamente", createdCount);
                return Ok(new { message = $"Se cargaron {createdCount} carros de demostración exitosamente", count = createdCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la carga de carros de demostración");
                return StatusCode(500, new { message = "Error al cargar los carros de demostración", error = ex.Message });
            }
        }

        // Endpoint para cargar imágenes
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided");

            // Validar tipo de archivo
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type");

            // Crear carpeta si no existe
            var uploadsFolder = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generar nombre único
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Devolver URL
            var imageUrl = $"http://localhost:5290/images/{fileName}";
            return Ok(new { url = imageUrl });
        }
    }
}