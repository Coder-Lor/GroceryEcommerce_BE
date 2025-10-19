using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductImage.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductImage.Handlers;

public class SetPrimaryProductImageCommandHandler(
    IProductImageRepository repository,
    ILogger<SetPrimaryProductImageCommandHandler> logger
) : IRequestHandler<SetPrimaryProductImageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(SetPrimaryProductImageCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting primary image {ImageId}", request.ImageId);

        var exists = await repository.ExistsAsync(request.ImageId, cancellationToken);
        if (!exists.IsSuccess || !exists.Data)
        {
            return Result<bool>.Failure("Image not found");
        }

        var setResult = await repository.SetPrimaryImageAsync(request.ImageId, cancellationToken);
        if (!setResult.IsSuccess || !setResult.Data)
        {
            return Result<bool>.Failure(setResult.ErrorMessage ?? "Failed to set primary image");
        }

        return Result<bool>.Success(true);
    }
}


