using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductTag.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductTag.Handlers;

public class GetProductTagByNameHandler(
    IProductTagRepository repository,
    IMapper mapper,
    ILogger<GetProductTagByNameHandler> logger
) : IRequestHandler<GetProductTagByNameQuery, Result<ProductTagDto>>
{
    public async Task<Result<ProductTagDto>> Handle(GetProductTagByNameQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product tag by name {Name}", request.Name);
        var result = await repository.GetByNameAsync(request.Name, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<ProductTagDto>.Failure("Product tag not found");
        }
        var dto = mapper.Map<ProductTagDto>(result.Data);
        return Result<ProductTagDto>.Success(dto);
    }
}
