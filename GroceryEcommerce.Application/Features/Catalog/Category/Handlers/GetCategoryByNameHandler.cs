using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers;

public class GetCategoryByNameHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<GetCategoryByNameHandler> logger
) : IRequestHandler<GetCategoryByNameQuery, Result<GetCategoryByNameResponse>>
{
    public async Task<Result<GetCategoryByNameResponse>> Handle(GetCategoryByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting category by name: {Name}", request.Name);

            var result = await categoryRepository.GetByNameAsync(request.Name, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                logger.LogWarning("Category not found: {Name}", request.Name);
                return Result<GetCategoryByNameResponse>.Failure("Category not found.");
            }

            var response = mapper.Map<GetCategoryByNameResponse>(result.Data);
            
            logger.LogInformation("Category retrieved successfully: {Name}", request.Name);
            return Result<GetCategoryByNameResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting category by name: {Name}", request.Name);
            return Result<GetCategoryByNameResponse>.Failure("An error occurred while retrieving the category.");
        }
    }
}
