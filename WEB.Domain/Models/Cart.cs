using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEB.Domain.Entities;

namespace WEB.Domain.Models {
    public class Cart {
        /// <summary>
        /// Список объектов в корзине
        /// key - идентификатор объекта
        /// </summary>
        public Dictionary<int, CartItem> CartItems { get; set; } = new();
        /// <summary>
        /// Добавить объект в корзину
        /// </summary>
        /// <param name="dish">Добавляемый объект</param>
        public virtual void AddToCart(Course course) {
            // Check if the course already exists in the cart by course name
            var existingCartItem = CartItems.Values.FirstOrDefault(item => item.Item.Name == course.Name);

            if (existingCartItem != null) {
                // If the course is already in the cart, increment the count
                existingCartItem.Count++;
            }
            else {
                // If the course is not in the cart, create a new CartItem and add it
                CartItem newCartItem = new CartItem {
                    Item = course,
                    Count = 1
                };

                // Add the new item to the dictionary with a unique key (e.g., course ID or a new ID)
                CartItems[course.Id] = newCartItem;
            }
        }

        /// <summary>
        /// Удалить объект из корзины
        /// </summary>
        /// <param name="id"> id удаляемого объекта</param>
        public virtual void RemoveItems(int id) {
            // Check if the cart contains the item with the given id
            if (CartItems.ContainsKey(id)) {
                var cartItem = CartItems[id];

                // Decrease the count of the item
                cartItem.Count--;

                // If the count reaches zero, remove the item from the cart
                if (cartItem.Count <= 0) {
                    CartItems.Remove(id);
                }
            }
        }
        /// <summary>
        /// Очистить корзину
        /// </summary>
        public virtual void ClearAll() {
            CartItems = new();
        }
        /// <summary>
        /// Количество объектов в корзине
        /// </summary>
        public int Count { get => CartItems.Sum(item => item.Value.Count); }
        /// <summary>
        /// Общее количество калорий
        /// </summary>
        public decimal TotalPrice {
            get => CartItems.Sum(item => item.Value.Item.Price * item.Value.Count);
        }
    }

}
