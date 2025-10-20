using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttributeValue.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttributeValue.Handlers;

public class UpdateProductAttributeValueCommandHandler(
    IProductAttributeValueRepository repository,
    IMapper mapper,
    ILogger<UpdateProductAttributeValueCommandHandler> logger
) : IRequestHandler<UpdateProductAttributeValueCommand, Result<UpdateProductAttributeValueResponse>>
{
    public async Task<Result<UpdateProductAttributeValueResponse>> Handle(UpdateProductAttributeValueCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating product attribute value {ValueId}", request.ValueId);

        var existing = await repository.GetByIdAsync(request.ValueId, cancellationToken);
        if (!existing.IsSuccess || existing.Data is null)
        {
            return Result<UpdateProductAttributeValueResponse>.Failure("Attribute value not found");
        }

        existing.Data.Value = request.Value;

        var updateResult = await repository.UpdateAsync(existing.Data, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            return Result<UpdateProductAttributeValueResponse>.Failure(updateResult.ErrorMessage ?? "Failed to update attribute value");
        }

        var response = mapper.Map<UpdateProductAttributeValueResponse>(existing.Data);
        return Result<UpdateProductAttributeValueResponse>.Success(response);
    }
}


