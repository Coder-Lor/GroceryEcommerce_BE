using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryEcommerce.Domain.Entities.Auth
{
    public class RefreshTokens
    {
        [Key]
        public Guid TokenId { get; set; }
        [Required]
        public required Guid UserId { get; set; }
        [Required]
        public required string RefreshToken { get; set; }
        [Required]
        public DateTime ExpiresAt { get; set; }
        public bool Revoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [StringLength(45)]
        public string? CreatedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public User? User { get; set; }
    }
}
