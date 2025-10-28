using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Brand.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Brand.Handlers;

public class GetActiveBrandsPagingHandler(
    IMapper mapper,
    IBrandRepository repository,
    ILogger<GetActiveBrandsPagingHandler> logger
) : IRequestHandler<GetActiveBrandsPagingQuery, Result<PagedResult<BrandDto>>>
{
    public async Task<Result<PagedResult<BrandDto>>> Handle(GetActiveBrandsPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetActiveBrandsPagingQuery with PageNumber: {PageNumber}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

        var brandResult = await repository.GetActiveBrandsAsync(request.Request, cancellationToken);

        if (!brandResult.IsSuccess || brandResult.Data is null)
        {
            logger.LogWarning("Brand not found for the given criteria.");
            return Result<PagedResult<BrandDto>>.Failure("Brand not found");
        }

        var response = mapper.Map<PagedResult<BrandDto>>(brandResult.Data);
        return Result<PagedResult<BrandDto>>.Success(response);
    }
}