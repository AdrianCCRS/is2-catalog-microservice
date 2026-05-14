using System.Collections.Generic;

namespace CatalogService.API.DTOs
{
    /// <summary>
    /// DTO de respuesta para importación masiva (HU-11)
    /// </summary>
    public class ImportResponseDto
    {
        public int TotalRows { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public List<ImportErrorDto> Errors { get; set; } = new();
        public List<ImportedProductDto> ImportedProducts { get; set; } = new();
    }

    public class ImportErrorDto
    {
        public int? RowNumber { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, object> Data { get; set; } = new();
    }

    public class ImportedProductDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
