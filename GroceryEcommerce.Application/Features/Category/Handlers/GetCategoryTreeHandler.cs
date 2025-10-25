using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Category.Handlers;

public class GetCategoryTreeHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<GetCategoryTreeHandler> logger
) : IRequestHandler<GetCategoryTreeQuery, Result<List<CategoryDto>>>
{
    public async Task<Result<List<CategoryDto>>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting category tree");

            var result = await categoryRepository.GetCategoryTreeAsync(cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to get category tree");
                return Result<List<CategoryDto>>.Failure(result.ErrorMessage ?? "Failed to get category tree.");
            }

            var response = mapper.Map<List<CategoryDto>>(result.Data);
            
            logger.LogInformation("Category tree retrieved successfully: {Count}", result.Data?.Count ?? 0);
            return Result<List<CategoryDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting category tree");
            return Result<List<CategoryDto>>.Failure("An error occurred while retrieving category tree.");
        }
    }
}
