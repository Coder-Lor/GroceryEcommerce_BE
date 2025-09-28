using MediatR;

namespace GroceryEcommerce.Domain.Common
{
    public abstract class DomainEvent : INotification
    {
        public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    }
}