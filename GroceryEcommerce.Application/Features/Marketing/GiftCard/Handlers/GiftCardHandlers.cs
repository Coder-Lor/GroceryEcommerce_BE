using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.GiftCard.Commands;
using GroceryEcommerce.Application.Features.Marketing.GiftCard.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using GroceryEcommerce.Domain.Entities.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.GiftCard.Handlers;

public class CreateGiftCardCommandHandler(
    IGiftCardRepository repository,
    IMapper mapper,
    ILogger<CreateGiftCardCommandHandler> logger
) : IRequestHandler<CreateGiftCardCommand, Result<GiftCardDto>>
{
    public async Task<Result<GiftCardDto>> Handle(CreateGiftCardCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating GiftCard with InitialAmount: {InitialAmount}", request.InitialAmount);

        // Generate unique code
        var code = GenerateGiftCardCode();

        // Check if code exists
        var existsResult = await repository.ExistsAsync(code, cancellationToken);
        if (existsResult.IsSuccess && existsResult.Data)
        {
            // Regenerate if exists
            code = GenerateGiftCardCode();
        }

        var giftCard = new GiftCard
        {
            GiftCardId = Guid.NewGuid(),
            Code = code,
            InitialAmount = request.InitialAmount,
            Balance = request.InitialAmount,
            ExpiresAt = request.ValidTo,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.PurchasedBy
        };

        var result = await repository.CreateAsync(giftCard, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to create GiftCard");
            return Result<GiftCardDto>.Failure(result.ErrorMessage ?? "Failed to create GiftCard.");
        }

        var dto = mapper.Map<GiftCardDto>(result.Data);
        logger.LogInformation("GiftCard created: {GiftCardId}, Code: {Code}", result.Data.GiftCardId, code);
        return Result<GiftCardDto>.Success(dto);
    }

    private static string GenerateGiftCardCode()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

public class UpdateGiftCardCommandHandler(
    IGiftCardRepository repository,
    IMapper mapper,
    ILogger<UpdateGiftCardCommandHandler> logger
) : IRequestHandler<UpdateGiftCardCommand, Result<GiftCardDto>>
{
    public async Task<Result<GiftCardDto>> Handle(UpdateGiftCardCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating GiftCard: {GiftCardId}", request.GiftCardId);

        var existingResult = await repository.GetByIdAsync(request.GiftCardId, cancellationToken);
        if (!existingResult.IsSuccess || existingResult.Data == null)
        {
            return Result<GiftCardDto>.Failure("GiftCard not found.");
        }

        var giftCard = existingResult.Data;
        giftCard.Balance = request.InitialAmount; // Update balance if initial amount changed
        giftCard.ExpiresAt = request.ValidTo;
        giftCard.IsActive = request.Status == 1;

        var updateResult = await repository.UpdateAsync(giftCard, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            logger.LogError("Failed to update GiftCard: {GiftCardId}", request.GiftCardId);
            return Result<GiftCardDto>.Failure(updateResult.ErrorMessage ?? "Failed to update GiftCard.");
        }

        var updatedResult = await repository.GetByIdAsync(request.GiftCardId, cancellationToken);
        if (!updatedResult.IsSuccess || updatedResult.Data == null)
        {
            return Result<GiftCardDto>.Failure("Failed to retrieve updated GiftCard.");
        }

        var dto = mapper.Map<GiftCardDto>(updatedResult.Data);
        logger.LogInformation("GiftCard updated: {GiftCardId}", request.GiftCardId);
        return Result<GiftCardDto>.Success(dto);
    }
}

public class DeleteGiftCardCommandHandler(
    IGiftCardRepository repository,
    ILogger<DeleteGiftCardCommandHandler> logger
) : IRequestHandler<DeleteGiftCardCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteGiftCardCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting GiftCard: {GiftCardId}", request.GiftCardId);

        var result = await repository.DeleteAsync(request.GiftCardId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to delete GiftCard: {GiftCardId}", request.GiftCardId);
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete GiftCard.");
        }

        logger.LogInformation("GiftCard deleted: {GiftCardId}", request.GiftCardId);
        return Result<bool>.Success(true);
    }
}

public class GetGiftCardByIdQueryHandler(
    IGiftCardRepository repository,
    IMapper mapper,
    ILogger<GetGiftCardByIdQueryHandler> logger
) : IRequestHandler<GetGiftCardByIdQuery, Result<GiftCardDto?>>
{
    public async Task<Result<GiftCardDto?>> Handle(GetGiftCardByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting GiftCard by Id: {GiftCardId}", request.GiftCardId);

        var result = await repository.GetByIdAsync(request.GiftCardId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get GiftCard: {GiftCardId}", request.GiftCardId);
            return Result<GiftCardDto?>.Failure(result.ErrorMessage ?? "Failed to get GiftCard.");
        }

        if (result.Data == null)
        {
            return Result<GiftCardDto?>.Success(null);
        }

        var dto = mapper.Map<GiftCardDto>(result.Data);
        return Result<GiftCardDto?>.Success(dto);
    }
}

public class GetGiftCardByCodeQueryHandler(
    IGiftCardRepository repository,
    IMapper mapper,
    ILogger<GetGiftCardByCodeQueryHandler> logger
) : IRequestHandler<GetGiftCardByCodeQuery, Result<GiftCardDto?>>
{
    public async Task<Result<GiftCardDto?>> Handle(GetGiftCardByCodeQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting GiftCard by Code: {Code}", request.Code);

        var result = await repository.GetByCodeAsync(request.Code, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get GiftCard by Code: {Code}", request.Code);
            return Result<GiftCardDto?>.Failure(result.ErrorMessage ?? "Failed to get GiftCard.");
        }

        if (result.Data == null)
        {
            return Result<GiftCardDto?>.Success(null);
        }

        var dto = mapper.Map<GiftCardDto>(result.Data);
        return Result<GiftCardDto?>.Success(dto);
    }
}

public class GetGiftCardsPagingQueryHandler(
    IGiftCardRepository repository,
    IMapper mapper,
    ILogger<GetGiftCardsPagingQueryHandler> logger
) : IRequestHandler<GetGiftCardsPagingQuery, Result<PagedResult<GiftCardDto>>>
{
    public async Task<Result<PagedResult<GiftCardDto>>> Handle(GetGiftCardsPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting GiftCards paging - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

        var result = await repository.GetPagedAsync(request.Request, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get GiftCards paging");
            return Result<PagedResult<GiftCardDto>>.Failure(result.ErrorMessage ?? "Failed to get GiftCards.");
        }

        var dtos = mapper.Map<List<GiftCardDto>>(result.Data?.Items ?? new List<GiftCard>());
        var pagedResult = new PagedResult<GiftCardDto>(
            dtos,
            result.Data?.TotalCount ?? 0,
            result.Data?.Page ?? request.Request.Page,
            result.Data?.PageSize ?? request.Request.PageSize
        );

        return Result<PagedResult<GiftCardDto>>.Success(pagedResult);
    }
}

