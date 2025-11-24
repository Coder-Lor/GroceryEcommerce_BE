using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Queries;

public record GetRewardPointsPagingQuery(
    PagedRequest Request
) : IRequest<Result<PagedResult<RewardPointDto>>>;

