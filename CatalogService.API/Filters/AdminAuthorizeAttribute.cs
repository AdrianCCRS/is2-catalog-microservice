using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CatalogService.API.Filters
{
    /// <summary>
    /// Atributo para validar que el usuario tiene rol admin (RNF-04)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Validar que el usuario esté autenticado
            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedObjectResult(
                    new { message = "Token JWT es requerido" }
                );
                return;
            }

            // Validar que tenga rol admin
            var adminClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            if (adminClaim != "admin" && !user.IsInRole("admin"))
            {
                context.Result = new ObjectResult(
                    new { message = "Se requiere rol 'admin' para esta operación" }
                ) { StatusCode = 403 };
                return;
            }

            await Task.CompletedTask;
        }
    }
}
