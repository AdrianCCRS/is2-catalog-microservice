using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace CatalogService.Infrastructure.Services
{
    /// <summary>
    /// Servicio para registrar eventos de auditoría (HU-10)
    /// </summary>
    public interface IAuditService
    {
        Task LogAsync(
            string entityType,
            string entityId,
            string action,
            string userId,
            string userName,
            string ipAddress,
            Dictionary<string, object?>? changes = null,
            string? description = null,
            bool isSuccessful = true,
            string? errorMessage = null);
    }

    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            IAuditLogRepository auditLogRepository,
            ILogger<AuditService> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task LogAsync(
            string entityType,
            string entityId,
            string action,
            string userId,
            string userName,
            string ipAddress,
            Dictionary<string, object?>? changes = null,
            string? description = null,
            bool isSuccessful = true,
            string? errorMessage = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    Action = action,
                    UserId = userId,
                    UserName = userName,
                    IpAddress = ipAddress,
                    Timestamp = DateTime.UtcNow,
                    Changes = changes,
                    Description = description ?? $"{action} {entityType}",
                    IsSuccessful = isSuccessful,
                    ErrorMessage = errorMessage
                };

                await _auditLogRepository.CreateAsync(auditLog);
                _logger.LogInformation(
                    "Evento auditado: {EntityType} {EntityId} {Action} por {UserId}",
                    entityType, entityId, action, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar evento de auditoría");
                // No lanzar excepción para que no afecte la operación principal
            }
        }
    }
}
