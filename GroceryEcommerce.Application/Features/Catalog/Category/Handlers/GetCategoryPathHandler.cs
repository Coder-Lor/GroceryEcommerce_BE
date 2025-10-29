using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers;

public class GetCategoryPathHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<GetCategoryPathHandler> logger
) : IRequestHandler<GetCategoryPathQuery, Result<GetCategoryPathResponse>>
{
    public async Task<Result<GetCategoryPathResponse>> Handle(GetCategoryPathQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting category path for: {CategoryId}", request.CategoryId);

            var result = await categoryRepository.GetCategoryPathAsync(request.CategoryId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to get category path for: {CategoryId}", request.CategoryId);
                return Result<GetCategoryPathResponse>.Failure(result.ErrorMessage ?? "Failed to get category path.");
            }

            var response = mapper.Map<GetCategoryPathResponse>(result.Data);
            logger.LogInformation("Category path retrieved: {CategoryId}", request.CategoryId);
            return Result<GetCategoryPathResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting category path for: {CategoryId}", request.CategoryId);
            return Result<GetCategoryPathResponse>.Failure("An error occurred while retrieving category path.");
        }
    }
}

