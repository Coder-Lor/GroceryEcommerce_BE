using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Handlers;

public class GetShopBySlugHandler(
    IMapper mapper,
    IShopRepository repository,
    ILogger<GetShopBySlugHandler> logger
) : IRequestHandler<GetShopBySlugQuery, Result<GetShopBySlugResponse>>
{
    public async Task<Result<GetShopBySlugResponse>> Handle(GetShopBySlugQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetShopBySlugQuery for shop: {Slug}", request.Slug);

        var shopResult = await repository.GetBySlugAsync(request.Slug, cancellationToken);
        if (!shopResult.IsSuccess || shopResult.Data is null)
        {
            logger.LogWarning("Shop not found: {Slug}", request.Slug);
            return Result<GetShopBySlugResponse>.Failure("Shop not found");
        }

        var response = mapper.Map<GetShopBySlugResponse>(shopResult.Data);
        return Result<GetShopBySlugResponse>.Success(response);
    }
}


