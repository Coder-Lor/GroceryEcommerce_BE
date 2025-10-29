using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductTagAssignment.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductTagAssignment.Handlers;

public class GetProductTagAssignmentsByProductHandler(
    IProductTagAssignmentRepository repository,
    IMapper mapper,
    ILogger<GetProductTagAssignmentsByProductHandler> logger
) : IRequestHandler<GetProductTagAssignmentsByProductQuery, Result<PagedResult<ProductTagAssignmentDto>>>
{
    public async Task<Result<PagedResult<ProductTagAssignmentDto>>> Handle(GetProductTagAssignmentsByProductQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting tag assignments for product {ProductId}", request.ProductId);
        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        var result = await repository.GetByProductIdAsync(pagedRequest, request.ProductId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<ProductTagAssignmentDto>>.Failure(result.ErrorMessage ?? "Failed to get tag assignments");
        }

        var mapped = mapper.Map<PagedResult<ProductTagAssignmentDto>>(result.Data);
        return Result<PagedResult<ProductTagAssignmentDto>>.Success(mapped);
    }
}
