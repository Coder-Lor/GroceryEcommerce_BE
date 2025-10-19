using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class GetProductBySkuHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetProductBySkuHandler> logger
) : IRequestHandler<GetProductBySkuQuery, Result<GetProductBySkuResponse>>
{
    public async Task<Result<GetProductBySkuResponse>> Handle(GetProductBySkuQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetProductBySkuQuery for SKU: {Sku}", request.Sku);

        var productResult = await repository.GetBySkuAsync(request.Sku, cancellationToken);
        if (!productResult.IsSuccess || productResult.Data is null)
        {
            logger.LogWarning("Product not found with SKU: {Sku}", request.Sku);
            return Result<GetProductBySkuResponse>.Failure("Product not found with SKU: " + request.Sku);
        }

        var response = mapper.Map<GetProductBySkuResponse>(productResult.Data);
        return Result<GetProductBySkuResponse>.Success(response);
    }
}