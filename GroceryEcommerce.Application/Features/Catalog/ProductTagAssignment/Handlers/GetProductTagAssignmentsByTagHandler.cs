using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductTagAssignment.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductTagAssignment.Handlers;

public class GetProductTagAssignmentsByTagHandler(
    IProductTagAssignmentRepository repository,
    IMapper mapper,
    ILogger<GetProductTagAssignmentsByTagHandler> logger
) : IRequestHandler<GetProductTagAssignmentsByTagQuery, Result<PagedResult<ProductTagAssignmentDto>>>
{
    public async Task<Result<PagedResult<ProductTagAssignmentDto>>> Handle(GetProductTagAssignmentsByTagQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting tag assignments for tag {TagId}", request.TagId);
        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        var result = await repository.GetByTagIdAsync(pagedRequest, request.TagId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<ProductTagAssignmentDto>>.Failure(result.ErrorMessage ?? "Failed to get tag assignments");
        }

        var mapped = mapper.Map<PagedResult<ProductTagAssignmentDto>>(result.Data);
        return Result<PagedResult<ProductTagAssignmentDto>>.Success(mapped);
    }
}
