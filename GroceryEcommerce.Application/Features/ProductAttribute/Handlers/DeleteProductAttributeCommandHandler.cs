// ...existing code...
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttribute.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Handlers;

public class DeleteProductAttributeCommandHandler(
    IProductAttributeRepository repository,
    ILogger<DeleteProductAttributeCommandHandler> logger
) : IRequestHandler<DeleteProductAttributeCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductAttributeCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting product attribute {AttributeId}", request.AttributeId);

        var existing = await repository.GetByIdAsync(request.AttributeId, cancellationToken);
        if (!existing.IsSuccess)
        {
            return Result<bool>.Failure(existing.ErrorMessage ?? "Failed to retrieve attribute");
        }

        if (existing.Data == null)
        {
            return Result<bool>.Failure($"Attribute with id '{request.AttributeId}' not found.");
        }

        var inUse = await repository.IsAttributeInUseAsync(request.AttributeId, cancellationToken);
        if (!inUse.IsSuccess)
        {
            return Result<bool>.Failure(inUse.ErrorMessage ?? "Failed to check attribute usage");
        }

        if (inUse.Data)
        {
            return Result<bool>.Failure("Attribute is currently in use and cannot be deleted.");
        }

        var result = await repository.DeleteAsync(request.AttributeId, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete attribute");
        }

        return Result<bool>.Success(result.Data);
    }
}
// ...existing code...
