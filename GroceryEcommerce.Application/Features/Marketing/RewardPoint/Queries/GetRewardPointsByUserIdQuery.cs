using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Queries;

public record GetRewardPointsByUserIdQuery(
    Guid UserId
) : IRequest<Result<List<RewardPointDto>>>;

