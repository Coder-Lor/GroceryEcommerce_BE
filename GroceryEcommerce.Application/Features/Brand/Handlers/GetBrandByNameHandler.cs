using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Brand.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Brand.Handlers;

public class GetBrandByNameHandler(
    IMapper mapper,
    IBrandRepository repository,
    ILogger<GetBrandByNameHandler> logger
) : IRequestHandler<GetBrandByNameQuery, Result<GetBrandByNameResponse>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IBrandRepository _repository = repository;
    private readonly ILogger<GetBrandByNameHandler> _logger = logger;

    public async Task<Result<GetBrandByNameResponse>> Handle(GetBrandByNameQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetBrandByNameQuery for brand: {Name}", request.Name);

        var brandResult = await _repository.GetByNameAsync(request.Name, cancellationToken);
        if (!brandResult.IsSuccess || brandResult.Data is null)
        {
            _logger.LogWarning("Brand not found: {Name}", request.Name);
            return Result<GetBrandByNameResponse>.Failure("Brand not found");
        }

        var response = _mapper.Map<GetBrandByNameResponse>(brandResult.Data);
        return Result<GetBrandByNameResponse>.Success(response);
    }
}