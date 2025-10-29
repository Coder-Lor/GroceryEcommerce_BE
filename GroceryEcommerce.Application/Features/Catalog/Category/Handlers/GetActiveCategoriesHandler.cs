using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers;

public class GetActiveCategoriesHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<GetActiveCategoriesHandler> logger
) : IRequestHandler<GetActiveCategoriesQuery, Result<GetActiveCategoriesResponse>>
{
    public async Task<Result<GetActiveCategoriesResponse>> Handle(GetActiveCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting active categories");

            var result = await categoryRepository.GetActiveCategoriesAsync(cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to get active categories");
                return Result<GetActiveCategoriesResponse>.Failure(result.ErrorMessage ?? "Failed to get active categories.");
            }

            var response = mapper.Map<GetActiveCategoriesResponse>(result.Data);
            
            logger.LogInformation("Active categories retrieved successfully: {Count}", result.Data?.Count ?? 0);
            return Result<GetActiveCategoriesResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting active categories");
            return Result<GetActiveCategoriesResponse>.Failure("An error occurred while retrieving active categories.");
        }
    }
}
