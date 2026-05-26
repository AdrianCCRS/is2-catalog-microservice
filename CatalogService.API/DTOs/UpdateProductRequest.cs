namespace CatalogService.API.DTOs
{
    /// <summary>
    /// Request DTO para actualizar un producto (HU-04)
    /// </summary>
    public class UpdateProductRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public string? CategoryId { get; set; }
        public List<string>? Images { get; set; }
        public List<string>? Tags { get; set; }
        public double? Rating { get; set; }
        public string? Brand { get; set; }
    }
}
