using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.CouponUsage.Queries;

public record GetCouponUsageByIdQuery(
    Guid UsageId
) : IRequest<Result<CouponUsageDto?>>;

