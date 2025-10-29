using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Brand.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Brand.Handlers;

public class GetBrandByNameHandler(
    IMapper mapper,
    IBrandRepository repository,
    ILogger<GetBrandByNameHandler> logger
) : IRequestHandler<GetBrandByNameQuery, Result<GetBrandByNameResponse>>
{
    public async Task<Result<GetBrandByNameResponse>> Handle(GetBrandByNameQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetBrandByNameQuery for brand: {Name}", request.Name);

        var brandResult = await repository.GetByNameAsync(request.Name, cancellationToken);
        if (!brandResult.IsSuccess || brandResult.Data is null)
        {
            logger.LogWarning("Brand not found: {Name}", request.Name);
            return Result<GetBrandByNameResponse>.Failure("Brand not found");
        }

        var response = mapper.Map<GetBrandByNameResponse>(brandResult.Data);
        return Result<GetBrandByNameResponse>.Success(response);
    }
}