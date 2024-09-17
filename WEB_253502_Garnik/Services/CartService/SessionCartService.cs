using System.Text.Json.Serialization;
using WEB.Domain.Entities;
using WEB_253502_Garnik.Controllers;
using WEB_253502_Garnik.Extensions;

namespace WEB_253502_Garnik.Services.CartService {
    public class SessionCart : Cart {
        public static Cart GetCart(IServiceProvider services) {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>().HttpContext?.Session;
            SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();
            cart.Session = session;
            return cart;
        }

        [JsonIgnore]
        public ISession? Session { get; set; }
        public override void AddToCart(Course course) {
            base.AddToCart(course);
            Session?.SetJson("Cart", this);
        }
        public override void RemoveItems(int id) {
            base.RemoveItems(id);
            Session?.SetJson("Cart", this);
        }
        public override void ClearAll() {
            base.ClearAll();
            Session?.Remove("Cart");
        }
    }
}
