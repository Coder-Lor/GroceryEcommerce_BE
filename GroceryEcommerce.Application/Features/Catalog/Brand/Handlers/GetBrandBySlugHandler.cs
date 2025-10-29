using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Brand.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Brand.Handlers;

public class GetBrandBySlugHandler(
    IMapper mapper,
    IBrandRepository repository,
    ILogger<GetBrandBySlugHandler> logger
) : IRequestHandler<GetBrandBySlugQuery, Result<GetBrandBySlugResponse>>
{
    public async Task<Result<GetBrandBySlugResponse>> Handle(GetBrandBySlugQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetBrandBySlugQuery for brand: {Slug}", request.Slug);

        var brandResult = await repository.GetBySlugAsync(request.Slug, cancellationToken);
        if (!brandResult.IsSuccess || brandResult.Data is null)
        {
            logger.LogWarning("Brand not found: {Slug}", request.Slug);
            return Result<GetBrandBySlugResponse>.Failure("Brand not found");
        }

        var response = mapper.Map<GetBrandBySlugResponse>(brandResult.Data);
        return Result<GetBrandBySlugResponse>.Success(response);
    }
}    