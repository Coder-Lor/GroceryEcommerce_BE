using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.ShoppingCart.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Handlers;

public class GetShoppingCartByUserIdHandler(
    ICartRepository cartRepository,
    IMapper mapper,
    ICurrentUserService currentUserService,
    ILogger<GetShoppingCartByUserIdHandler> logger
) : IRequestHandler<GetShoppingCartByUserIdQuery, Result<ShoppingCartDto>>
{
    public async Task<Result<ShoppingCartDto>> Handle(GetShoppingCartByUserIdQuery request, CancellationToken cancellationToken)
    {
        var userId = request?.UserId ?? currentUserService.GetCurrentUserId() ?? Guid.Empty;
        
        logger.LogInformation("Getting shopping cart for user {UserId}", userId);

        var result = await cartRepository.GetShoppingCartByUserIdAsync(userId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<ShoppingCartDto>.Failure(result.ErrorMessage ?? "Shopping cart not found");
        }

        var dto = mapper.Map<ShoppingCartDto>(result.Data);
        return Result<ShoppingCartDto>.Success(dto);
    }
}


