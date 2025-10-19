using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Brand.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Brand.Handlers;

public class GetBrandByIdHandler(
    IMapper mapper,
    IBrandRepository repository,
    ILogger<GetBrandByIdHandler> logger
) : IRequestHandler<GetBrandByIdQuery, Result<GetBrandByIdResponse>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IBrandRepository _repository = repository;
    private readonly ILogger<GetBrandByIdHandler> _logger = logger;

    public async Task<Result<GetBrandByIdResponse>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetBrandByIdQuery for brand: {BrandId}", request.BrandId);

        var brandResult = await repository.GetByIdAsync(request.BrandId, cancellationToken);
        if (!brandResult.IsSuccess || brandResult.Data is null)
        {
            logger.LogWarning("Brand not found: {BrandId}", request.BrandId);
            return Result<GetBrandByIdResponse>.Failure("Brand not found");
        }

        var response = _mapper.Map<GetBrandByIdResponse>(brandResult.Data);
        return Result<GetBrandByIdResponse>.Success(response);
    }
}