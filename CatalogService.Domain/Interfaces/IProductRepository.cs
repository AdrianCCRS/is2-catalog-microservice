using CatalogService.Domain.Entities;

namespace CatalogService.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(string id);
        Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<Product>> SearchAsync(string searchText, int page, int pageSize);
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(string id);
        Task DeleteAllAsync();
    }
}