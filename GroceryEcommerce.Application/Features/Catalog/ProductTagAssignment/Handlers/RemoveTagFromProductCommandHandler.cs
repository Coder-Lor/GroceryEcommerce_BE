using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductTagAssignment.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductTagAssignment.Handlers;

public class RemoveTagFromProductCommandHandler(
    IProductTagAssignmentRepository repository,
    ILogger<RemoveTagFromProductCommandHandler> logger
) : IRequestHandler<RemoveTagFromProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveTagFromProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing tag {TagId} from product {ProductId}", request.TagId, request.ProductId);

        var result = await repository.RemoveTagFromProductAsync(request.ProductId, request.TagId, cancellationToken);
        if (!result.IsSuccess || !result.Data)
        {
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to remove tag from product");
        }

        return Result<bool>.Success(true);
    }
}
