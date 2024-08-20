using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_253502_Garnik.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace WEB_253502_Garnik.Components {

    public class UserCartComponent : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync() {
            // Здесь вы можете получить информацию о корзине из сессии, базы данных или другого источника
            var cart = GetCartInfo();

            // Возвращаем результат, связанный с определённым представлением и моделью данных
            return new HtmlContentViewComponentResult(
                new HtmlString($"{Math.Round(cart.TotalPrice, 2)} руб <i class=\"fa-solid fa-cart-shopping\"></i> ({cart.ItemCount})")
            );
        }

        private CartModel GetCartInfo() {
            // Пример данных. Здесь необходимо получить реальные данные корзины.
            return new CartModel {
                TotalPrice = 100.0m,
                ItemCount = 3
            };
        }
    }
    public class CartModel {
        public decimal TotalPrice { get; set; }
        public int ItemCount { get; set; }
    }
}
