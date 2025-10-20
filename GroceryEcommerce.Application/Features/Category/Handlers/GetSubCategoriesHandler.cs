using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Category.Handlers;

public class GetSubCategoriesHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<GetSubCategoriesHandler> logger
) : IRequestHandler<GetSubCategoriesQuery, Result<GetSubCategoriesResponse>>
{
    public async Task<Result<GetSubCategoriesResponse>> Handle(GetSubCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting subcategories for parent: {ParentCategoryId}", request.ParentCategoryId);

            var result = await categoryRepository.GetSubCategoriesAsync(request.ParentCategoryId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to get subcategories for parent: {ParentCategoryId}", request.ParentCategoryId);
                return Result<GetSubCategoriesResponse>.Failure(result.ErrorMessage ?? "Failed to get subcategories.");
            }

            var response = mapper.Map<GetSubCategoriesResponse>(result.Data);
            logger.LogInformation("Subcategories retrieved for parent: {ParentCategoryId} Count={Count}", request.ParentCategoryId, result.Data?.Count ?? 0);
            return Result<GetSubCategoriesResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting subcategories for parent: {ParentCategoryId}", request.ParentCategoryId);
            return Result<GetSubCategoriesResponse>.Failure("An error occurred while retrieving subcategories.");
        }
    }
}

