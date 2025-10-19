using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttributeValue.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttributeValue.Handlers;

public class DeleteProductAttributeValueCommandHandler(
    IProductAttributeValueRepository repository,
    ILogger<DeleteProductAttributeValueCommandHandler> logger
) : IRequestHandler<DeleteProductAttributeValueCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductAttributeValueCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting product attribute value {ValueId}", request.ValueId);

        var exists = await repository.ExistsAsync(request.ValueId, cancellationToken);
        if (!exists.IsSuccess || !exists.Data)
        {
            return Result<bool>.Failure("Attribute value not found");
        }

        var del = await repository.DeleteAsync(request.ValueId, cancellationToken);
        if (!del.IsSuccess || !del.Data)
        {
            return Result<bool>.Failure(del.ErrorMessage ?? "Failed to delete attribute value");
        }

        return Result<bool>.Success(true);
    }
}


