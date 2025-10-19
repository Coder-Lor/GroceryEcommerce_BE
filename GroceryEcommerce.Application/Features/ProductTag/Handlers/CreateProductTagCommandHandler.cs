using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductTag.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductTag.Handlers;

public class CreateProductTagCommandHandler(
    IProductTagRepository repository,
    IMapper mapper,
    ILogger<CreateProductTagCommandHandler> logger
) : IRequestHandler<CreateProductTagCommand, Result<CreateProductTagResponse>>
{
    public async Task<Result<CreateProductTagResponse>> Handle(CreateProductTagCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product tag: {Name}", request.Name);

        var existingTag = await repository.GetByNameAsync(request.Name, cancellationToken);
        if (existingTag.IsSuccess && existingTag.Data != null)
        {
            return Result<CreateProductTagResponse>.Failure("Tag with this name already exists");
        }

        var createReq = new CreateProductTagRequest
        {
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description
        };

        var entity = mapper.Map<Domain.Entities.Catalog.ProductTag>(createReq);

        var result = await repository.CreateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<CreateProductTagResponse>.Failure(result.ErrorMessage ?? "Failed to create product tag");
        }

        var response = mapper.Map<CreateProductTagResponse>(result.Data);
        return Result<CreateProductTagResponse>.Success(response);
    }
}
