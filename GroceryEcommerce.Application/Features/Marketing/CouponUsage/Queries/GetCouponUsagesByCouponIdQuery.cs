using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.CouponUsage.Queries;

public record GetCouponUsagesByCouponIdQuery(
    Guid CouponId,
    PagedRequest Request
) : IRequest<Result<PagedResult<CouponUsageDto>>>;

