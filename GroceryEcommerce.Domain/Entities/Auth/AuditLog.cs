using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryEcommerce.Domain.Entities.Auth
{
    public class AuditLog
    {
        [Key]
        public Guid AuditId { get; set; }
        
        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        
        [Required]
        [StringLength(200)]
        public required string Action { get; set; }
        
        [StringLength(100)]
        public string? Entity { get; set; }
        public Guid? EntityId { get; set; }
        public string? Detail { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? User { get; set; }

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
