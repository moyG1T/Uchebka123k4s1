using System;
using Uchebka123k4s1.Data.Remote.SqlModel;

namespace Uchebka123k4s1.Domain.Contexts
{
    public class OrderContext
    {
        public event Action<Order> OrderAdded;
        public void NotifyOrderAdded(Order order)
        {
            OrderAdded?.Invoke(order);
        }

        public Order SelectedOrder { get; set; }
    }
}
