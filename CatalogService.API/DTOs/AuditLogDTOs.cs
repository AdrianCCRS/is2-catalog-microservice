using System;
using System.Collections.Generic;

namespace CatalogService.API.DTOs
{
    /// <summary>
    /// DTO de respuesta para registros de auditoría
    /// </summary>
    public class AuditLogResponse
    {
        public string Id { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public Dictionary<string, object?>? Changes { get; set; } = new();
        public string Description { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// DTO de respuesta paginada para auditoría
    /// </summary>
    public class AuditLogPageResponse
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
        public List<AuditLogResponse> Logs { get; set; } = new();
    }

    /// <summary>
    /// Filtros para búsqueda de auditoría
    /// </summary>
    public class AuditLogFilterRequest
    {
        public string? EntityId { get; set; }
        public string? EntityType { get; set; }
        public string? UserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
