using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.ShoppingCart.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Handlers;

public class GetShoppingCartByUserIdHandler(
    ICartRepository cartRepository,
    IMapper mapper,
    ILogger<GetShoppingCartByUserIdHandler> logger
) : IRequestHandler<GetShoppingCartByUserIdQuery, Result<ShoppingCartDto>>
{
    public async Task<Result<ShoppingCartDto>> Handle(GetShoppingCartByUserIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting shopping cart for user {UserId}", request.UserId);

        var result = await cartRepository.GetShoppingCartByUserIdAsync(request.UserId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<ShoppingCartDto>.Failure(result.ErrorMessage ?? "Shopping cart not found");
        }

        var dto = mapper.Map<ShoppingCartDto>(result.Data);
        return Result<ShoppingCartDto>.Success(dto);
    }
}


