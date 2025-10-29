using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.ShoppingCart.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Handlers;

public class GetCartSummaryHandler(
    ICartRepository cartRepository,
    ILogger<GetCartSummaryHandler> logger
) : IRequestHandler<GetCartSummaryQuery, Result<CartSummaryDto>>
{
    public async Task<Result<CartSummaryDto>> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting cart summary for user {UserId}", request.UserId);

        var cartResult = await cartRepository.GetShoppingCartByUserIdAsync(request.UserId, cancellationToken);
        if (!cartResult.IsSuccess || cartResult.Data is null)
        {
            var emptySummary = new CartSummaryDto
            {
                ItemCount = 0,
                SubTotal = 0,
                TotalAmount = 0,
                IsValid = true
            };
            return Result<CartSummaryDto>.Success(emptySummary);
        }

        var totalResult = await cartRepository.CalculateCartTotalAsync(cartResult.Data.CartId, cancellationToken);
        var itemCountResult = await cartRepository.GetCartItemCountAsync(request.UserId, cancellationToken);

        var summary = new CartSummaryDto
        {
            ItemCount = itemCountResult.IsSuccess ? itemCountResult.Data : 0,
            SubTotal = totalResult.IsSuccess ? totalResult.Data : 0m,
            TaxAmount = 0,
            ShippingAmount = 0,
            DiscountAmount = 0,
            TotalAmount = totalResult.IsSuccess ? totalResult.Data : 0m,
            IsValid = true,
            ValidationErrors = new List<string>()
        };

        return Result<CartSummaryDto>.Success(summary);
    }
}

