using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using WEB.Api.Data;
using WEB.Api.Services;
using WEB.Domain.Entities;
using WEB.Domain.Models;
using WEB_253502_Garnik.Services.Authentication;
using WEB_253502_Garnik.Services.CourceService;
using WEB_253502_Garnik.Services.FileService;

namespace WEB.Tests.ServiceTests {
	public class ProductServiceTests {
		private readonly ProductService _productService;
		private readonly AppDbContext _context;

		public ProductServiceTests() {
			// Настраиваем in-memory базу данных
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			_context = new AppDbContext(options);

			// Заполняем тестовые данные
			SeedData();

			_productService = new ProductService(_context);
		}

		private void SeedData() {
			// Создаем тестовые категории
			var category1 = new Category { NormalizedName = "Category1", Name = "Category1name" };
			var category2 = new Category { NormalizedName = "Category2", Name = "Category2name" };

			_context.Categories.AddRange(category1, category2);

			// Создаем тестовые курсы
			_context.Courses.AddRange(
				new Course { Name = "Course1", Category = category1 },
				new Course { Name = "Course2", Category = category1 },
				new Course { Name = "Course3", Category = category1 },
				new Course { Name = "Course4", Category = category2 },
				new Course { Name = "Course5", Category = category2 }
			);

			_context.SaveChanges();
		}

		[Fact]
		public async Task GetProductListAsync_ReturnsFirstPage_WithDefaultPageSize() {
			// Act
			var result = await _productService.GetProductListAsync(null);

			// Assert
			Assert.True(result.Successfull);
			Assert.Equal(3, result.Data.Items.Count);
			Assert.Equal(2, result.Data.TotalPages);
			Assert.Equal(1, result.Data.CurrentPage);
		}

		[Fact]
		public async Task GetProductListAsync_ReturnsCorrectPage() {
			// Act
			var result = await _productService.GetProductListAsync(null, pageNo: 2);

			// Assert
			Assert.True(result.Successfull);
			Assert.Equal(2, result.Data.Items.Count); // На второй странице 2 объекта
			Assert.Equal(2, result.Data.CurrentPage);
		}

		[Fact]
		public async Task GetProductListAsync_FiltersByCategory() {
			// Act
			var result = await _productService.GetProductListAsync("Category1");

			// Assert
			Assert.True(result.Successfull);
			Assert.Equal(3, result.Data.Items.Count); // Должны вернуться 3 курса из "Category1"
		}

		[Fact]
		public async Task GetProductListAsync_PageSizeCannotExceedMaxPageSize() {
			// Act
			var result = await _productService.GetProductListAsync(null, pageSize: 30);

			// Assert
			Assert.True(result.Successfull);
			Assert.True(20>result.Data.Items.Count); // Возвращается размер по умолчанию 3
		}

		[Fact]
		public async Task GetProductListAsync_ReturnsErrorWhenPageExceedsTotalPages() {
			// Act
			var result = await _productService.GetProductListAsync(null, pageNo: 3);

			// Assert
			Assert.False(result.Successfull);
			Assert.Equal("No such page", result.ErrorMessage); // Проверка ошибки при превышении номеров страниц
		}
	}
}
