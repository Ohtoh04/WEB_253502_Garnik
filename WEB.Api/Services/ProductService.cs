using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using WEB.Api.Data;
using WEB.Domain.Entities;
using WEB.Domain.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WEB.Api.Services
{
    public class ProductService : IAPIProductService {
        private readonly int _maxPageSize = 20;
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context) {
            _context = context;
        }

        //fin
        public async Task<ResponseData<Course>> CreateProductAsync(Course product) {
            try {
                var tempCat = new Category();
                if (product.Category != null) {
                    tempCat = product.Category;
                    product.Category = null;
                }
                _context.Courses.Add(product);
                await _context.SaveChangesAsync();
                product.Category = tempCat;
                await UpdateProductAsync(product.Id, product);
            } catch (Exception ex) {
                return ResponseData<Course>.Error(ex.Message);
            }

            return ResponseData<Course>.Success(product);
        }
        
        //fin
        public async Task DeleteProductAsync(int id) {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) {
                throw new KeyNotFoundException("Course not found");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        //fin
        public async Task<ResponseData<Course>> GetProductByIdAsync(int id) {
            var course = await _context.Courses
                                       .Include(c => c.Category)
                                       .FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) {
                return ResponseData<Course>.Error("No such product");
            }

            return ResponseData<Course>.Success(course);
        }

        //fin
        public async Task<ResponseData<ListModel<Course>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1, int pageSize = 3)
        {
            if (pageSize > _maxPageSize)
                pageSize = _maxPageSize;

            var query = _context.Courses
                                .Include(c => c.Category) // Eagerly load the Category entity
                                .AsQueryable();

            var dataList = new ListModel<Course>();

            query = query.Where(d => categoryNormalizedName == null ||
                                     d.Category.NormalizedName.Equals(categoryNormalizedName));

            // количество элементов в списке
            var count = await query.CountAsync();
            if (count == 0)
            {
                return ResponseData<ListModel<Course>>.Success(dataList);
            }

            // количество страниц
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (pageNo > totalPages)
                return ResponseData<ListModel<Course>>.Error("No such page");

            try {
                dataList.Items = await query.OrderBy(d => d.Id)
                                            .Skip((pageNo - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }


            dataList.CurrentPage = pageNo;
            dataList.TotalPages = totalPages;

            return ResponseData<ListModel<Course>>.Success(dataList);
        }

        public async Task<ResponseData<string>> SaveImageAsync(int id, IFormFile formFile) {
            // Retrieve the existing course by its ID
            var existingCourse = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (existingCourse == null) {
                return ResponseData<string>.Error("Course not found");
            }

            // Get the path to the wwwroot/Images folder
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var imagesPath = Path.Combine(wwwRootPath, "Images");

            // Ensure the Images folder exists
            if (!Directory.Exists(imagesPath)) {
                Directory.CreateDirectory(imagesPath);
            }

            // Generate a unique file name for the new image
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
            var filePath = Path.Combine(imagesPath, fileName);

            // Save the new image to the Images folder
            using (var stream = new FileStream(filePath, FileMode.Create)) {
                await formFile.CopyToAsync(stream);
            }

            // Delete the previous image if it exists
            if (!string.IsNullOrEmpty(existingCourse.Image)) {
                var oldImagePath = Path.Combine(wwwRootPath, existingCourse.Image);
                if (File.Exists(oldImagePath)) {
                    File.Delete(oldImagePath);
                }
            }

            // Update the Image path in the Course model
            existingCourse.Image = Path.Combine("Images", fileName);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return ResponseData<string>.Success(Path.Combine("Images", fileName));
        }

        //fin
        public async Task UpdateProductAsync(int id, Course product) {
            // Retrieve the existing course by its ID
            //var existingProduct = await _context.Courses
            //                           .Include(c => c.Category)
            //                           .FirstOrDefaultAsync(c => c.Id == id);

            //if (existingProduct == null) {
            //    throw new KeyNotFoundException("Course not found");
            //}

            //// Update properties
            //existingProduct.Name = product.Name;
            //existingProduct.Description = product.Description;
            //existingProduct.Price = product.Price;
            //existingProduct.Image = product.Image;

            //if (product.Category != null) {
            //    existingProduct.Category = product.Category;
            //}

            try {

                // Save changes to the database
                _context.Courses.Update(product);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }

        }


    }
}
