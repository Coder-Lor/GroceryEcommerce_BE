using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class GetProductsPagingHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetProductsPagingHandler> logger
) : IRequestHandler<GetProductsPagingQuery, Result<PagedResult<ProductBaseResponse>>>
{
    public async Task<Result<PagedResult<ProductBaseResponse>>> Handle(GetProductsPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetProductsPagingQuery - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);


        var result = await repository.SearchProductsAsync(request.Request, request.Request.Search ?? string.Empty, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<ProductBaseResponse>>.Failure(result.ErrorMessage ?? "Failed to get paged products.");
        }

        // Map từng item trong PagedResult thay vì map toàn bộ PagedResult
        var mappedItems = mapper.Map<List<ProductBaseResponse>>(result.Data.Items);
        var response = new PagedResult<ProductBaseResponse>(
            mappedItems,
            result.Data.TotalCount,
            result.Data.Page,
            result.Data.PageSize
        );
        
        return Result<PagedResult<ProductBaseResponse>>.Success(response);
    }
}
