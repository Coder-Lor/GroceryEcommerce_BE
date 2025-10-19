using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Brand.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Brand.Handlers;

public class GetBrandBySlugHandler(
    IMapper mapper,
    IBrandRepository repository,
    ILogger<GetBrandBySlugHandler> logger
) : IRequestHandler<GetBrandBySlugQuery, Result<GetBrandBySlugResponse>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IBrandRepository _repository = repository;
    private readonly ILogger<GetBrandBySlugHandler> _logger = logger;

    public async Task<Result<GetBrandBySlugResponse>> Handle(GetBrandBySlugQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetBrandBySlugQuery for brand: {Slug}", request.Slug);

        var brandResult = await _repository.GetBySlugAsync(request.Slug, cancellationToken);
        if (!brandResult.IsSuccess || brandResult.Data is null)
        {
            _logger.LogWarning("Brand not found: {Slug}", request.Slug);
            return Result<GetBrandBySlugResponse>.Failure("Brand not found");
        }

        var response = _mapper.Map<GetBrandBySlugResponse>(brandResult.Data);
        return Result<GetBrandBySlugResponse>.Success(response);
    }
}    