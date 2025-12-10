using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Marketing;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Marketing;

public class GiftCardRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<GiftCardRepository> logger
) : BasePagedRepository<GiftCardEntity, GiftCard>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IGiftCardRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("Code", typeof(string)),
            new SearchableField("GiftCardId", typeof(Guid)),
            new SearchableField("CreatedBy", typeof(Guid)),
            new SearchableField("IsActive", typeof(bool)),
            new SearchableField("Balance", typeof(decimal)),
            new SearchableField("InitialAmount", typeof(decimal)),
            new SearchableField("CreatedAt", typeof(DateTime)),
            new SearchableField("ExpiresAt", typeof(DateTime))
        };
    }

    public override string GetDefaultSortField()
    {
        return "CreatedAt";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "Code", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "GiftCardId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedBy", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "IsActive", FieldType = typeof(bool), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Balance", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "InitialAmount", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ExpiresAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "code", GiftCardFields.Code },
            { "giftcardid", GiftCardFields.GiftCardId },
            { "createdby", GiftCardFields.CreatedBy },
            { "isactive", GiftCardFields.IsActive },
            { "balance", GiftCardFields.Balance },
            { "initialamount", GiftCardFields.InitialAmount },
            { "createdat", GiftCardFields.CreatedAt },
            { "expiresat", GiftCardFields.ExpiresAt }
        };
    }

    protected override EntityQuery<GiftCardEntity> ApplySearch(EntityQuery<GiftCardEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        return query.Where(SearchPredicateBuilder.BuildContainsPredicate(searchTerm, GiftCardFields.Code));
    }

    protected override EntityQuery<GiftCardEntity> ApplySorting(EntityQuery<GiftCardEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "code" => GiftCardFields.Code,
            "giftcardid" => GiftCardFields.GiftCardId,
            "createdby" => GiftCardFields.CreatedBy,
            "isactive" => GiftCardFields.IsActive,
            "balance" => GiftCardFields.Balance,
            "initialamount" => GiftCardFields.InitialAmount,
            "createdat" => GiftCardFields.CreatedAt,
            "expiresat" => GiftCardFields.ExpiresAt,
            _ => GiftCardFields.CreatedAt
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<GiftCardEntity> ApplyDefaultSorting(EntityQuery<GiftCardEntity> query)
    {
        return query.OrderBy(GiftCardFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return GiftCardFields.GiftCardId;
    }

    protected override object GetEntityId(GiftCardEntity entity, EntityField2 primaryKeyField)
    {
        return entity.GiftCardId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return GiftCardFields.GiftCardId.In(ids);
    }

    public async Task<Result<GiftCard?>> GetByIdAsync(Guid giftCardId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(GiftCardFields.GiftCardId, giftCardId, "GiftCard", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<GiftCard?>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<GiftCardEntity>()
                .Where(GiftCardFields.Code == code);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                Logger.LogWarning("GiftCard not found by code: {Code}", code);
                return Result<GiftCard?>.Success(null);
            }
            
            var domainEntity = Mapper.Map<GiftCard>(entity);
            return Result<GiftCard?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting GiftCard by code: {Code}", code);
            return Result<GiftCard?>.Failure("An error occurred while fetching GiftCard.");
        }
    }

    public async Task<Result<List<GiftCard>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<GiftCardEntity>()
                .OrderBy(GiftCardFields.CreatedAt.Descending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<GiftCard>>(entities);
            
            return Result<List<GiftCard>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all GiftCards");
            return Result<List<GiftCard>>.Failure("An error occurred while fetching GiftCards.");
        }
    }

    public async Task<Result<PagedResult<GiftCard>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await base.GetPagedAsync(request, cancellationToken);
    }

    public async Task<Result<GiftCard>> CreateAsync(GiftCard giftCard, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<GiftCardEntity>(giftCard);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("GiftCard*", cancellationToken);
            
            Logger.LogInformation("GiftCard created: {GiftCardId}", entity.GiftCardId);
            var domainEntity = Mapper.Map<GiftCard>(giftCard);
            return Result<GiftCard>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating GiftCard");
            return Result<GiftCard>.Failure("An error occurred while creating GiftCard.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(GiftCard giftCard, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<GiftCardEntity>(giftCard);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("GiftCard*", cancellationToken);
            
            Logger.LogInformation("GiftCard updated: {GiftCardId}", entity.GiftCardId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating GiftCard: {GiftCardId}", giftCard.GiftCardId);
            return Result<bool>.Failure("An error occurred while updating GiftCard.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid giftCardId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new GiftCardEntity(giftCardId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("GiftCard*", cancellationToken);
            
            Logger.LogInformation("GiftCard deleted: {GiftCardId}", giftCardId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting GiftCard: {GiftCardId}", giftCardId);
            return Result<bool>.Failure("An error occurred while deleting GiftCard.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<GiftCardEntity>()
                .Where(GiftCardFields.Code == code)
                .Limit(1)
                .Select(() => Functions.CountRow());
            
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<bool>.Success(count > 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking if GiftCard exists by code: {Code}", code);
            return Result<bool>.Failure("An error occurred while checking GiftCard existence.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid giftCardId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(GiftCardFields.GiftCardId, giftCardId, cancellationToken);
    }

    public async Task<Result<PagedResult<GiftCard>>> GetActiveGiftCardsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("IsActive", true),
            GiftCardFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<GiftCard>>> GetExpiredGiftCardsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<GiftCardEntity>()
                .Where(GiftCardFields.ExpiresAt <= DateTime.UtcNow)
                .OrderBy(GiftCardFields.ExpiresAt.Descending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<GiftCard>>(entities);
            
            var totalCount = domainEntities.Count;
            var pagedItems = domainEntities
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            var result = new PagedResult<GiftCard>(pagedItems, totalCount, request.Page, request.PageSize);
            return Result<PagedResult<GiftCard>>.Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting expired GiftCards");
            return Result<PagedResult<GiftCard>>.Failure("An error occurred while fetching expired GiftCards.");
        }
    }

    public async Task<Result<PagedResult<GiftCard>>> GetGiftCardsByUserAsync(PagedRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("CreatedBy", userId),
            GiftCardFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<bool>> IsGiftCardValidAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var giftCardResult = await GetByCodeAsync(code, cancellationToken);
            if (!giftCardResult.IsSuccess || giftCardResult.Data == null)
            {
                return Result<bool>.Success(false);
            }
            
            var giftCard = giftCardResult.Data;
            var isValid = giftCard.IsActive 
                && (giftCard.ExpiresAt == null || giftCard.ExpiresAt > DateTime.UtcNow)
                && giftCard.Balance > 0;
            
            return Result<bool>.Success(isValid);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating GiftCard: {Code}", code);
            return Result<bool>.Failure("An error occurred while validating GiftCard.");
        }
    }

    public async Task<Result<decimal>> GetRemainingBalanceAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var giftCardResult = await GetByCodeAsync(code, cancellationToken);
            if (!giftCardResult.IsSuccess || giftCardResult.Data == null)
            {
                return Result<decimal>.Failure("GiftCard not found.");
            }
            
            return Result<decimal>.Success(giftCardResult.Data.Balance);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting remaining balance for GiftCard: {Code}", code);
            return Result<decimal>.Failure("An error occurred while getting remaining balance.");
        }
    }

    public async Task<Result<bool>> RedeemGiftCardAsync(string code, decimal amount, CancellationToken cancellationToken = default)
    {
        try
        {
            var giftCardResult = await GetByCodeAsync(code, cancellationToken);
            if (!giftCardResult.IsSuccess || giftCardResult.Data == null)
            {
                return Result<bool>.Failure("GiftCard not found.");
            }
            
            var giftCard = giftCardResult.Data;
            if (giftCard.Balance < amount)
            {
                return Result<bool>.Failure("Insufficient balance.");
            }
            
            giftCard.Balance -= amount;
            var updateResult = await UpdateAsync(giftCard, cancellationToken);
            
            if (!updateResult.IsSuccess)
            {
                return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to redeem GiftCard.");
            }
            
            Logger.LogInformation("GiftCard {Code} redeemed: {Amount}", code, amount);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error redeeming GiftCard: {Code}", code);
            return Result<bool>.Failure("An error occurred while redeeming GiftCard.");
        }
    }
}

