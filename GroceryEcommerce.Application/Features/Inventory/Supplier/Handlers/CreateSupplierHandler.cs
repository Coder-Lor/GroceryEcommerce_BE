using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Supplier.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Supplier.Handlers;

public class CreateSupplierHandler(
    ISupplierRepository repository,
    IMapper mapper,
    ILogger<CreateSupplierHandler> logger
) : IRequestHandler<CreateSupplierCommand, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating supplier: {Name}", request.Name);

        var existingResult = await repository.GetByNameAsync(request.Name, cancellationToken);
        if (existingResult.IsSuccess && existingResult.Data != null)
        {
            logger.LogWarning("Supplier with name {Name} already exists", request.Name);
            return Result<SupplierDto>.Failure("Supplier with this name already exists");
        }

        var supplier = new Domain.Entities.Inventory.Supplier
        {
            SupplierId = Guid.NewGuid(),
            Name = request.Name,
            ContactName = request.ContactName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            Address = request.Address,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await repository.CreateAsync(supplier, cancellationToken);
        if (!createResult.IsSuccess)
        {
            logger.LogError("Failed to create supplier: {Name}", request.Name);
            return Result<SupplierDto>.Failure(createResult.ErrorMessage);
        }

        var dto = mapper.Map<SupplierDto>(createResult.Data);
        logger.LogInformation("Supplier created successfully: {SupplierId}", dto.SupplierId);
        return Result<SupplierDto>.Success(dto);
    }
}


