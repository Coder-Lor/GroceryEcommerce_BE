// ...existing code...
using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttribute.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Handlers;

public class CreateProductAttributeCommandHandler(
    IProductAttributeRepository repository,
    IMapper mapper,
    ILogger<CreateProductAttributeCommandHandler> logger
) : IRequestHandler<CreateProductAttributeCommand, Result<CreateProductAttributeResponse>>
{
    public async Task<Result<CreateProductAttributeResponse>> Handle(CreateProductAttributeCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product attribute {Name}", request.Name);

        // check if attribute with same name exists
        var exists = await repository.ExistsAsync(request.Name, cancellationToken);
        if (!exists.IsSuccess)
        {
            return Result<CreateProductAttributeResponse>.Failure(exists.ErrorMessage ?? "Failed to check attribute existence");
        }

        if (exists.Data)
        {
            return Result<CreateProductAttributeResponse>.Failure($"Attribute with name '{request.Name}' already exists.");
        }

        // Build the domain entity directly
        var entity = new Domain.Entities.Catalog.ProductAttribute
        {
            AttributeId = Guid.NewGuid(),
            Name = request.Name,
            DisplayName = request.DisplayName,
            AttributeType = (short)request.AttributeType,
            IsRequired = request.IsRequired,
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        var result = await repository.CreateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<CreateProductAttributeResponse>.Failure(result.ErrorMessage ?? "Failed to create attribute");
        }

        var response = mapper.Map<CreateProductAttributeResponse>(result.Data);
        return Result<CreateProductAttributeResponse>.Success(response);
    }
}
// ...existing code...
