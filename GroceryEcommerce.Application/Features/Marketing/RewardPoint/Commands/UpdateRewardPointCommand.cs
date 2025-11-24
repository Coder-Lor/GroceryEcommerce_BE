using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Commands;

public record UpdateRewardPointCommand(
    Guid RewardPointId,
    short TransactionType,
    int Points,
    string Reason,
    DateTime? ExpiresAt
) : IRequest<Result<RewardPointDto>>;

