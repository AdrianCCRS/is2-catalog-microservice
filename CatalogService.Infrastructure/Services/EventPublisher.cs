using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CatalogService.Infrastructure.Services
{
    /// <summary>
    /// Evento de dominio para publicar en RabbitMQ
    /// </summary>
    public abstract class DomainEvent
    {
        public string EventId { get; } = Guid.NewGuid().ToString();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
        public string EventType => GetType().Name;
    }

    /// <summary>
    /// Eventos de producto
    /// </summary>
    public class ProductCreatedEvent : DomainEvent
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class ProductUpdatedEvent : DomainEvent
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }

    public class ProductDeletedEvent : DomainEvent
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DeletedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Interfaz para publicar eventos a RabbitMQ
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event, string? routingKey = null) where T : DomainEvent;
    }

    /// <summary>
    /// Implementación de publicador de eventos para RabbitMQ
    /// Nota: Esta es una implementación placeholder. En producción usar RabbitMQ.Client
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly ILogger<EventPublisher> _logger;
        
        // En producción, se inyectaría IConnection de RabbitMQ.Client
        // private readonly IConnection _connection;
        // private readonly IModel _channel;

        public EventPublisher(ILogger<EventPublisher> logger)
        {
            _logger = logger;
            // En producción: inicializar conexión y canal de RabbitMQ
        }

        public async Task PublishAsync<T>(T @event, string? routingKey = null) where T : DomainEvent
        {
            try
            {
                routingKey ??= @event.EventType;
                var message = JsonSerializer.Serialize(@event, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                // En producción:
                // var body = Encoding.UTF8.GetBytes(message);
                // _channel.BasicPublish("catalog.events", routingKey, null, body);

                _logger.LogInformation(
                    "Evento publicado: {EventType} a {RoutingKey} - {EventId}",
                    @event.EventType, routingKey, @event.EventId);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publicando evento {EventType}", @event.EventType);
                // No lanzar excepción para no afectar operación principal
            }
        }
    }
}
