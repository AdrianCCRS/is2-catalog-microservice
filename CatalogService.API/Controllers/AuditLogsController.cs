using CatalogService.API.DTOs;
using CatalogService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.API.Controllers
{
    /// <summary>
    /// Controlador para auditoría (HU-10)
    /// Proporciona acceso al histórico de cambios
    /// </summary>
    [ApiController]
    [Route("api/v1/audit-logs")]
    [Produces("application/json")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<AuditLogsController> _logger;

        public AuditLogsController(
            IAuditLogRepository auditLogRepository,
            ILogger<AuditLogsController> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de auditoría (paginado)
        /// </summary>
        /// <remarks>
        /// Retorna los últimos registros de auditoría en orden descendente por timestamp
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<AuditLogPageResponse>> GetAllLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var logs = await _auditLogRepository.GetAllAsync(page, pageSize);
                var total = await _auditLogRepository.CountAsync();

                var response = new AuditLogPageResponse
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    Logs = logs.Select(MapToResponse).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros de auditoría");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene auditoría por ID de entidad
        /// </summary>
        /// <remarks>
        /// Retorna todos los cambios realizados a una entidad específica
        /// </remarks>
        [HttpGet("entity/{entityId}")]
        public async Task<ActionResult<AuditLogPageResponse>> GetByEntityId(
            string entityId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entityId))
                    return BadRequest(new { message = "EntityId es requerido" });

                var logs = await _auditLogRepository.GetByEntityIdAsync(entityId, page, pageSize);
                var total = await _auditLogRepository.CountByEntityIdAsync(entityId);

                if (total == 0)
                    return NotFound(new { message = $"No hay auditoría para la entidad {entityId}" });

                var response = new AuditLogPageResponse
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    Logs = logs.Select(MapToResponse).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditoría por entityId");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene auditoría por tipo de entidad
        /// </summary>
        /// <remarks>
        /// Retorna todos los cambios para un tipo específico (ej: Product, Category)
        /// </remarks>
        [HttpGet("type/{entityType}")]
        public async Task<ActionResult<AuditLogPageResponse>> GetByEntityType(
            string entityType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entityType))
                    return BadRequest(new { message = "EntityType es requerido" });

                var logs = await _auditLogRepository.GetByEntityTypeAsync(entityType, page, pageSize);
                var total = await _auditLogRepository.CountAsync();

                var response = new AuditLogPageResponse
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    Logs = logs.Select(MapToResponse).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditoría por entityType");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene auditoría por usuario
        /// </summary>
        /// <remarks>
        /// Retorna todos los cambios realizados por un usuario específico
        /// </remarks>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<AuditLogPageResponse>> GetByUserId(
            string userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest(new { message = "UserId es requerido" });

                var logs = await _auditLogRepository.GetByUserIdAsync(userId, page, pageSize);
                var total = await _auditLogRepository.CountAsync();

                var response = new AuditLogPageResponse
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    Logs = logs.Select(MapToResponse).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditoría por userId");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene auditoría por rango de fechas
        /// </summary>
        /// <remarks>
        /// Retorna registros de auditoría dentro del rango de fechas especificado
        /// </remarks>
        [HttpGet("date-range")]
        public async Task<ActionResult<AuditLogPageResponse>> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (from > to)
                    return BadRequest(new { message = "La fecha 'from' debe ser menor que 'to'" });

                var logs = await _auditLogRepository.GetByDateRangeAsync(from, to, page, pageSize);
                var total = await _auditLogRepository.CountAsync();

                var response = new AuditLogPageResponse
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    Logs = logs.Select(MapToResponse).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditoría por rango de fechas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Mapea una entidad AuditLog a su DTO de respuesta
        /// </summary>
        private AuditLogResponse MapToResponse(Domain.Entities.AuditLog auditLog)
        {
            return new AuditLogResponse
            {
                Id = auditLog.Id.ToString(),
                EntityType = auditLog.EntityType,
                EntityId = auditLog.EntityId,
                Action = auditLog.Action,
                UserId = auditLog.UserId,
                UserName = auditLog.UserName,
                Timestamp = auditLog.Timestamp,
                IpAddress = auditLog.IpAddress,
                Changes = auditLog.Changes,
                Description = auditLog.Description,
                IsSuccessful = auditLog.IsSuccessful,
                ErrorMessage = auditLog.ErrorMessage
            };
        }
    }
}
