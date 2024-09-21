using System.ComponentModel.DataAnnotations;

namespace WEB.Blazor.SSR {
    public class CounterHelper {
        [Range(1, 10)]
        public int CountVal {  get; set; }  
    }
}
