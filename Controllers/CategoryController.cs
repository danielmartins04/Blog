using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Categories;
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
            return Ok(new ResultViewModel<List<Category>>(categories));
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context
        ) {
            try {
                var category = await context.Categories.FirstOrDefaultAsync(x=>x.Id == id);

                if (category == null) 
                    return NotFound(new ResultViewModel<Category>("Category not found."));
                    
                return Ok(new ResultViewModel<Category>(category));
            } catch {
                return StatusCode(500, new ResultViewModel<Category>("Internal Server Error"));
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context
        ) {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            try {
                var category = new Category 
                {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug
                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            } catch {
                return StatusCode(500, new ResultViewModel<Category>("Internal Server Error"));
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context
        ) {
            try {
                var category = await context.Categories.FirstOrDefaultAsync(x=>x.Id == id);

                if (category == null) 
                    return NotFound(new ResultViewModel<Category>("Category not found."));

                category.Name = model.Name;
                category.Slug = model.Slug;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            } catch(Exception e) {
                return StatusCode(500, new ResultViewModel<Category>("Internal Server Error."));
            }
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context
        ) {
            try {
                var category = await context.Categories.FirstOrDefaultAsync(x=>x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>("Category not found."));

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            } catch(Exception e) {
                return StatusCode(500, new ResultViewModel<Category>("Internal Server Error."));
            }
        }
    }
}