using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using CatalogService.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación de repositorio de auditoría en MongoDB (HU-10)
    /// </summary>
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly IMongoCollection<AuditLog> _auditLogsCollection;
        private readonly ILogger<AuditLogRepository> _logger;

        public AuditLogRepository(
            IOptions<MongoDbSettings> mongoDbSettings,
            ILogger<AuditLogRepository> logger)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _auditLogsCollection = mongoDatabase.GetCollection<AuditLog>(
                mongoDbSettings.Value.AuditLogsCollectionName ?? "auditLogs");

            _logger = logger;

            // Crear índices para búsquedas frecuentes
            CreateIndexes();
        }

        /// <summary>
        /// Crear índices de base de datos
        /// </summary>
        private void CreateIndexes()
        {
            try
            {
                // Índice en entityId para búsquedas rápidas
                var entityIdIndexModel = new CreateIndexModel<AuditLog>(
                    Builders<AuditLog>.IndexKeys.Ascending(a => a.EntityId),
                    new CreateIndexOptions { Name = "idx_entityId" });

                // Índice en userId para búsquedas por usuario
                var userIdIndexModel = new CreateIndexModel<AuditLog>(
                    Builders<AuditLog>.IndexKeys.Ascending(a => a.UserId),
                    new CreateIndexOptions { Name = "idx_userId" });

                // Índice en entityType para búsquedas por tipo
                var entityTypeIndexModel = new CreateIndexModel<AuditLog>(
                    Builders<AuditLog>.IndexKeys.Ascending(a => a.EntityType),
                    new CreateIndexOptions { Name = "idx_entityType" });

                // Índice en timestamp para búsquedas por rango de fechas
                var timestampIndexModel = new CreateIndexModel<AuditLog>(
                    Builders<AuditLog>.IndexKeys.Descending(a => a.Timestamp),
                    new CreateIndexOptions { Name = "idx_timestamp" });

                // Índice compuesto para búsquedas entity + timestamp
                var compositeIndexModel = new CreateIndexModel<AuditLog>(
                    Builders<AuditLog>.IndexKeys
                        .Ascending(a => a.EntityId)
                        .Descending(a => a.Timestamp),
                    new CreateIndexOptions { Name = "idx_entityId_timestamp" });

                _auditLogsCollection.Indexes.CreateMany(
                    new[] { entityIdIndexModel, userIdIndexModel, entityTypeIndexModel, timestampIndexModel, compositeIndexModel });

                _logger.LogInformation("Índices de auditoría creados exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear índices de auditoría");
            }
        }

        public async Task CreateAsync(AuditLog auditLog)
        {
            try
            {
                await _auditLogsCollection.InsertOneAsync(auditLog);
                _logger.LogInformation("Registro de auditoría creado: {EntityType} {EntityId} {Action}",
                    auditLog.EntityType, auditLog.EntityId, auditLog.Action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear registro de auditoría");
                throw;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                var skip = (page - 1) * pageSize;
                return await _auditLogsCollection
                    .Find(FilterDefinition<AuditLog>.Empty)
                    .Sort(Builders<AuditLog>.Sort.Descending(a => a.Timestamp))
                    .Skip(skip)
                    .Limit(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros de auditoría");
                throw;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByEntityIdAsync(string entityId, int page = 1, int pageSize = 20)
        {
            try
            {
                var filter = Builders<AuditLog>.Filter.Eq(a => a.EntityId, entityId);
                var skip = (page - 1) * pageSize;

                return await _auditLogsCollection
                    .Find(filter)
                    .Sort(Builders<AuditLog>.Sort.Descending(a => a.Timestamp))
                    .Skip(skip)
                    .Limit(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditoría por entityId: {EntityId}", entityId);
                throw;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByEntityTypeAsync(string entityType, int page = 1, int pageSize = 20)
        {
            try
            {
                var filter = Builders<AuditLog>.Filter.Eq(a => a.EntityType, entityType);
                var skip = (page - 1) * pageSize;

                return await _auditLogsCollection
                    .Find(filter)
                    .Sort(Builders<AuditLog>.Sort.Descending(a => a.Timestamp))
                    .Skip(skip)
                    .Limit(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditoría por entityType: {EntityType}", entityType);
                throw;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var filter = Builders<AuditLog>.Filter.Eq(a => a.UserId, userId);
                var skip = (page - 1) * pageSize;

                return await _auditLogsCollection
                    .Find(filter)
                    .Sort(Builders<AuditLog>.Sort.Descending(a => a.Timestamp))
                    .Skip(skip)
                    .Limit(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditoría por userId: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to, int page = 1, int pageSize = 20)
        {
            try
            {
                var filter = Builders<AuditLog>.Filter.And(
                    Builders<AuditLog>.Filter.Gte(a => a.Timestamp, from),
                    Builders<AuditLog>.Filter.Lte(a => a.Timestamp, to)
                );

                var skip = (page - 1) * pageSize;

                return await _auditLogsCollection
                    .Find(filter)
                    .Sort(Builders<AuditLog>.Sort.Descending(a => a.Timestamp))
                    .Skip(skip)
                    .Limit(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditoría por rango de fechas");
                throw;
            }
        }

        public async Task<long> CountAsync()
        {
            try
            {
                return await _auditLogsCollection.CountDocumentsAsync(FilterDefinition<AuditLog>.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar registros de auditoría");
                throw;
            }
        }

        public async Task<long> CountByEntityIdAsync(string entityId)
        {
            try
            {
                var filter = Builders<AuditLog>.Filter.Eq(a => a.EntityId, entityId);
                return await _auditLogsCollection.CountDocumentsAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar auditoría por entityId: {EntityId}", entityId);
                throw;
            }
        }
    }
}
