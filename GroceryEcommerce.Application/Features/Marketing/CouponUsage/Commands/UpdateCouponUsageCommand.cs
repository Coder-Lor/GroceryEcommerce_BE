using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.CouponUsage.Commands;

public record UpdateCouponUsageCommand(
    Guid UsageId,
    decimal DiscountAmount
) : IRequest<Result<CouponUsageDto>>;

