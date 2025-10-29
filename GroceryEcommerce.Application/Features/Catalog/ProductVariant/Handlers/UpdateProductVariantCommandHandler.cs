using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Handlers;

public class UpdateProductVariantCommandHandler(
    IProductVariantRepository repository,
    IMapper mapper,
    ILogger<UpdateProductVariantCommandHandler> logger
) : IRequestHandler<UpdateProductVariantCommand, Result<UpdateProductVariantResponse>>
{
    public async Task<Result<UpdateProductVariantResponse>> Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating product variant {VariantId}", request.VariantId);

        var existing = await repository.GetByIdAsync(request.VariantId, cancellationToken);
        if (!existing.IsSuccess || existing.Data is null)
        {
            return Result<UpdateProductVariantResponse>.Failure("Product variant not found");
        }

        existing.Data.Sku = request.Sku;
        existing.Data.Name = request.Name;
        existing.Data.Price = request.Price;
        existing.Data.DiscountPrice = request.DiscountPrice;
        existing.Data.StockQuantity = request.StockQuantity;
        existing.Data.Weight = request.Weight;
        existing.Data.ImageUrl = request.ImageUrl;
        existing.Data.Status = request.Status;
        existing.Data.UpdatedAt = DateTime.UtcNow;

        var updateResult = await repository.UpdateAsync(existing.Data, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            return Result<UpdateProductVariantResponse>.Failure(updateResult.ErrorMessage ?? "Failed to update product variant");
        }

        var response = mapper.Map<UpdateProductVariantResponse>(existing.Data);
        return Result<UpdateProductVariantResponse>.Success(response);
    }
}
