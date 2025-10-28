using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class UpdateProductStatusCommandHandler(
    IProductRepository repository,
    IMapper mapper,
    ILogger<UpdateProductStatusCommandHandler> logger
) : IRequestHandler<UpdateProductStatusCommand, Result<UpdateProductStatusResponse>>
{
    public async Task<Result<UpdateProductStatusResponse>> Handle(UpdateProductStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling UpdateProductStatusCommand for product: {ProductId}, Status: {Status}", request.ProductId, request.Status);

        // Get existing product
        var existingProductResult = await repository.GetByIdAsync(request.ProductId, cancellationToken);
        if (!existingProductResult.IsSuccess || existingProductResult.Data is null)
        {
            logger.LogWarning("Product not found: {ProductId}", request.ProductId);
            return Result<UpdateProductStatusResponse>.Failure("Product not found");
        }

        var existingProduct = existingProductResult.Data;
        existingProduct.Status = request.Status;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        // Update product status
        var updateResult = await repository.UpdateAsync(existingProduct, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            logger.LogError("Failed to update product status: {ProductId}", request.ProductId);
            return Result<UpdateProductStatusResponse>.Failure(updateResult.ErrorMessage ?? "Failed to update product status.");
        }

        var response = mapper.Map<UpdateProductStatusResponse>(updateResult.Data);
        logger.LogInformation("Product status updated successfully: {ProductId}", request.ProductId);
        return Result<UpdateProductStatusResponse>.Success(response);
    }
}
