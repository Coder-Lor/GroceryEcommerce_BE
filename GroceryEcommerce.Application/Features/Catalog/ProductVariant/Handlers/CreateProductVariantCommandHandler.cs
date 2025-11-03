using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Handlers;

public class CreateProductVariantCommandHandler(
    IProductVariantRepository repository,
    IMapper mapper,
    ILogger<CreateProductVariantCommandHandler> logger
) : IRequestHandler<CreateProductVariantCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product variant for product {ProductId}", request.ProductId);

        var existingVariant = await repository.GetBySkuAsync(request.Sku, cancellationToken);
        if (existingVariant.IsSuccess && existingVariant.Data != null)
        {
            return Result<bool>.Failure("Variant with this SKU already exists");
        }

        var createReq = new CreateProductVariantRequest
        {
            ProductId = request.ProductId,
            Sku = request.Sku,
            Name = request.Name,
            Price = request.Price,
            DiscountPrice = request.DiscountPrice,
            StockQuantity = request.StockQuantity,
            MinStockLevel = request.MinStockLevel,
            Weight = request.Weight,
            Dimensions = request.Dimensions,
            ImageUrl = request.ImageUrl,
            Status = request.Status
        };

        var entity = mapper.Map<Domain.Entities.Catalog.ProductVariant>(createReq);

        var result = await repository.CreateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to create product variant");
        }

        return Result<bool>.Success(true);
    }
}
