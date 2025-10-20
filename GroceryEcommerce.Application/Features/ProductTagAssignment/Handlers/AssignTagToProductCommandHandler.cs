using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductTagAssignment.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductTagAssignment.Handlers;

public class AssignTagToProductCommandHandler(
    IProductTagAssignmentRepository repository,
    ILogger<AssignTagToProductCommandHandler> logger
) : IRequestHandler<AssignTagToProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AssignTagToProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Assigning tag {TagId} to product {ProductId}", request.TagId, request.ProductId);

        var result = await repository.AssignTagToProductAsync(request.ProductId, request.TagId, cancellationToken);
        if (!result.IsSuccess || !result.Data)
        {
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to assign tag to product");
        }

        return Result<bool>.Success(true);
    }
}
