using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_253502_Garnik.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Http;

namespace WEB_253502_Garnik.Components {

    public class UserCartComponent : ViewComponent {
        private readonly Cart _cart;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserCartComponent(Cart cart, IHttpContextAccessor httpContextAccessor) {
            _cart = cart;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IViewComponentResult> InvokeAsync() {

            // Возвращаем результат, связанный с определённым представлением и моделью данных
            return new HtmlContentViewComponentResult(
                new HtmlString($"{Math.Round(_cart.TotalPrice, 2)} руб <i class=\"fa-solid fa-cart-shopping\"></i> ({_cart.Count})")
            );
        }
    }
}
