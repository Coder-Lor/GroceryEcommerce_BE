using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Supplier.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Supplier.Handlers;

public class GetSuppliersPagingHandler(
    ISupplierRepository repository,
    IMapper mapper,
    ILogger<GetSuppliersPagingHandler> logger
) : IRequestHandler<GetSuppliersPagingQuery, Result<PagedResult<SupplierDto>>>
{
    public async Task<Result<PagedResult<SupplierDto>>> Handle(GetSuppliersPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting suppliers paging: Page {Page}, PageSize {PageSize}", 
            request.Request.Page, request.Request.PageSize);

        var result = await repository.GetPagedAsync(request.Request, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            logger.LogWarning("Failed to get suppliers");
            return Result<PagedResult<SupplierDto>>.Failure(result.ErrorMessage);
        }

        var dtoResult = mapper.Map<PagedResult<SupplierDto>>(result.Data);
        return Result<PagedResult<SupplierDto>>.Success(dtoResult);
    }
}


