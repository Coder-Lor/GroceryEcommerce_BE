using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductTag.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductTag.Handlers;

public class UpdateProductTagCommandHandler(
    IProductTagRepository repository,
    IMapper mapper,
    ILogger<UpdateProductTagCommandHandler> logger
) : IRequestHandler<UpdateProductTagCommand, Result<UpdateProductTagResponse>>
{
    public async Task<Result<UpdateProductTagResponse>> Handle(UpdateProductTagCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating product tag {TagId}", request.TagId);

        var existing = await repository.GetByIdAsync(request.TagId, cancellationToken);
        if (!existing.IsSuccess || existing.Data is null)
        {
            return Result<UpdateProductTagResponse>.Failure("Product tag not found");
        }

        existing.Data.Name = request.Name;
        existing.Data.Slug = request.Slug;
        existing.Data.Description = request.Description;

        var updateResult = await repository.UpdateAsync(existing.Data, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            return Result<UpdateProductTagResponse>.Failure(updateResult.ErrorMessage ?? "Failed to update product tag");
        }

        var response = mapper.Map<UpdateProductTagResponse>(existing.Data);
        return Result<UpdateProductTagResponse>.Success(response);
    }
}
