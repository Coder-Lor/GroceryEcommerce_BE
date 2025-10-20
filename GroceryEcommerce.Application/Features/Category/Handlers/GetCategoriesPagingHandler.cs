using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryEcommerce.Application.Features.Category.Handlers;

public class GetCategoriesPagingHandler : IRequestHandler<GetCategoriesPagingQuery, Result<PagedResult<CategoryDto>>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCategoriesPagingHandler> _logger;

    public GetCategoriesPagingHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<GetCategoriesPagingHandler> logger)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<PagedResult<CategoryDto>>> Handle(GetCategoriesPagingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var req = request.Request ?? new PagedRequest();

            _logger.LogInformation("Getting categories paged: Page={Page}, PageSize={PageSize}", req.Page, req.PageSize);

            var pagedRequest = new PagedRequest
            {
                Page = req.Page,
                PageSize = req.PageSize,
                Search = req.Search,
                SortBy = req.SortBy,
                SortDirection = req.SortDirection
            };

            if (req.Filters != null)
            {
                foreach (var f in req.Filters)
                {
                    pagedRequest.AddFilter(f.FieldName, f.Value, f.Operator);
                }
            }


            var result = await _categoryRepository.GetPagedAsync(pagedRequest, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to get paged categories");
                return Result<PagedResult<CategoryDto>>.Failure(result.ErrorMessage ?? "Failed to get categories.");
            }

            var mapped = _mapper.Map<PagedResult<CategoryDto>>(result.Data);
            return Result<PagedResult<CategoryDto>>.Success(mapped);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged categories");
            return Result<PagedResult<CategoryDto>>.Failure("An error occurred while retrieving categories.");
        }
    }
}
