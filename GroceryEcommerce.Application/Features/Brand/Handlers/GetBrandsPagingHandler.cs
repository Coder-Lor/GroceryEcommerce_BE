using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Brand.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Brand.Handlers;

public class GetBrandsPagingHandler(
    IMapper mapper,
    IBrandRepository repository,
    ILogger<GetBrandsPagingHandler> logger
) : IRequestHandler<GetBrandsPagingQuery, Result<PagedResult<BrandDto>>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IBrandRepository _repository = repository;
    private readonly ILogger<GetBrandsPagingHandler> _logger = logger;

    public async Task<Result<PagedResult<BrandDto>>> Handle(GetBrandsPagingQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetBrandsPagingQuery with PageNumber: {PageNumber}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

        var brandResult = await _repository.GetPagedAsync(request.Request, cancellationToken);
        if (!brandResult.IsSuccess || brandResult.Data is null)
        {
            _logger.LogWarning("Brand not found for the given criteria.");
            return Result<PagedResult<BrandDto>>.Failure("Brand not found");
        }

        var response = _mapper.Map<PagedResult<BrandDto>>(brandResult.Data);
        return Result<PagedResult<BrandDto>>.Success(response);
    }
}