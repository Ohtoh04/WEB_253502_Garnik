namespace WEB_253502_Garnik.Models {
    public class ProductPageViewModel {
        // List of courses
        public List<Course> Courses { get; set; } = new List<Course>();

        // Pagination properties
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Page1 { get; set; }
        public int Page2 { get; set; }
        public int Page3 { get; set; }
        public string ReturnUrl { get; set; }

        // Current category
        public string CurrentCategory { get; set; }
    }
}