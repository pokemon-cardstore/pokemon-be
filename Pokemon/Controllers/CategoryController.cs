using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.ModelView;
using Services.Service.Interface;

namespace RolexApplication_Backend.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("/api/v1/categories")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategories();
            if (categories == null)
            {
                return NotFound("Categories not found");
            }
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(id);
                if (category == null) {
                    return NotFound("No category match this id");
                }
                return Ok(category);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost()]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDtoRequest request)
        {
            try
            {
                if (request == null) {
                    return BadRequest("Category cannot null");
                }
                if (request.Name.IsNullOrEmpty() || request.Name.IsNullOrEmpty())
                {
                    return BadRequest("Please fill all fields");
                }
                await _categoryService.CreateCategory(request);
                return Ok("Create category successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDtoRequest request, int id)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Category cannot null");
                }
                if (request.Name.IsNullOrEmpty() || request.Name.IsNullOrEmpty())
                {
                    return BadRequest("Please fill all fields");
                }
                await _categoryService.UpdateCategory(id, request);
                return Ok("Update category successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategory(id);
                return Ok("Delete category successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
