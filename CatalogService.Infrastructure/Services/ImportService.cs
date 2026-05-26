using CatalogService.Domain.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CatalogService.Infrastructure.Services
{
    /// <summary>
    /// Servicio para importación masiva de productos (HU-11)
    /// Soporta CSV y JSON
    /// </summary>
    public interface IImportService
    {
        Task<ImportResult> ImportFromCsvAsync(Stream csvStream);
        Task<ImportResult> ImportFromJsonAsync(Stream jsonStream);
    }

    public class ImportResult
    {
        public int TotalRows { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public List<ImportError> Errors { get; set; } = new();
        public List<ImportedProduct> ImportedProducts { get; set; } = new();
    }

    public class ImportError
    {
        public int? RowNumber { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, object?>? Data { get; set; }
    }

    public class ImportedProduct
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    public class ImportCsvRow
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty; // Comma-separated
        public string Images { get; set; } = string.Empty; // Comma-separated
    }

    public class ImportService : IImportService
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageValidator _imageValidator;
        private readonly ILogger<ImportService> _logger;

        public ImportService(
            IProductRepository productRepository,
            IImageValidator imageValidator,
            ILogger<ImportService> logger)
        {
            _productRepository = productRepository;
            _imageValidator = imageValidator;
            _logger = logger;
        }

        public async Task<ImportResult> ImportFromCsvAsync(Stream csvStream)
        {
            var result = new ImportResult();

            try
            {
                using (var reader = new StreamReader(csvStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<ImportCsvRowMap>();
                    var records = csv.GetRecords<ImportCsvRow>().ToList();
                    result.TotalRows = records.Count;

                    int rowNumber = 2; // Empezar en 2 (1 es header)
                    foreach (var record in records)
                    {
                        try
                        {
                            await ImportProductAsync(record, result, rowNumber);
                            result.SuccessfulImports++;
                        }
                        catch (Exception ex)
                        {
                            result.FailedImports++;
                            result.Errors.Add(new ImportError
                            {
                                RowNumber = rowNumber,
                                Message = ex.Message,
                                Data = new Dictionary<string, object?> { { "name", record.Name } }
                            });
                            _logger.LogWarning(ex, "Error importando fila {RowNumber}", rowNumber);
                        }
                        rowNumber++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar archivo CSV");
                result.Errors.Add(new ImportError { Message = $"Error al procesar CSV: {ex.Message}" });
            }

            return result;
        }

        public async Task<ImportResult> ImportFromJsonAsync(Stream jsonStream)
        {
            var result = new ImportResult();

            try
            {
                using (var reader = new StreamReader(jsonStream))
                {
                    var json = await reader.ReadToEndAsync();
                    var products = JsonSerializer.Deserialize<List<ImportJsonProduct>>(json, 
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                    if (products == null || products.Count == 0)
                    {
                        result.Errors.Add(new ImportError { Message = "No se encontraron productos en el JSON" });
                        return result;
                    }

                    result.TotalRows = products.Count;

                    int rowNumber = 1;
                    foreach (var product in products)
                    {
                        try
                        {
                            var csvRow = new ImportCsvRow
                            {
                                Name = product.Name ?? string.Empty,
                                Description = product.Description ?? string.Empty,
                                Price = product.Price,
                                Stock = product.Stock,
                                CategoryId = product.CategoryId ?? string.Empty,
                                Tags = product.Tags != null ? string.Join(",", product.Tags) : string.Empty,
                                Images = product.Images != null ? string.Join(",", product.Images) : string.Empty
                            };

                            await ImportProductAsync(csvRow, result, rowNumber);
                            result.SuccessfulImports++;
                        }
                        catch (Exception ex)
                        {
                            result.FailedImports++;
                            result.Errors.Add(new ImportError
                            {
                                RowNumber = rowNumber,
                                Message = ex.Message,
                                Data = new Dictionary<string, object?> { { "name", product.Name } }
                            });
                            _logger.LogWarning(ex, "Error importando producto JSON {RowNumber}", rowNumber);
                        }
                        rowNumber++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar archivo JSON");
                result.Errors.Add(new ImportError { Message = $"Error al procesar JSON: {ex.Message}" });
            }

            return result;
        }

        private async Task ImportProductAsync(ImportCsvRow row, ImportResult result, int rowNumber)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(row.Name))
                throw new ArgumentException("El nombre del producto es requerido");

            if (row.Price <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0");

            if (row.Stock < 0)
                throw new ArgumentException("El stock no puede ser negativo");

            // Parsear imágenes
            var images = new List<string>();
            if (!string.IsNullOrWhiteSpace(row.Images))
            {
                images = row.Images
                    .Split(',')
                    .Select(i => i.Trim())
                    .Where(i => !string.IsNullOrWhiteSpace(i))
                    .ToList();
            }

            // Validar imágenes
            if (images.Count > 0)
            {
                var (isValid, errorMessage) = _imageValidator.ValidateImageUrls(images, 0, 5);
                if (!isValid)
                    throw new ArgumentException(errorMessage);
            }

            // Parsear tags
            var tags = new List<string>();
            if (!string.IsNullOrWhiteSpace(row.Tags))
            {
                tags = row.Tags
                    .Split(',')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList();
            }

            // Crear producto
            var product = new Domain.Entities.Product
            {
                Name = row.Name.Trim(),
                Description = row.Description?.Trim() ?? string.Empty,
                Price = row.Price,
                Stock = row.Stock,
                CategoryId = !string.IsNullOrWhiteSpace(row.CategoryId) ? row.CategoryId : string.Empty,
                Images = images,
                Tags = tags,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "import_system"
            };

            // Crear en BD
            await _productRepository.CreateAsync(product);

            result.ImportedProducts.Add(new ImportedProduct
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock
            });

            _logger.LogInformation("Producto importado: {ProductId} {ProductName}", product.Id, product.Name);
        }
    }

    public class ImportJsonProduct
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? CategoryId { get; set; }
        public List<string>? Tags { get; set; }
        public List<string>? Images { get; set; }
    }

    public class ImportCsvRowMap : CsvHelper.Configuration.ClassMap<ImportCsvRow>
    {
        public ImportCsvRowMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.Description).Name("Description");
            Map(m => m.Price).Name("Price");
            Map(m => m.Stock).Name("Stock");
            Map(m => m.CategoryId).Name("CategoryId").Optional();
            Map(m => m.Tags).Name("Tags").Optional();
            Map(m => m.Images).Name("Images").Optional();
        }
    }
}
