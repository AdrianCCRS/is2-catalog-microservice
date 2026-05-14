using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using CatalogService.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CatalogService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;

        public ProductRepository(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _products = database.GetCollection<Product>(
                settings.Value.ProductsCollectionName
            );

            // Índice de texto para búsqueda (HU-05)
            var indexKeys = Builders<Product>.IndexKeys
                .Text(p => p.Name)
                .Text(p => p.Description);
            _products.Indexes.CreateOne(
                new CreateIndexModel<Product>(indexKeys)
            );
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            return await _products
                .Find(p => p.Id == id && p.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize)
        {
            return await _products
                .Find(p => p.IsActive)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(string searchText, int page, int pageSize)
        {
            // Búsqueda de texto usando índice de texto creado en el constructor (HU-05)
            var filter = Builders<Product>.Filter.Text(searchText) & 
                         Builders<Product>.Filter.Eq(p => p.IsActive, true);
            
            return await _products
                .Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task CreateAsync(Product product)
        {
            await _products.InsertOneAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
        }

        public async Task DeleteAsync(string id)
        {
            // Borrado lógico: solo marca isActive = false
            var update = Builders<Product>.Update
                .Set(p => p.IsActive, false)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);
            await _products.UpdateOneAsync(p => p.Id == id, update);
        }

        public async Task DeleteAllAsync()
        {
            // Borrado físico para limpiar colección completamente
            var filter = Builders<Product>.Filter.Empty;
            await _products.DeleteManyAsync(filter);
        }
    }
}
