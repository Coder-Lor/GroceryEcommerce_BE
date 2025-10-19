using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Brand.Handlers;

public class UpdateBrandStatusCommandHandler(
    IMapper mapper,
    IBrandRepository repository,
    ICurrentUserService currentUserService,
    ILogger<UpdateBrandStatusCommandHandler> logger
) : IRequestHandler<UpdateBrandStatusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateBrandStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating brand status: {BrandId} to {Status}", request.BrandId, request.Status);
        
        var brandResult = await repository.GetByIdAsync(request.BrandId, cancellationToken);
        if (!brandResult.IsSuccess || brandResult.Data is null)
        {
            logger.LogWarning("Brand not found: {BrandId}", request.BrandId);
            return Result<bool>.Failure("Brand not found");
        }
        
        var brand = brandResult.Data;
        brand.Status = request.Status;
        brand.UpdatedAt = DateTime.UtcNow;
        brand.UpdatedBy = currentUserService.GetCurrentUserId();
        
        var updated = await repository.UpdateAsync(brand, cancellationToken);
        if (updated.IsSuccess)
        {
            logger.LogInformation("Brand status updated successfully: {BrandId}", request.BrandId);
            return Result<bool>.Success(true);
        }
        logger.LogError("Failed to update brand status: {BrandId}", request.BrandId);
        return Result<bool>.Failure("Failed to update brand status");
    }
}