using CatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogService.Domain.Interfaces
{
    /// <summary>
    /// Interfaz para operaciones de auditoría (HU-10)
    /// </summary>
    public interface IAuditLogRepository
    {
        /// <summary>
        /// Registra un evento de auditoría
        /// </summary>
        Task CreateAsync(AuditLog auditLog);

        /// <summary>
        /// Obtiene histórico de auditoría paginado
        /// </summary>
        Task<IEnumerable<AuditLog>> GetAllAsync(int page = 1, int pageSize = 20);

        /// <summary>
        /// Obtiene histórico de una entidad específica
        /// </summary>
        Task<IEnumerable<AuditLog>> GetByEntityIdAsync(string entityId, int page = 1, int pageSize = 20);

        /// <summary>
        /// Obtiene histórico de un tipo de entidad
        /// </summary>
        Task<IEnumerable<AuditLog>> GetByEntityTypeAsync(string entityType, int page = 1, int pageSize = 20);

        /// <summary>
        /// Obtiene histórico de un usuario
        /// </summary>
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 20);

        /// <summary>
        /// Obtiene histórico en rango de fechas
        /// </summary>
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to, int page = 1, int pageSize = 20);

        /// <summary>
        /// Obtiene el total de registros de auditoría
        /// </summary>
        Task<long> CountAsync();

        /// <summary>
        /// Obtiene el total de registros para una entidad
        /// </summary>
        Task<long> CountByEntityIdAsync(string entityId);
    }
}
