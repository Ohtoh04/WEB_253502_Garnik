using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEB_253502_Garnik.Controllers {
    public class CartController : Controller {
        private readonly ICourseService _productService;
        private readonly Cart _cart;
        public CartController(ICourseService productService, Cart cart) {
            _productService = productService;
            _cart = cart;
        }

        [Route("Cart")]
        public IActionResult Cart() {
            ViewData["ReturnUrl"] = Request.Path + Request.QueryString.ToUriComponent();
            return View("~/Views/Cart/CartView.cshtml", _cart);
        }
        [Authorize]
        [Route("[controller]/remove/{id:int}")]
        public async Task<ActionResult> Remove(int id, string returnUrl) {
            _cart.RemoveItems(id);
            return Redirect(returnUrl);
        }
        [Authorize]
        [Route("[controller]/add/{id:int}")]
        public async Task<ActionResult> Add(int id, string returnUrl) {
            var data = await _productService.GetCourseByIdAsync(id);
            if (data.Successfull) {
                _cart.AddToCart(data.Data);
            }
            return Redirect(returnUrl);
        }

    }
}
