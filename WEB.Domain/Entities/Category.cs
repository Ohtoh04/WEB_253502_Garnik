using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEB.Domain.Entities {
    internal class Category {
        private int ID { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}
