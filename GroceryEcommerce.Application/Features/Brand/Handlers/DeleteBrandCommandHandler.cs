using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Brand.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Brand.Handlers;

public class DeleteBrandCommandHandler (
    IBrandRepository repository,
    ILogger<DeleteBrandCommandHandler> logger
) : IRequestHandler<DeleteBrandCommand, Result<bool>>
{   
    public async Task<Result<bool>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting brand: {BrandId}", request.BrandId);
        
        var brandResult = await repository.GetByIdAsync(request.BrandId, cancellationToken);
        if (brandResult is { IsSuccess: true, Data: not null })
        {
            logger.LogInformation("Deleting brand: {BrandId}", request.BrandId);
            return Result<bool>.Failure("Brand not found");
        }
        
        var isUsedResult = await repository.IsBrandInUseAsync(request.BrandId, cancellationToken);
        if (isUsedResult is { IsSuccess: true, Data: true })
        {
            logger.LogInformation("Deleting brand: {BrandId}", request.BrandId);
            return Result<bool>.Failure("Brand is used");
        }
        
        var deleteResult = await repository.DeleteAsync(request.BrandId, cancellationToken);
        if (deleteResult is { IsSuccess: true })
        {
            logger.LogInformation("Brand deleted successfully: {BrandId}", request.BrandId);
            return Result<bool>.Success(true);
        }
        
        logger.LogError("Failed to delete brand: {BrandId}", request.BrandId);
        return Result<bool>.Failure("Failed to delete brand"); 
    }
}