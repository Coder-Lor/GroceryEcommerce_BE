using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Commands;

public record DeleteRewardPointCommand(
    Guid RewardPointId
) : IRequest<Result<bool>>;

