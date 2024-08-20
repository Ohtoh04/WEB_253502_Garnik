using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEB.Domain.Entities {
    internal class Course {
        private int ID;
        public string Name { get; set; }   
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string PictureURL { get; set; }
        public string MIME { get; set; }

    }
}
