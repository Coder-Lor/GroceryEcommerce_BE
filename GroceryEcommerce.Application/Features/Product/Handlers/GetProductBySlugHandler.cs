using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class GetProductBySlugHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetProductBySlugHandler> logger
) : IRequestHandler<GetProductBySlugQuery, Result<GetProductBySlugResponse>>
{
    public async Task<Result<GetProductBySlugResponse>> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetProductBySlugQuery for slug: {Slug}", request.Slug);

        var productResult = await repository.GetBySlugAsync(request.Slug, cancellationToken);
        if (!productResult.IsSuccess || productResult.Data is null)
        {
            logger.LogWarning("Product not found with slug: {Slug}", request.Slug);
            return Result<GetProductBySlugResponse>.Failure("Product not found with slug: " + request.Slug);
        }

        var response = mapper.Map<GetProductBySlugResponse>(productResult.Data);
        return Result<GetProductBySlugResponse>.Success(response);
    }
}
