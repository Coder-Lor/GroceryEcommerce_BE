using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers;

public class GetRootCategoriesHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<GetRootCategoriesHandler> logger
) : IRequestHandler<GetRootCategoriesQuery, Result<GetRootCategoriesResponse>>
{
    public async Task<Result<GetRootCategoriesResponse>> Handle(GetRootCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting root categories");

            var result = await categoryRepository.GetRootCategoriesAsync(cancellationToken);
            if (!result.IsSuccess)
            {   
                logger.LogError("Failed to get root categories");
                return Result<GetRootCategoriesResponse>.Failure(result.ErrorMessage ?? "Failed to get root categories.");
            }

            var response = mapper.Map<GetRootCategoriesResponse>(result.Data);
            
            logger.LogInformation("Root categories retrieved successfully: {Count}", result.Data?.Count ?? 0);
            return Result<GetRootCategoriesResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting root categories");
            return Result<GetRootCategoriesResponse>.Failure("An error occurred while retrieving root categories.");
        }
    }
}
