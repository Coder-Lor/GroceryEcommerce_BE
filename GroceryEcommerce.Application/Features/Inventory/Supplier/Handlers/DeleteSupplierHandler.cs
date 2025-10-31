using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Supplier.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Supplier.Handlers;

public class DeleteSupplierHandler(
    ISupplierRepository repository,
    ILogger<DeleteSupplierHandler> logger
) : IRequestHandler<DeleteSupplierCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting supplier: {SupplierId}", request.SupplierId);

        var existsResult = await repository.ExistsAsync(request.SupplierId, cancellationToken);
        if (!existsResult.IsSuccess || !existsResult.Data)
        {
            logger.LogWarning("Supplier not found: {SupplierId}", request.SupplierId);
            return Result<bool>.Failure("Supplier not found");
        }

        var inUseResult = await repository.IsSupplierInUseAsync(request.SupplierId, cancellationToken);
        if (inUseResult.IsSuccess && inUseResult.Data)
        {
            logger.LogWarning("Cannot delete supplier {SupplierId} - it is in use", request.SupplierId);
            return Result<bool>.Failure("Cannot delete supplier that is in use");
        }

        var deleteResult = await repository.DeleteAsync(request.SupplierId, cancellationToken);
        if (!deleteResult.IsSuccess)
        {
            logger.LogError("Failed to delete supplier: {SupplierId}", request.SupplierId);
            return Result<bool>.Failure(deleteResult.ErrorMessage);
        }

        logger.LogInformation("Supplier deleted successfully: {SupplierId}", request.SupplierId);
        return Result<bool>.Success(true);
    }
}


