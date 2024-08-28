using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WEB.Domain.Entities;
using WEB_253502_Garnik.Services.CategoryService;

namespace WEB_253502_Garnik.Services.CourceService {
    public class MemoryCourseService : ICourseService {
        List<Course> _courses;
        List<Category> _categories;
        public int _itemsPerPage { get; }
        public int _pageNo { get; }

        public MemoryCourseService([FromServices] IConfiguration config,
                                    ICategoryService categoryService) {
            _categories = categoryService.GetCategoryListAsync().Result.Data;
            _pageNo = config.GetValue<int>("PageNo", 1);
            _itemsPerPage = config.GetValue<int>("ItemsPerPage");
            SetupData();
        }

        private void SetupData() {
            _courses = new List<Course> {
                new Course {ID = 1, Name="English language",
                            Description="learn inglish sooka",
                            Price = 200, Image="/Images/czipsy.jpg",
                            Category= _categories.Find(c=>c.NormalizedName.Equals("Languages"))},
                new Course { ID = 2, Name="German language", Description="deutsch sprache ja ja",
                            Price = 330, Image="/Images/niemecko.jpg",
                            Category= _categories.Find(c=>c.NormalizedName.Equals("Languages"))},
                new Course { ID = 3, Name="Tancy s gorinom", Description="bebebebebbebe",
                            Price = 69, Image="/Images/ded.jpg",
                            Category= _categories.Find(c=>c.NormalizedName.Equals("Autism"))},
                new Course { ID = 4, Name="gddfdjgj", Description="fjghsdshshs",
                            Price = 420, Image="/Images/happy.jpg",
                            Category= _categories.Find(c=>c.NormalizedName.Equals("Autism"))},
                new Course { ID = 5, Name="bvmcbmcvmvb", Description="asfhfdhfd",
                            Price = 1234, Image="/Images/IMG_20191029_000252.jpg",
                            Category= _categories.Find(c=>c.NormalizedName.Equals("Autism"))},
                new Course { ID = 6, Name="aboba", Description="asfafafaf",
                            Price = 228, Image="/Images/ladia.jpg",
                            Category= _categories.Find(c=>c.NormalizedName.Equals("Autism"))},
                new Course { ID = 7, Name="babababba", Description="brebrbwqe",
                            Price = 282, Image="/Images/screen-2.jpg",
                            Category= _categories.Find(c=>c.NormalizedName.Equals("Autism"))},
                new Course { ID = 8, Name="bebebebbe", Description="ghedfgdgfd",
                            Price = 25, Image="/Images/vozmak.jpg",
                            Category= _categories.Find(c=>c.NormalizedName.Equals("Autism"))},
            };

        }
        /// <summary>
        /// Получение списка всех объектов
        /// </summary>
        /// <param name="categoryNormalizedName">нормализованное имя категории для фильтрации</param>
        /// <param name="pageNo">номер страницы списка</param>
        /// <returns></returns>
        public Task<ResponseData<ListModel<Course>>> GetCourseListAsync(string? categoryNormalizedName, int pageNo = 1) {
            var responseData = new ResponseData<ListModel<Course>>();
            var listModel = new ListModel<Course>();

            // Фильтрация по категории
            IEnumerable<Course> filteredCourses;
            if (!categoryNormalizedName.IsNullOrEmpty()) {
                filteredCourses = _courses.Where(course => course.Category.NormalizedName == categoryNormalizedName);
            }
            else {
                filteredCourses = _courses;
            }
            

            // Подсчет общего количества страниц
            int totalItemCount = filteredCourses.Count();

            // Пропуск нужного количества элементов и выборка нужного количества элементов на текущей странице
            var pagedCourses = filteredCourses.Skip((pageNo - 1) * _itemsPerPage).Take(_itemsPerPage);

            // Добавление выбранных элементов в модель
            listModel.Items.AddRange(pagedCourses);
            listModel.TotalPages = (totalItemCount + _itemsPerPage - 1) / _itemsPerPage;
            listModel.CurrentPage = pageNo;

            // Установка данных в responseData
            responseData.Data = listModel;

            return Task.FromResult(responseData);
        }
        /// <summary>
        /// Поиск объекта по Id
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Найденный объект или null, если объект не найден</returns>
        public Task<ResponseData<Course>> GetCourseByIdAsync(int id) {
            var responseData = new ResponseData<Course>();
            responseData.Data = _courses.FirstOrDefault(course => course.ID == id);
            return Task.FromResult(responseData);
        }
        /// <summary>
        /// Обновление объекта
        /// </summary>
        /// <param name="id">Id изменяемомго объекта</param>
        /// <param name="Course">объект с новыми параметрами</param>
        /// <param name="formFile">Файл изображения</param>
        /// <returns></returns>
        public Task UpdateCourseAsync(int id, Course course, IFormFile? formFile) {
            var existingCourse = _courses.Find(c => c.ID == id);

            if (existingCourse != null) {
                existingCourse.Name = course.Name;
                existingCourse.Description = course.Description;

                if (formFile != null) {
                    // Handle the uploaded file
                    var fileName = Path.GetFileName(formFile.FileName);
                    var filePath = Path.Combine("uploads", fileName);

                    // Save the file to a location, e.g., a local directory
                    using (var stream = new FileStream(filePath, FileMode.Create)) {
                         formFile.CopyTo(stream);
                    }

                    // Assuming you want to store the file path in the course object
                    existingCourse.Image = filePath;
                }
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// Удаление объекта
        /// </summary>
        /// <param name="id">Id удаляемомго объекта</param>
        /// <returns></returns>
        public Task DeleteCourseAsync(int id) {
            _courses.Remove(_courses.Find(course => course.ID == id));
            return Task.CompletedTask;
        }
        /// <summary>
        /// Создание объекта
        /// </summary>
        /// <param name="Course">Новый объект</param>
        /// <param name="formFile">Файл изображения</param>
        /// <returns>Созданный объект</returns>
        public Task<ResponseData<Course>> CreateCourseAsync(Course course, IFormFile? formFile) {
            if (formFile != null) {
                var fileName = Path.GetFileName(formFile.FileName);
                var filePath = Path.Combine("uploads", fileName);

                // Save the file to a location, e.g., a local directory
                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    formFile.CopyTo(stream);
                }

                course.Image = filePath;
            }

            _courses.Add(course);

            var response = new ResponseData<Course>();
            response.Data = course;

            return Task.FromResult(response);
        }
    }
}
