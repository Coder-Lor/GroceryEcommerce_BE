using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Supplier.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Supplier.Handlers;

public class UpdateSupplierHandler(
    ISupplierRepository repository,
    IMapper mapper,
    ILogger<UpdateSupplierHandler> logger
) : IRequestHandler<UpdateSupplierCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating supplier: {SupplierId}", request.SupplierId);

        var existingResult = await repository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (!existingResult.IsSuccess || existingResult.Data == null)
        {
            logger.LogWarning("Supplier not found: {SupplierId}", request.SupplierId);
            return Result<bool>.Failure("Supplier not found");
        }

        var supplier = existingResult.Data;
        supplier.Name = request.Name;
        supplier.ContactName = request.ContactName;
        supplier.ContactEmail = request.ContactEmail;
        supplier.ContactPhone = request.ContactPhone;
        supplier.Address = request.Address;
        supplier.Note = request.Note;

        var updateResult = await repository.UpdateAsync(supplier, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            logger.LogError("Failed to update supplier: {SupplierId}", request.SupplierId);
            return Result<bool>.Failure(updateResult.ErrorMessage);
        }

        logger.LogInformation("Supplier updated successfully: {SupplierId}", request.SupplierId);
        return Result<bool>.Success(true);
    }
}


