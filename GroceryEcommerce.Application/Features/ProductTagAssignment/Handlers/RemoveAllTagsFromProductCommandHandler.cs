using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductTagAssignment.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductTagAssignment.Handlers;

public class RemoveAllTagsFromProductCommandHandler(
    IProductTagAssignmentRepository repository,
    ILogger<RemoveAllTagsFromProductCommandHandler> logger
) : IRequestHandler<RemoveAllTagsFromProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveAllTagsFromProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing all tags from product {ProductId}", request.ProductId);

        var result = await repository.RemoveAllTagsFromProductAsync(request.ProductId, cancellationToken);
        if (!result.IsSuccess || !result.Data)
        {
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to remove all tags from product");
        }

        return Result<bool>.Success(true);
    }
}
