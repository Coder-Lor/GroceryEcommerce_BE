using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Supplier.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Supplier.Handlers;

public class SearchSuppliersByNameHandler(
    ISupplierRepository repository,
    IMapper mapper,
    ILogger<SearchSuppliersByNameHandler> logger
) : IRequestHandler<SearchSuppliersByNameQuery, Result<List<SupplierDto>>>
{
    public async Task<Result<List<SupplierDto>>> Handle(SearchSuppliersByNameQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Searching suppliers by name: {SearchTerm}", request.SearchTerm);

        var result = await repository.SearchByNameAsync(request.SearchTerm, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            logger.LogWarning("Failed to search suppliers");
            return Result<List<SupplierDto>>.Failure(result.ErrorMessage);
        }

        var dtos = mapper.Map<List<SupplierDto>>(result.Data);
        return Result<List<SupplierDto>>.Success(dtos);
    }
}


