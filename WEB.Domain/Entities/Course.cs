using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEB.Domain.Entities {
    public class Course {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;   
        public string Description { get; set; } = string.Empty;
        public Category? Category { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string MIME { get; set; } = string.Empty;

    }
}
