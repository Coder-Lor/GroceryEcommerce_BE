using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Commands;

public record CreateRewardPointCommand(
    Guid UserId,
    short TransactionType,
    int Points,
    string Reason,
    string? ReferenceId,
    string? ReferenceType,
    DateTime? ExpiresAt
) : IRequest<Result<RewardPointDto>>;

