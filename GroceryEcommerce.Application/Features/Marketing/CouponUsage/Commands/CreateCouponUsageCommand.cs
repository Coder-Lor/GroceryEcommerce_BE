using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.CouponUsage.Commands;

public record CreateCouponUsageCommand(
    Guid CouponId,
    Guid UserId,
    Guid OrderId,
    decimal OrderAmount,
    decimal DiscountAmount
) : IRequest<Result<CouponUsageDto>>;

