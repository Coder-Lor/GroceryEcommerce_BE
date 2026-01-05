using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class UpdateProductCommandHandler(
    IProductRepository repository,
    IMapper mapper,
    ILogger<UpdateProductCommandHandler> logger
) : IRequestHandler<UpdateProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling UpdateProductCommand for product: {ProductId}", request.ProductId);

        // Get existing product
        var existingProductResult = await repository.GetByIdAsync(request.ProductId, cancellationToken);
        if (!existingProductResult.IsSuccess || existingProductResult.Data is null)
        {
            logger.LogWarning("Product not found: {ProductId}", request.ProductId);
            return Result<bool>.Failure("Product not found");
        }

        var existingProduct = existingProductResult.Data;

        // Update product properties
        existingProduct.Name = request.Name;
        existingProduct.Slug = request.Slug;
        existingProduct.Sku = request.Sku;
        existingProduct.Description = request.Description;
        existingProduct.ShortDescription = request.ShortDescription;
        existingProduct.Price = request.Price;
        existingProduct.DiscountPrice = request.DiscountPrice;
        existingProduct.Cost = request.Cost;
        existingProduct.StockQuantity = request.StockQuantity;
        existingProduct.MinStockLevel = request.MinStockLevel;
        existingProduct.Weight = request.Weight;
        existingProduct.Dimensions = request.Dimensions;
        existingProduct.CategoryId = request.CategoryId;
        existingProduct.BrandId = request.BrandId;
        existingProduct.ShopId = request.ShopId;
        existingProduct.Status = request.Status;
        existingProduct.IsFeatured = request.IsFeatured;
        existingProduct.IsDigital = request.IsDigital;
        existingProduct.MetaTitle = request.MetaTitle;
        existingProduct.MetaDescription = request.MetaDescription;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        // Update product
        var updateResult = await repository.UpdateAsync(existingProduct, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            logger.LogError("Failed to update product: {ProductId}", request.ProductId);
            return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to update product.");
        }

        logger.LogInformation("Product updated successfully: {ProductId}", request.ProductId);
        return Result<bool>.Success(updateResult.Data);
    }
}
