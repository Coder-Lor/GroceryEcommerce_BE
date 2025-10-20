using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductImage.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductImage.Handlers;

public class UpdateProductImageCommandHandler(
    IProductImageRepository repository,
    IMapper mapper,
    ILogger<UpdateProductImageCommandHandler> logger
) : IRequestHandler<UpdateProductImageCommand, Result<UpdateProductImageResponse>>
{
    public async Task<Result<UpdateProductImageResponse>> Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating product image {ImageId}", request.ImageId);

        var existing = await repository.GetByIdAsync(request.ImageId, cancellationToken);
        if (!existing.IsSuccess || existing.Data is null)
        {
            logger.LogWarning("Product image not found: {ImageId}", request.ImageId);
            return Result<UpdateProductImageResponse>.Failure("Product image not found");
        }

        var updateReq = new UpdateProductImageRequest
        {
            ImageUrl = request.ImageUrl,
            AltText = request.AltText,
            DisplayOrder = request.DisplayOrder,
            IsPrimary = request.IsPrimary
        };

        mapper.Map(updateReq, existing.Data);

        var updateResult = await repository.UpdateAsync(existing.Data, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            logger.LogWarning("Failed to update product image {ImageId}: {Error}", request.ImageId, updateResult.ErrorMessage);
            return Result<UpdateProductImageResponse>.Failure(updateResult.ErrorMessage ?? "Failed to update product image");
        }

        var response = mapper.Map<UpdateProductImageResponse>(existing.Data);
        return Result<UpdateProductImageResponse>.Success(response);
    }
}


