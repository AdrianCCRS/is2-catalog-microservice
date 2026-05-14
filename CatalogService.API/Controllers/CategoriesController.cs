using CatalogService.API.DTOs;
using CatalogService.API.Filters;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de Categorías (HU-07)
    /// </summary>
    [ApiController]
    [Route("api/v1/catalog/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;

        public CategoriesController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Obtener todas las categorías activas
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var categories = await _repository.GetAllAsync(page, pageSize);
            return Ok(new
            {
                page,
                pageSize,
                results = categories
            });
        }

        /// <summary>
        /// Obtener categorías raíz (sin padre)
        /// </summary>
        [HttpGet("root")]
        public async Task<IActionResult> GetRootCategories()
        {
            var categories = await _repository.GetRootCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Obtener subcategorías de una categoría padre
        /// </summary>
        [HttpGet("{parentId}/subcategories")]
        public async Task<IActionResult> GetSubCategories(string parentId)
        {
            var parent = await _repository.GetByIdAsync(parentId);
            if (parent == null)
                return NotFound(new { message = $"Categoría padre con id '{parentId}' no encontrada." });

            var subCategories = await _repository.GetSubCategoriesAsync(parentId);
            return Ok(subCategories);
        }

        /// <summary>
        /// Obtener categoría por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var category = await _repository.GetByIdAsync(id);

            if (category == null)
                return NotFound(new { message = $"Categoría con id '{id}' no encontrada." });

            return Ok(category);
        }

        /// <summary>
        /// Obtener categoría por slug
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var category = await _repository.GetBySlugAsync(slug);

            if (category == null)
                return NotFound(new { message = $"Categoría con slug '{slug}' no encontrada." });

            return Ok(category);
        }

        /// <summary>
        /// Crear nueva categoría
        /// </summary>
        [HttpPost]
        [AdminAuthorize]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "El nombre de la categoría es obligatorio." });

            if (string.IsNullOrWhiteSpace(request.Slug))
                return BadRequest(new { message = "El slug es obligatorio." });

            // Validar slug único
            if (await _repository.SlugExistsAsync(request.Slug))
                return BadRequest(new { message = $"El slug '{request.Slug}' ya existe." });

            // Crear categoría
            var category = new Category
            {
                Name = request.Name,
                Slug = request.Slug.ToLower(),
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                ParentId = request.ParentId,
                IsActive = true
            };

            await _repository.CreateAsync(category);

            return CreatedAtAction(
                nameof(GetById),
                new { id = category.Id },
                category
            );
        }

        /// <summary>
        /// Actualizar categoría
        /// </summary>
        [HttpPut("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCategoryRequest request)
        {
            var existingCategory = await _repository.GetByIdAsync(id);

            if (existingCategory == null)
                return NotFound(new { message = $"Categoría con id '{id}' no encontrada." });

            // Actualizar campos
            if (!string.IsNullOrWhiteSpace(request.Name))
                existingCategory.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Slug))
            {
                // Validar nuevo slug (permitir el mismo solo si es el actual)
                if (request.Slug != existingCategory.Slug && await _repository.SlugExistsAsync(request.Slug, id))
                    return BadRequest(new { message = $"El slug '{request.Slug}' ya existe." });

                existingCategory.Slug = request.Slug.ToLower();
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
                existingCategory.Description = request.Description;

            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
                existingCategory.ImageUrl = request.ImageUrl;

            if (!string.IsNullOrWhiteSpace(request.ParentId))
                existingCategory.ParentId = request.ParentId;

            existingCategory.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existingCategory);

            return Ok(new { message = "Categoría actualizada exitosamente.", category = existingCategory });
        }

        /// <summary>
        /// Eliminar categoría
        /// </summary>
        [HttpDelete("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> Delete(string id)
        {
            var existingCategory = await _repository.GetByIdAsync(id);

            if (existingCategory == null)
                return NotFound(new { message = $"Categoría con id '{id}' no encontrada." });

            // Validar que no tenga productos activos asociados
            // (Este check se haría integrando con ProductRepository)
            // Por ahora, solo hacemos borrado lógico

            await _repository.DeleteAsync(id);

            return NoContent();
        }
    }
}
