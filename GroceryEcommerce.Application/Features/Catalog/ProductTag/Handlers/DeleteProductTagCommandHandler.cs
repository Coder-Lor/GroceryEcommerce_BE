using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductTag.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductTag.Handlers;

public class DeleteProductTagCommandHandler(
    IProductTagRepository repository,
    ILogger<DeleteProductTagCommandHandler> logger
) : IRequestHandler<DeleteProductTagCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductTagCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting product tag {TagId}", request.TagId);

        var exists = await repository.ExistsAsync(request.TagId, cancellationToken);
        if (!exists.IsSuccess || !exists.Data)
        {
            return Result<bool>.Failure("Product tag not found");
        }

        var del = await repository.DeleteAsync(request.TagId, cancellationToken);
        if (!del.IsSuccess || !del.Data)
        {
            return Result<bool>.Failure(del.ErrorMessage ?? "Failed to delete product tag");
        }

        return Result<bool>.Success(true);
    }
}
