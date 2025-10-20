using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Brand.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Brand.Handlers;

public class UpdateBrandCommandHandler(
    IMapper mapper,
    IBrandRepository brandRepository,
    ILogger<UpdateBrandCommandHandler> logger
) : IRequestHandler<UpdateBrandCommand, Result<UpdateBrandResponse>>
{
    public async Task<Result<UpdateBrandResponse>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating brand: {BrandId}", request.BrandId);
        
        var brandResult = await brandRepository.GetByIdAsync(request.BrandId, cancellationToken);
        if (!brandResult.IsSuccess || brandResult.Data is null)
        {
            logger.LogWarning("Brand not found: {BrandId}", request.BrandId);
            return Result<UpdateBrandResponse>.Failure("Brand not found");
        }
        
        var brand = brandResult.Data;

        if (!string.IsNullOrEmpty(request.Name))
        {
            var existingBrand = await brandRepository.GetByNameAsync(request.Name, cancellationToken);
            if (existingBrand is { IsSuccess: true, Data: not null } && existingBrand.Data.BrandId != brand.BrandId)
            {
                logger.LogWarning("Brand with name '{Name}' already exists", request.Name);
                return Result<UpdateBrandResponse>.Failure("Brand with this name already exists");
            }
            
            brand.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Slug))
        {
            var existingSlug = await brandRepository.GetBySlugAsync(request.Slug, cancellationToken);
            if (existingSlug is { IsSuccess: true, Data: not null } && existingSlug.Data.BrandId != brand.BrandId)
            {
                logger.LogWarning("Brand with slug '{Slug}' already exists", request.Slug);
                return Result<UpdateBrandResponse>.Failure("Brand with this slug already exists");
            }
            brand.Slug = request.Slug;
        }
        
        var updated = await brandRepository.UpdateAsync(brand, cancellationToken);
        if (updated.IsSuccess)
        {
            logger.LogInformation("Brand updated successfully: {BrandId}", request.BrandId);
            return Result<UpdateBrandResponse>.Success(mapper.Map<UpdateBrandResponse>(brand));
        }
        
        logger.LogError("Failed to update brand: {BrandId}", request.BrandId);
        return Result<UpdateBrandResponse>.Failure("Failed to update brand");
    }
}