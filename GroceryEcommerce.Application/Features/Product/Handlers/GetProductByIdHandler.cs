using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class GetProductByIdHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetProductByIdHandler> logger
) : IRequestHandler<GetProductByIdQuery, Result<GetProductByIdResponse>>
{
    public async Task<Result<GetProductByIdResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetProductByIdQuery for product: {ProductId}", request.ProductId);

        var productResult = await repository.GetByIdAsync(request.ProductId, cancellationToken);
        if (!productResult.IsSuccess || productResult.Data is null)
        {
            logger.LogWarning("Product not found: {ProductId}", request.ProductId);
            return Result<GetProductByIdResponse>.Failure("Product not found");
        }

        var response = mapper.Map<GetProductByIdResponse>(productResult.Data);
        return Result<GetProductByIdResponse>.Success(response);
    }
}
