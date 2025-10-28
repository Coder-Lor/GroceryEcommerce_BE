// ...existing code...
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Handlers;

public class CheckAttributeInUseQueryHandler(
    IProductAttributeRepository repository,
    ILogger<CheckAttributeInUseQueryHandler> logger
) : IRequestHandler<CheckAttributeInUseQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckAttributeInUseQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking if attribute {AttributeId} is in use", request.AttributeId);

        var inUse = await repository.IsAttributeInUseAsync(request.AttributeId, cancellationToken);
        if (!inUse.IsSuccess)
        {
            return Result<bool>.Failure(inUse.ErrorMessage ?? "Failed to check attribute usage");
        }

        return Result<bool>.Success(inUse.Data);
    }
}
// ...existing code...
