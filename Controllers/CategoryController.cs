using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]    
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context
        ) {
            var categories = await context.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context
        ) {
            try {
                var category = await context.Categories.FirstOrDefaultAsync(x=>x.Id == id);

                if (category == null) return NotFound();
                    
                return Ok(category);
            } catch (Exception e) {
                return StatusCode(500, "Falha interna no servidor.");
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] CreateCategoryViewModel model,
            [FromServices] BlogDataContext context
        ) {
            try {
                var category = new Category 
                {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug
                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", category);
            } catch(Exception e) {
                return StatusCode(500, "Falha interna no servidor.");
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] Category model,
            [FromServices] BlogDataContext context
        ) {
            try {
                var category = await context.Categories.FirstOrDefaultAsync(x=>x.Id == id);

                if (category == null) return NotFound();

                category.Name = model.Name;
                category.Slug = model.Slug;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(category);
            } catch(Exception e) {
                return StatusCode(500, "Falha interna no servidor.");
            }
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context
        ) {
            try {
                var category = await context.Categories.FirstOrDefaultAsync(x=>x.Id == id);

                if (category == null) return NotFound();

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(category);
            } catch(Exception e) {
                return StatusCode(500, "Falha interna no servidor.");
            }
        }
    }
}