using GroceryEcommerce.Domain.Common;

namespace GroceryEcommerce.Domain.Catalog.Events
{
    public class ProductUpdatedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string Name { get; }
        public decimal Price { get; }

        public ProductUpdatedEvent(Guid productId, string name, decimal price)
        {
            ProductId = productId;
            Name = name;
            Price = price;
        }
    }
}