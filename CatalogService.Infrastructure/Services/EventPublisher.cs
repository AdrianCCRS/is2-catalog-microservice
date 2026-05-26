using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CatalogService.Infrastructure.Services
{
    public abstract class DomainEvent
    {
        public string EventId { get; } = Guid.NewGuid().ToString();

        [JsonIgnore]
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }

    public class ProductEventData
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Price { get; set; }
        public double Rating { get; set; }
        public bool Available { get; set; }
        public string Brand { get; set; } = string.Empty;
    }

    public class ProductCreatedEvent : DomainEvent
    {
        public string EventType { get; set; } = "ProductCreated";
        public string Version { get; set; } = "1.0";
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
        public ProductEventData Data { get; set; } = new();
    }

    public class ProductUpdatedEvent : DomainEvent
    {
        public string EventType { get; set; } = "ProductUpdated";
        public string Version { get; set; } = "1.0";
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
        public ProductEventData Data { get; set; } = new();
    }

    public class ProductDeletedEvent : DomainEvent
    {
        public string EventType { get; set; } = "ProductDeleted";
        public string Version { get; set; } = "1.0";
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event, string? routingKey = null) where T : DomainEvent;
    }

    public class EventPublisher : IEventPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly ILogger<EventPublisher> _logger;
        private const string ExchangeName = "catalog.events";

        public EventPublisher(IConfiguration configuration, ILogger<EventPublisher> logger)
        {
            _logger = logger;
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:Username"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            try
            {
                _connection = factory.CreateConnection();
                using var channel = _connection.CreateModel();
                channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
                _logger.LogInformation("RabbitMQ connection established. Exchange '{Exchange}' declared.", ExchangeName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ at {Host}:{Port}",
                    factory.HostName, factory.Port);
                throw;
            }
        }

        public async Task PublishAsync<T>(T @event, string? routingKey = null) where T : DomainEvent
        {
            try
            {
                routingKey ??= GetRoutingKey(@event);
                var message = JsonSerializer.Serialize(@event, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var body = Encoding.UTF8.GetBytes(message);

                using var channel = _connection.CreateModel();
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.MessageId = @event.EventId;
                properties.ContentType = "application/json";

                channel.BasicPublish(ExchangeName, routingKey, properties, body);

                _logger.LogInformation(
                    "Published {EventType} with routing key {RoutingKey} to {Exchange}",
                    @event.GetType().Name, routingKey, ExchangeName);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing event {EventType}", @event.GetType().Name);
            }
        }

        private static string GetRoutingKey<T>(T @event) where T : DomainEvent => @event switch
        {
            ProductCreatedEvent => "product.created",
            ProductUpdatedEvent => "product.updated",
            ProductDeletedEvent => "product.deleted",
            _ => @event.GetType().Name.ToLower()
        };

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
