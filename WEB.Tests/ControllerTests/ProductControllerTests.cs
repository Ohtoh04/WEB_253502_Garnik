using NSubstitute;
using WEB_253502_Garnik.Services.CourceService;
using WEB_253502_Garnik.Services.CategoryService;
using Microsoft.AspNetCore.Mvc;
using WEB_253502_Garnik.Controllers;
using WEB.Domain.Entities;
using WEB.Domain.Models;
using Microsoft.AspNetCore.Http;
using Moq;

namespace WEB.Tests.ControllerTests {

	public class ProductControllerTests {
		private readonly ICourseService _courseService = Substitute.For<ICourseService>();
		private readonly ICategoryService _categoryService = Substitute.For<ICategoryService>();
		private readonly Product _controller;

		public ProductControllerTests() {
			// Контекст контроллера
			var controllerContext = new ControllerContext();
			// Макет HttpContext
			var moqHttpContext = new Mock<HttpContext>();
			moqHttpContext.Setup(c => c.Request.Headers).Returns(new HeaderDictionary());
			controllerContext.HttpContext = moqHttpContext.Object;
			// Создаем контроллер с подставными сервисами
			_controller = new Product(_courseService, _categoryService) { ControllerContext = controllerContext };
		}

		[Fact]
		public async Task Catalog_ReturnsNotFound_WhenCourseResponseIsUnsuccessful() {
			// Arrange
			_courseService.GetCourseListAsync(Arg.Any<string>(), Arg.Any<int>())
				.Returns(Task.FromResult(new ResponseData<ListModel<Course>> { Successfull = false, ErrorMessage = "Error" }));

			// Act
			var result = await _controller.Catalog(null);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("Error", notFoundResult.Value);
		}

		[Fact]
		public async Task Catalog_ReturnsNotFound_WhenCategoryListIsEmpty() {
			// Arrange
			_courseService.GetCourseListAsync(Arg.Any<string>(), Arg.Any<int>())
				.Returns(Task.FromResult(new ResponseData<ListModel<Course>> { Successfull = true, Data = new ListModel<Course>() }));
			_categoryService.GetCategoryListAsync()
				.Returns(Task.FromResult(new ResponseData<List<Category>> { Successfull = false, ErrorMessage = "Error" }));

			// Act
			var result = await _controller.Catalog("");

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("Error", notFoundResult.Value);
		}

		[Fact]
		public async Task Catalog_ReturnsViewWithCorrectData_WhenSuccessful() {
			// Arrange
			var categories = new List<Category>
			{
				new Category { Name = "Category1", NormalizedName = "CATEGORY1" },
				new Category { Name = "Category2", NormalizedName = "CATEGORY2" }
			};

			_courseService.GetCourseListAsync(Arg.Any<string>(), Arg.Any<int>())
				.Returns(Task.FromResult(new ResponseData<ListModel<Course>> { Successfull = true, Data = new ListModel<Course>() }));
			_categoryService.GetCategoryListAsync()
				.Returns(Task.FromResult(new ResponseData<List<Category>> { Successfull = true, Data = categories }));

			// Act
			var result = await _controller.Catalog("CATEGORY1");

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal(categories, viewResult.ViewData["Categories"]);
			Assert.Equal(categories.First(c => c.NormalizedName == "CATEGORY1"), viewResult.ViewData["CurrentCategory"]);
			Assert.IsAssignableFrom<ListModel<Course>>(viewResult.Model);
		}

		[Fact]
		public async Task Catalog_SetsCurrentCategoryToAll_WhenCategoryIsNull() {
			// Arrange
			var categories = new List<Category>
			{
			new Category { Name = "Category1", NormalizedName = "CATEGORY1" },
			new Category { Name = "Category2", NormalizedName = "CATEGORY2" }
		};

			_courseService.GetCourseListAsync(Arg.Any<string>(), Arg.Any<int>())
				.Returns(Task.FromResult(new ResponseData<ListModel<Course>> { Successfull = true, Data = new ListModel<Course>() }));
			_categoryService.GetCategoryListAsync()
				.Returns(Task.FromResult(new ResponseData<List<Category>> { Successfull = true, Data = categories }));

			// Act
			var result = await _controller.Catalog(null);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal(categories, viewResult.ViewData["Categories"]);
			Assert.NotNull(viewResult.ViewData["CurrentCategory"]);
			Assert.IsType<Category>(viewResult.ViewData["CurrentCategory"]);
		}
	}
}
