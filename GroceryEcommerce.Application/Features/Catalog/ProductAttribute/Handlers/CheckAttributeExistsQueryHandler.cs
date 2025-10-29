// ...existing code...
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Handlers;

public class CheckAttributeExistsQueryHandler(
    IProductAttributeRepository repository,
    ILogger<CheckAttributeExistsQueryHandler> logger
) : IRequestHandler<CheckAttributeExistsQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckAttributeExistsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking existence of attribute by name {Name}", request.Name);

        var exists = await repository.ExistsAsync(request.Name, cancellationToken);
        if (!exists.IsSuccess)
        {
            return Result<bool>.Failure(exists.ErrorMessage ?? "Failed to check attribute existence");
        }

        return Result<bool>.Success(exists.Data);
    }
}
