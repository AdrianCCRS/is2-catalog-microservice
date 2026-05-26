namespace CatalogService.API.DTOs
{
    /// <summary>
    /// Request DTO para crear un producto (HU-03)
    /// </summary>
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public double Rating { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}
