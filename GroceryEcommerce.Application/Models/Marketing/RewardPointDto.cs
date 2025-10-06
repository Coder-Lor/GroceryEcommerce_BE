namespace GroceryEcommerce.Application.Models.Marketing;

public class RewardPointDto
{
    public Guid RewardPointId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public short TransactionType { get; set; }
    public string TransactionTypeName { get; set; } = string.Empty;
    public int Points { get; set; }
    public int BalanceBefore { get; set; }
    public int BalanceAfter { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsExpired { get; set; }
}

public class CreateRewardPointRequest
{
    public Guid UserId { get; set; }
    public short TransactionType { get; set; }
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class UpdateRewardPointRequest
{
    public short TransactionType { get; set; }
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}

public class UserRewardSummaryDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int AvailablePoints { get; set; }
    public int UsedPoints { get; set; }
    public int ExpiredPoints { get; set; }
    public int PendingPoints { get; set; }
    public DateTime? LastEarnedDate { get; set; }
    public DateTime? LastUsedDate { get; set; }
    public List<RewardPointDto> RecentTransactions { get; set; } = new();
}

public class AddRewardPointsRequest
{
    public Guid UserId { get; set; }
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class DeductRewardPointsRequest
{
    public Guid UserId { get; set; }
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
}

public class RewardPointSettingsDto
{
    public decimal PointsPerDollar { get; set; }
    public decimal DollarPerPoint { get; set; }
    public int MinPointsToRedeem { get; set; }
    public int MaxPointsPerTransaction { get; set; }
    public int PointsExpiryDays { get; set; }
    public bool AllowNegativeBalance { get; set; }
    public List<RewardTierDto> RewardTiers { get; set; } = new();
}

public class RewardTierDto
{
    public Guid RewardTierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinPoints { get; set; }
    public int MaxPoints { get; set; }
    public decimal Multiplier { get; set; }
    public string? Benefits { get; set; }
    public bool IsActive { get; set; }
}
