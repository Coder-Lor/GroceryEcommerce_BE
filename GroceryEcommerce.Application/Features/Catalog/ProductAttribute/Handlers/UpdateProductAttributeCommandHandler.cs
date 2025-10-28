using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Handlers;

public class UpdateProductAttributeCommandHandler(
    IProductAttributeRepository repository,
    IMapper mapper,
    ILogger<UpdateProductAttributeCommandHandler> logger
) : IRequestHandler<UpdateProductAttributeCommand, Result<UpdateProductAttributeResponse>>
{
    public async Task<Result<UpdateProductAttributeResponse>> Handle(UpdateProductAttributeCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating product attribute {AttributeId}", request.AttributeId);

        // check attribute exists by id
        var existing = await repository.GetByIdAsync(request.AttributeId, cancellationToken);
        if (!existing.IsSuccess)
        {
            return Result<UpdateProductAttributeResponse>.Failure(existing.ErrorMessage ?? "Failed to retrieve attribute");
        }

        if (existing.Data == null)
        {
            return Result<UpdateProductAttributeResponse>.Failure($"Attribute with id '{request.AttributeId}' not found.");
        }

        // if name changed, ensure new name not used by another attribute
        if (!string.Equals(existing.Data.Name, request.Name, StringComparison.OrdinalIgnoreCase))
        {
            var nameExists = await repository.ExistsAsync(request.Name, cancellationToken);
            if (!nameExists.IsSuccess)
            {
                return Result<UpdateProductAttributeResponse>.Failure(nameExists.ErrorMessage ?? "Failed to check attribute name");
            }

            if (nameExists.Data)
            {
                return Result<UpdateProductAttributeResponse>.Failure($"Attribute with name '{request.Name}' already exists.");
            }
        }

        // Build the domain entity directly for update
        var entity = new Domain.Entities.Catalog.ProductAttribute
        {
            AttributeId = request.AttributeId,
            Name = request.Name,
            DisplayName = request.DisplayName,
            AttributeType = (short)request.AttributeType,
            IsRequired = request.IsRequired,
            DisplayOrder = request.DisplayOrder,
            CreatedAt = existing.Data.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await repository.UpdateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<UpdateProductAttributeResponse>.Failure(result.ErrorMessage ?? "Failed to update attribute");
        }

        var response = mapper.Map<UpdateProductAttributeResponse>(entity);
        return Result<UpdateProductAttributeResponse>.Success(response);
    }
}
