using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Category.Handlers;

public class GetCategoryBySlugHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<GetCategoryBySlugHandler> logger
) : IRequestHandler<GetCategoryBySlugQuery, Result<GetCategoryBySlugResponse>>
{
    public async Task<Result<GetCategoryBySlugResponse>> Handle(GetCategoryBySlugQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting category by slug: {Slug}", request.Slug);

            var result = await categoryRepository.GetBySlugAsync(request.Slug, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                logger.LogWarning("Category not found by slug: {Slug}", request.Slug);
                return Result<GetCategoryBySlugResponse>.Failure("Category not found.");
            }

            var response = mapper.Map<GetCategoryBySlugResponse>(result.Data);
            logger.LogInformation("Category retrieved by slug: {CategoryId}", result.Data.CategoryId);
            return Result<GetCategoryBySlugResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting category by slug: {Slug}", request.Slug);
            return Result<GetCategoryBySlugResponse>.Failure("An error occurred while retrieving the category.");
        }
    }
}

