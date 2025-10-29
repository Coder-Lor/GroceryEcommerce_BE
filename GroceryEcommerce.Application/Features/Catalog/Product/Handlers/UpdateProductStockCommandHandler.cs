using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class UpdateProductStockCommandHandler(
    IProductRepository repository,
    IMapper mapper,
    ILogger<UpdateProductStockCommandHandler> logger
) : IRequestHandler<UpdateProductStockCommand, Result<UpdateProductStockResponse>>
{
    public async Task<Result<UpdateProductStockResponse>> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling UpdateProductStockCommand for product: {ProductId}, Stock: {StockQuantity}", request.ProductId, request.StockQuantity);

        // Get existing product
        var existingProductResult = await repository.GetByIdAsync(request.ProductId, cancellationToken);
        if (!existingProductResult.IsSuccess || existingProductResult.Data is null)
        {
            logger.LogWarning("Product not found: {ProductId}", request.ProductId);
            return Result<UpdateProductStockResponse>.Failure("Product not found");
        }

        var existingProduct = existingProductResult.Data;
        existingProduct.StockQuantity = request.StockQuantity;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        // Update product stock
        var updateResult = await repository.UpdateAsync(existingProduct, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            logger.LogError("Failed to update product stock: {ProductId}", request.ProductId);
            return Result<UpdateProductStockResponse>.Failure(updateResult.ErrorMessage ?? "Failed to update product stock.");
        }

        var response = mapper.Map<UpdateProductStockResponse>(updateResult.Data);
        logger.LogInformation("Product stock updated successfully: {ProductId}", request.ProductId);
        return Result<UpdateProductStockResponse>.Success(response);
    }
}
