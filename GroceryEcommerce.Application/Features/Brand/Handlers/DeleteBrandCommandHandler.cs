using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Brand.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Brand.Handlers;

public class DeleteBrandCommandHandler (
    IBrandRepository repository,
    ILogger<DeleteBrandCommandHandler> logger
) : IRequestHandler<DeleteBrandCommand, Result<bool>>
{
    private readonly IBrandRepository _repository = repository;
    private readonly ILogger<DeleteBrandCommandHandler> _logger = logger;
    
    public async Task<Result<bool>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting brand: {BrandId}", request.BrandId);
        
        var brandResult = await _repository.GetByIdAsync(request.BrandId, cancellationToken);
        if (brandResult is { IsSuccess: true, Data: not null })
        {
            _logger.LogInformation("Deleting brand: {BrandId}", request.BrandId);
            return Result<bool>.Failure("Brand not found");
        }
        
        var isUsedResult = await _repository.IsBrandInUseAsync(request.BrandId, cancellationToken);
        if (isUsedResult is { IsSuccess: true, Data: true })
        {
            _logger.LogInformation("Deleting brand: {BrandId}", request.BrandId);
            return Result<bool>.Failure("Brand is used");
        }
        
        var deleteResult = await _repository.DeleteAsync(request.BrandId, cancellationToken);
        if (deleteResult is { IsSuccess: true })
        {
            _logger.LogInformation("Brand deleted successfully: {BrandId}", request.BrandId);
            return Result<bool>.Success(true);
        }
        
        _logger.LogError("Failed to delete brand: {BrandId}", request.BrandId);
        return Result<bool>.Failure("Failed to delete brand"); 
    }
}