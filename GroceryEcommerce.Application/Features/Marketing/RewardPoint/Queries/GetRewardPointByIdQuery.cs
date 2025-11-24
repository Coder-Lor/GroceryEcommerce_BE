using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Queries;

public record GetRewardPointByIdQuery(
    Guid RewardPointId
) : IRequest<Result<RewardPointDto?>>;

