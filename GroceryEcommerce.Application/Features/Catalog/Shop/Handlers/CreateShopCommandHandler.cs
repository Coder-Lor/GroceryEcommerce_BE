using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Handlers;

public class CreateShopCommandHandler(
    IMapper mapper,
    IShopRepository shopRepository,
    ICurrentUserService currentUserService,
    ILogger<CreateShopCommandHandler> logger
) : IRequestHandler<CreateShopCommand, Result<CreateShopResponse>>
{
    public async Task<Result<CreateShopResponse>> Handle(CreateShopCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting shop creation for {Name}", request.Name);

        var currentUserId = currentUserService.GetCurrentUserId();
        var currentUserEmail = currentUserService.GetCurrentUserEmail();

        if (currentUserId is null)
        {
            logger.LogWarning("Could not retrieve current user ID");
            return Result<CreateShopResponse>.Failure("Unable to identify current user");
        }

        logger.LogInformation("Creating shop for user: {UserId} ({Email})", currentUserId, currentUserEmail);

        // Slug
        var slug = request.Slug ?? GenerateSlug(request.Name);
        var existingSlug = await shopRepository.GetBySlugAsync(slug, cancellationToken);
        if (existingSlug.IsSuccess && existingSlug.Data is not null)
        {
            slug = $"{slug}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        var ownerUserId = request.OwnerUserId != Guid.Empty ? request.OwnerUserId : currentUserId.Value;

        var shop = new Domain.Entities.Catalog.Shop
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            LogoUrl = request.LogoUrl,
            Status = request.Status,
            IsAccepted = false, // Shop mới đăng ký chưa được chấp nhận
            OwnerUserId = ownerUserId,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await shopRepository.CreateAsync(shop, cancellationToken);
        if (!createResult.IsSuccess)
        {
            logger.LogError("Failed to create shop: {Name}", request.Name);
            return Result<CreateShopResponse>.Failure("Failed to create shop");
        }

        logger.LogInformation("Shop created successfully: {ShopId} by user: {UserId}", shop.ShopId, currentUserId);

        var response = mapper.Map<CreateShopResponse>(shop);
        return Result<CreateShopResponse>.Success(response);
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


