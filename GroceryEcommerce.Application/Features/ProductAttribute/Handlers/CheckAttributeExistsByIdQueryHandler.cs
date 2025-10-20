// ...existing code...
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttribute.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Handlers;

public class CheckAttributeExistsByIdQueryHandler(
    IProductAttributeRepository repository,
    ILogger<CheckAttributeExistsByIdQueryHandler> logger
) : IRequestHandler<CheckAttributeExistsByIdQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckAttributeExistsByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking existence of attribute by id {AttributeId}", request.AttributeId);

        var exists = await repository.ExistsAsync(request.AttributeId, cancellationToken);
        if (!exists.IsSuccess)
        {
            return Result<bool>.Failure(exists.ErrorMessage ?? "Failed to check attribute existence");
        }

        return Result<bool>.Success(exists.Data);
    }
}
// ...existing code...
