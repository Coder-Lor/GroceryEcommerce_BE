using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Category.Handlers;

public class GetCategoryByIdHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<GetCategoryByIdHandler> logger
) : IRequestHandler<GetCategoryByIdQuery, Result<GetCategoryByIdResponse>>
{
    public async Task<Result<GetCategoryByIdResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting category by ID: {CategoryId}", request.CategoryId);

            var result = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                logger.LogWarning("Category not found: {CategoryId}", request.CategoryId);
                return Result<GetCategoryByIdResponse>.Failure("Category not found.");
            }

            var response = mapper.Map<GetCategoryByIdResponse>(result.Data);
            
            logger.LogInformation("Category retrieved successfully: {CategoryId}", request.CategoryId);
            return Result<GetCategoryByIdResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting category by ID: {CategoryId}", request.CategoryId);
            return Result<GetCategoryByIdResponse>.Failure("An error occurred while retrieving the category.");
        }
    }
}
