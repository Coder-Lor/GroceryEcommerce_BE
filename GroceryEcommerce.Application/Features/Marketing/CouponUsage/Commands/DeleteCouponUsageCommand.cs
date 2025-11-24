using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Marketing.CouponUsage.Commands;

public record DeleteCouponUsageCommand(
    Guid UsageId
) : IRequest<Result<bool>>;

