using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Supplier.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Supplier.Handlers;

public class GetSupplierByIdHandler(
    ISupplierRepository repository,
    IMapper mapper,
    ILogger<GetSupplierByIdHandler> logger
) : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting supplier by id: {SupplierId}", request.SupplierId);

        var result = await repository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            logger.LogWarning("Supplier not found: {SupplierId}", request.SupplierId);
            return Result<SupplierDto>.Failure("Supplier not found");
        }

        var dto = mapper.Map<SupplierDto>(result.Data);
        return Result<SupplierDto>.Success(dto);
    }
}


