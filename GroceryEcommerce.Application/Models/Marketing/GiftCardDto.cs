namespace GroceryEcommerce.Application.Models.Marketing;

public class GiftCardDto
{
    public Guid GiftCardId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal InitialAmount { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal UsedAmount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public Guid? PurchasedBy { get; set; }
    public string? PurchasedByName { get; set; }
    public Guid? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public string? AssignedToEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public bool IsValid { get; set; }
    public string? ValidationMessage { get; set; }
    public List<GiftCardTransactionDto> Transactions { get; set; } = new();
}

public class CreateGiftCardRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal InitialAmount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public Guid? PurchasedBy { get; set; }
    public Guid? AssignedTo { get; set; }
    public string? AssignedToEmail { get; set; }
}

public class UpdateGiftCardRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal InitialAmount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public short Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public string? AssignedToEmail { get; set; }
}

public class GiftCardTransactionDto
{
    public Guid GiftCardTransactionId { get; set; }
    public Guid GiftCardId { get; set; }
    public string GiftCardCode { get; set; } = string.Empty;
    public short TransactionType { get; set; }
    public string TransactionTypeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
}

public class RedeemGiftCardRequest
{
    public string Code { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public string? Description { get; set; }
}

public class ValidateGiftCardRequest
{
    public string Code { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
}
