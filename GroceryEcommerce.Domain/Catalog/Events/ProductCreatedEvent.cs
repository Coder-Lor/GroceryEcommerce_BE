using GroceryEcommerce.Domain.Common;

namespace GroceryEcommerce.Domain.Catalog.Events
{
    public class ProductCreatedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string Name { get; }
        public string Sku { get; }
        public decimal Price { get; }
        public Guid CategoryId { get; }

        public ProductCreatedEvent(Guid productId, string name, string sku, decimal price, Guid categoryId)
        {
            ProductId = productId;
            Name = name;
            Sku = sku;
            Price = price;
            CategoryId = categoryId;
        }
    }
}