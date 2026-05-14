using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CatalogService.Domain.Entities
{
    /// <summary>
    /// Entidad para registrar histórico de auditoría (HU-10)
    /// Registra cambios en productos, categorías y otras entidades
    /// </summary>
    public class AuditLog
    {
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Tipo de entidad auditada (Product, Category, etc.)
        /// </summary>
        [BsonElement("entityType")]
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// ID de la entidad que fue modificada
        /// </summary>
        [BsonElement("entityId")]
        public string EntityId { get; set; } = string.Empty;

        /// <summary>
        /// Acción realizada: Create, Update, Delete
        /// </summary>
        [BsonElement("action")]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Usuario que realizó la acción
        /// </summary>
        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del usuario para trazabilidad
        /// </summary>
        [BsonElement("userName")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp de la acción
        /// </summary>
        [BsonElement("timestamp")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// IP del cliente que realizó la acción
        /// </summary>
        [BsonElement("ipAddress")]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Cambios realizados (JSON con antes/después)
        /// Ejemplo: { "before": {...}, "after": {...} }
        /// </summary>
        [BsonElement("changes")]
        [BsonRepresentation(BsonType.Document)]
        public Dictionary<string, object?>? Changes { get; set; }

        /// <summary>
        /// Descripción adicional de la acción
        /// </summary>
        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la acción fue exitosa
        /// </summary>
        [BsonElement("isSuccessful")]
        public bool IsSuccessful { get; set; } = true;

        /// <summary>
        /// Mensaje de error si la acción falló
        /// </summary>
        [BsonElement("errorMessage")]
        public string? ErrorMessage { get; set; }
    }
}
