using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CatalogService.Infrastructure.Services
{
    /// <summary>
    /// Configuración de JWT
    /// </summary>
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 60;
    }

    /// <summary>
    /// Interfaz para manejo de JWT
    /// </summary>
    public interface IJwtService
    {
        string GenerateToken(string userId, string userName, List<string> roles);
        ClaimsPrincipal? ValidateToken(string token);
        Dictionary<string, object?> ExtractClaims(string token);
    }

    /// <summary>
    /// Servicio de JWT para autenticación
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<JwtService> _logger;

        public JwtService(JwtSettings jwtSettings, ILogger<JwtService> logger)
        {
            _jwtSettings = jwtSettings;
            _logger = logger;
        }

        public string GenerateToken(string userId, string userName, List<string> roles)
        {
            // Este es un placeholder. En producción, usarías System.IdentityModel.Tokens.Jwt
            // Para ahora, esto es solo una estructura de demostración
            var claims = new Dictionary<string, object?>
            {
                { "sub", userId },
                { "name", userName },
                { "roles", roles },
                { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                { "exp", DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes).ToUnixTimeSeconds() }
            };

            _logger.LogInformation("Token generado para usuario: {UserId}", userId);
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(claims)));
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var claims = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(json);

                if (claims == null)
                    return null;

                var claimsList = new List<Claim>();
                foreach (var claim in claims)
                {
                    claimsList.Add(new Claim(claim.Key, claim.Value?.ToString() ?? string.Empty));
                }

                return new ClaimsPrincipal(new ClaimsIdentity(claimsList));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error validando token JWT");
                return null;
            }
        }

        public Dictionary<string, object?> ExtractClaims(string token)
        {
            try
            {
                var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(json) 
                    ?? new Dictionary<string, object?>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo claims del token");
                return new Dictionary<string, object?>();
            }
        }
    }
}
