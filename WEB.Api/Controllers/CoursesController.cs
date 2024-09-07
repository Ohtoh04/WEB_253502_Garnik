using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB.Api.Data;
using WEB.Domain.Entities;
using WEB.Api.Services;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;

namespace WEB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase {
        private readonly IAPIProductService _productService;

        public CoursesController(IAPIProductService productService) {
            _productService = productService;
        }

        // GET: api/Courses
        [HttpGet("{category?}")]
        [AllowAnonymous] // Разрешаем неавторизованный доступ для чтения
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses(string? category, int pageNo = 1, int pageSize = 3) {
            return Ok(await _productService.GetProductListAsync(category, pageNo, pageSize));
        }

        // GET: api/Courses/5
        [HttpGet("{id:int}")]
        [AllowAnonymous] // Разрешаем неавторизованный доступ для чтения
        public async Task<ActionResult<Course>> GetCourse(int id) {
            var course = await _productService.GetProductByIdAsync(id);

            if (course == null) {
                return NotFound();
            }

            return Ok(course);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        [Authorize(Policy = "admin")] // Требуем политику «admin» для изменения
        public async Task<IActionResult> PutCourse(int id, Course course) {
            if (id != course.Id) {
                return BadRequest();
            }

            try {
                await _productService.UpdateProductAsync(id, course);
            }
            catch (Exception ex) {
                return NotFound();
            }
            return Ok();
        }

        // POST: api/Courses
        [HttpPost]
        [Authorize(Policy = "admin")] // Требуем политику «admin» для создания
        public async Task<ActionResult<Course>> PostCourse(Course course) {
            return Ok(await _productService.CreateProductAsync(course));
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "admin")] // Требуем политику «admin» для удаления
        public async Task<IActionResult> DeleteCourse(int id) {
            try {
                await _productService.DeleteProductAsync(id);
            }
            catch (Exception ex) {
                return NotFound();
            }

            return NoContent();
        }
    }

}

//[Route("api/[controller]")]
//[ApiController]
//public class CoursesController : ControllerBase {
//    private readonly IAPIProductService _productService;

//    public CoursesController(IAPIProductService productService) {
//        _productService = productService;
//    }

//    // GET: api/Courses
//    [HttpGet("{category?}")]
//    public async Task<ActionResult<IEnumerable<Course>>> GetCourses(string? category,
//    int pageNo = 1, int pageSize = 3) {
//        return Ok(await _productService.GetProductListAsync(category, pageNo, pageSize));
//    }

//    // GET: api/Courses/5
//    [HttpGet("{id:int}")]
//    public async Task<ActionResult<Course>> GetCourse(int id) {
//        var course = await _productService.GetProductByIdAsync(id);

//        if (course == null) {
//            return NotFound();
//        }

//        return Ok(course);
//    }

//    //PUT: api/Courses/5
//    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//    [HttpPut("{id}")]
//    public async Task<IActionResult> PutCourse(int id, Course course) {
//        if (id != course.Id) {
//            return BadRequest();
//        }

//        try {
//            await _productService.UpdateProductAsync(id, course);
//        }
//        catch (Exception ex) {
//            return NotFound();
//        }
//        return Ok();
//    }

//    // POST: api/Courses
//    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//    [HttpPost]
//    public async Task<ActionResult<Course>> PostCourse(Course course) {

//        return Ok(await _productService.CreateProductAsync(course));
//    }

//    // DELETE: api/Courses/5
//    [HttpDelete("{id}")]
//    public async Task<IActionResult> DeleteCourse(int id) {
//        try {
//            await _productService.DeleteProductAsync(id);
//        }
//        catch (Exception ex) {
//            return NotFound();
//        }

//        return NoContent();
//    }

//    //private bool CourseExists(int id)
//    //{
//    //    return _context.Courses.Any(e => e.ID == id);
//    //}
//}