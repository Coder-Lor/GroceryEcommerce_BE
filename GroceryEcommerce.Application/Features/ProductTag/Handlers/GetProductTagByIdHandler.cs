using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductTag.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductTag.Handlers;

public class GetProductTagByIdHandler(
    IProductTagRepository repository,
    IMapper mapper,
    ILogger<GetProductTagByIdHandler> logger
) : IRequestHandler<GetProductTagByIdQuery, Result<ProductTagDto>>
{
    public async Task<Result<ProductTagDto>> Handle(GetProductTagByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product tag by id {TagId}", request.TagId);
        var result = await repository.GetByIdAsync(request.TagId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<ProductTagDto>.Failure("Product tag not found");
        }
        var dto = mapper.Map<ProductTagDto>(result.Data);
        return Result<ProductTagDto>.Success(dto);
    }
}
