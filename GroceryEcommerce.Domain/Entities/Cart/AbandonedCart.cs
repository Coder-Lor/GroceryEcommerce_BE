using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryEcommerce.Domain.Entities.Cart
{
    public class AbandonedCart
    {
		public Guid AbandonedId { get; set; }
		public Guid CartId { get; set; }
		public Guid? UserId { get; set; }
        public DateTime AbandonedAt { get; set; }
		public bool Notified { get; set;  }
        public bool IsUserAbandoned() => UserId.HasValue;
        public bool ShouldSendNotification() => !Notified && IsUserAbandoned();
        public void MarkAsNotified() => Notified = true;

        public AbandonedCart() { }

        public AbandonedCart(Guid abandonedId, Guid cartId, Guid? userId, DateTime abandonedAt, bool notified) {
            AbandonedId = abandonedId;
            CartId = cartId;
            UserId = userId;
            AbandonedAt = abandonedAt;
            Notified = notified;
        }
    }
}
