namespace GroceryEcommerce.Application.Models.Marketing;

public class CouponDto
{
    public Guid CouponId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public short DiscountType { get; set; }
    public string DiscountTypeName { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public int UserUsageLimit { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string? ValidationMessage { get; set; }
}

public class CreateCouponRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public short DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int UserUsageLimit { get; set; } = 1;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public short Status { get; set; } = 1;
}

public class UpdateCouponRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public short DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int UserUsageLimit { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public short Status { get; set; }
}

public class ValidateCouponRequest
{
    public string Code { get; set; } = string.Empty;
    public decimal OrderAmount { get; set; }
    public Guid? UserId { get; set; }
}

public class CouponUsageDto
{
    public Guid CouponUsageId { get; set; }
    public Guid CouponId { get; set; }
    public string CouponCode { get; set; } = string.Empty;
    public string CouponName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal OrderAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime UsedAt { get; set; }
}

public class CreateCouponUsageRequest
{
    public Guid CouponId { get; set; }
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public decimal OrderAmount { get; set; }
    public decimal DiscountAmount { get; set; }
}
