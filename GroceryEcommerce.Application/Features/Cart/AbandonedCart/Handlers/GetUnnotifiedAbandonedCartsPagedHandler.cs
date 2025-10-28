using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.AbandonedCart.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.AbandonedCart.Handlers;

public class GetUnnotifiedAbandonedCartsPagedHandler(
    IAbandonedCartRepository abandonedCartRepository,
    IMapper mapper,
    ILogger<GetUnnotifiedAbandonedCartsPagedHandler> logger
) : IRequestHandler<GetUnnotifiedAbandonedCartsPagedQuery, Result<PagedResult<AbandonedCartDto>>>
{
    public async Task<Result<PagedResult<AbandonedCartDto>>> Handle(GetUnnotifiedAbandonedCartsPagedQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting unnotified abandoned carts paged");

        var result = await abandonedCartRepository.GetUnnotifiedCartsAsync(request.Request, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<AbandonedCartDto>>.Failure(result.ErrorMessage ?? "Failed to get abandoned carts");
        }

        var mappedItems = mapper.Map<List<AbandonedCartDto>>(result.Data.Items);
        var response = new PagedResult<AbandonedCartDto>(
            mappedItems,
            result.Data.TotalCount,
            result.Data.Page,
            result.Data.PageSize
        );

        return Result<PagedResult<AbandonedCartDto>>.Success(response);
    }
}

