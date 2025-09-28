using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryEcommerce.Domain.Entities.Auth
{
    public class AuditLog
    {
        public Guid AuditId { get; set; }
        public Guid? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Entity { get; set; }
        public string? Detail { get; set; }
        public Guid? EntityId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Domain methods
        public bool IsUserAction() => UserId.HasValue;
        public bool HasEntityReference() => EntityId.HasValue && !string.IsNullOrEmpty(Entity);
        public void SetActionDetails(string action, string? entity = null, Guid? entityId = null, string? detail = null) {
            Action = action;
            Entity = entity;
            EntityId = entityId;
            Detail = detail;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
