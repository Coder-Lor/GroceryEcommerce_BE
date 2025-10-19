using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Brand.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Brand.Handlers;

public class CreateBrandCommandHandler(
    IMapper mapper,
    IBrandRepository brandRepository,   
    ICurrentUserService currentUserService,
    ILogger<CreateBrandCommandHandler> logger
): IRequestHandler<CreateBrandCommand, Result<CreateBrandResponse>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IBrandRepository _brandRepository = brandRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ILogger<CreateBrandCommandHandler> _logger = logger;

  public async Task<Result<CreateBrandResponse>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
  {
        _logger.LogInformation("Starting brand creation for {Name}", request.Name);
        // Lấy thông tin user hiện tại
        var currentUserId = _currentUserService.GetCurrentUserId();
        var currentUserEmail = _currentUserService.GetCurrentUserEmail();
        
        if (currentUserId is null)
        {
            _logger.LogWarning("Could not retrieve current user ID");
            return Result<CreateBrandResponse>.Failure("Unable to identify current user");
        }

        _logger.LogInformation("Creating brand for user: {UserId} ({Email})", currentUserId, currentUserEmail);

        // Kiểm tra tên brand đã tồn tại chưa
        var existingBrand = await _brandRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingBrand is { IsSuccess: true, Data: not null })
        {
            _logger.LogWarning("Brand with name '{Name}' already exists", request.Name);
            return Result<CreateBrandResponse>.Failure("Brand with this name already exists");
        }

        // Tạo slug nếu không có
        var slug = request.Slug ?? GenerateSlug(request.Name);
        
        // Kiểm tra slug đã tồn tại chưa
        var existingSlug = await _brandRepository.GetBySlugAsync(slug, cancellationToken);
        if (existingSlug.IsSuccess && existingSlug.Data is not null)
        {
            slug = $"{slug}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        // Tạo brand entity
        var brand = new Domain.Entities.Catalog.Brand
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            LogoUrl = request.LogoUrl,
            Website = request.Website,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId.Value // Sử dụng ID của user hiện tại
        };

        // Tạo brand trong database
        var createResult = await _brandRepository.CreateAsync(brand, cancellationToken);
        if (!createResult.IsSuccess)
        {
            _logger.LogError("Failed to create brand: {Name}", request.Name);
            return Result<CreateBrandResponse>.Failure("Failed to create brand");
        }

        // Lưu thay đổi
        _logger.LogInformation("Brand created successfully: {BrandId} by user: {UserId}", brand.BrandId, currentUserId);

        // Map từ Entity sang Response
        var response = _mapper.Map<CreateBrandResponse>(brand);
        return Result<CreateBrandResponse>.Success(response);
    }


    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace("(", "")
            .Replace(")", "");
    }
}