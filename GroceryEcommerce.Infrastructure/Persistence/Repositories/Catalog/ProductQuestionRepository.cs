using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Catalog;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Catalog;

public class ProductQuestionRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductQuestionRepository> logger
) : BasePagedRepository<ProductQuestionEntity, ProductQuestion>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IProductQuestionRepository
{

    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "answer" => ProductQuestionFields.Answer,
            "answeredby" => ProductQuestionFields.AnsweredBy,
            "createdat" => ProductQuestionFields.CreatedAt,
            "productid" => ProductQuestionFields.ProductId,
            "question" => ProductQuestionFields.Question,
            "questionid" => ProductQuestionFields.QuestionId,
            "status" => ProductQuestionFields.Status,
            "updatedat" => ProductQuestionFields.UpdatedAt,
            "userid" => ProductQuestionFields.UserId,
            _ => ProductQuestionFields.QuestionId
        };
    }

    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField> {
            new SearchableField("Answer", typeof(string)),
			new SearchableField("AnsweredBy", typeof(Guid)),
			new SearchableField("CreatedAt", typeof(DateTime)),
			new SearchableField("ProductId", typeof(Guid)),
			new SearchableField("Question", typeof(string)),
			new SearchableField("QuestionId", typeof(Guid)),
			new SearchableField("Status", typeof(short)),
			new SearchableField("UpdatedAt", typeof(DateTime)),
			new SearchableField("UserId", typeof(Guid)),
        };
    }

    public override string GetDefaultSortField()
    {
        return "QuestionId";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping> {
            new FieldMapping { FieldName = "Answer", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "AnsweredBy", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ProductId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Question", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "QuestionId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Status", FieldType = typeof(short), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UpdatedAt", FieldType = typeof(DateTime), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase) {
            { "answer", ProductQuestionFields.Answer },
            { "answeredby", ProductQuestionFields.AnsweredBy },
            { "createdat", ProductQuestionFields.CreatedAt },
            { "productid", ProductQuestionFields.ProductId },
            { "question", ProductQuestionFields.Question },
            { "questionid", ProductQuestionFields.QuestionId },
            { "status", ProductQuestionFields.Status },
            { "updatedat", ProductQuestionFields.UpdatedAt },
            { "userid", ProductQuestionFields.UserId },
        };
    }

    protected override EntityQuery<ProductQuestionEntity> ApplySearch(EntityQuery<ProductQuestionEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        searchTerm = searchTerm.Trim().ToLower();
        return query.Where(
            ProductQuestionFields.Question.Contains(searchTerm) |
            ProductQuestionFields.Answer.Contains(searchTerm)
        );
    }

    protected override EntityQuery<ProductQuestionEntity> ApplySorting(EntityQuery<ProductQuestionEntity> query, string? sortBy, SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        var sortField = GetSortField(sortBy);
        if (sortField is null) return query;
        return sortDirection == SortDirection.Descending
            ? query.OrderBy(sortField.Descending())
            : query.OrderBy(sortField.Ascending());
    }

    protected override EntityQuery<ProductQuestionEntity> ApplyDefaultSorting(EntityQuery<ProductQuestionEntity> query)
    {
        return query.OrderBy(ProductQuestionFields.QuestionId.Ascending());
    }

    protected override async Task<IList<ProductQuestionEntity>> FetchEntitiesAsync(EntityQuery<ProductQuestionEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<ProductQuestionEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return ProductQuestionFields.QuestionId;
    }

    protected override object GetEntityId(ProductQuestionEntity entity, EntityField2 primaryKeyField)
    {
        return entity.QuestionId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public async Task<Result<ProductQuestion?>> GetByIdAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        if (questionId == Guid.Empty)
        {
            logger.LogWarning("Question id is required");
            return Result<ProductQuestion?>.Failure("Invalid question ID.");   
        }
        return await GetSingleAsync(ProductQuestionFields.QuestionId, questionId, "ProductQuestion", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<PagedResult<ProductQuestion>>> GetByProductIdAsync(PagedRequest request, Guid productId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("ProductId", productId), "QuestionId", SortDirection.Ascending, cancellationToken);

    public async Task<Result<PagedResult<ProductQuestion>>> GetByUserIdAsync(PagedRequest request, Guid userId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("UserId", userId), "QuestionId", SortDirection.Ascending, cancellationToken);

    public async Task<Result<ProductQuestion>> CreateAsync(ProductQuestion question, CancellationToken cancellationToken = default)
    {
        try {
            var entity = Mapper.Map<ProductQuestionEntity>(question);
            entity.IsNew = true;
            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync($"ProductQuestion_{entity.QuestionId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductQuestions_ByProduct_{entity.ProductId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductQuestions_ByUser_{entity.UserId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductQuestions_ByStatus_{entity.Status}", cancellationToken);
                logger.LogInformation("Product question created: {QuestionId}", entity.QuestionId);
                return Result<ProductQuestion>.Success(Mapper.Map<ProductQuestion>(entity));
            }
            logger.LogWarning("Product question not created: {QuestionId}", entity.QuestionId);
            return Result<ProductQuestion>.Failure("Product question not created.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error creating product question: {Question}", question);
            return Result<ProductQuestion>.Failure("An error occurred while creating product question.", ex.Message);
        }
    }

    public Task<Result<bool>> UpdateAsync(ProductQuestion question, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> DeleteAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        try {
            var entity = new ProductQuestionEntity(questionId);
            var adapter = GetAdapter();
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted) {
                await CacheService.RemoveAsync($"ProductQuestion_{questionId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductQuestions_ByProduct_{questionId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductQuestions_ByUser_{questionId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductQuestions_ByStatus_{questionId}", cancellationToken);
                logger.LogInformation("Product question deleted: {QuestionId}", questionId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product question not deleted: {QuestionId}", questionId);
            return Result<bool>.Failure("Product question not deleted.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error deleting product question: {QuestionId}", questionId);
            return Result<bool>.Failure("An error occurred while deleting product question.", ex.Message);
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        if (questionId == Guid.Empty) {
            logger.LogWarning("Question id is required");
            return Result<bool>.Failure("Invalid question ID.");
        }
        return await ExistsByCountAsync(ProductQuestionFields.QuestionId, questionId, cancellationToken);
    }

    public async Task<Result<PagedResult<ProductQuestion>>> GetUnansweredQuestionsAsync(PagedRequest request, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("Status", 1), "QuestionId", SortDirection.Ascending, cancellationToken);

    public async Task<Result<PagedResult<ProductQuestion>>> GetAnsweredQuestionsAsync(PagedRequest request, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("Status", 2), "QuestionId", SortDirection.Ascending, cancellationToken);

    public async Task<Result<PagedResult<ProductQuestion>>> GetByStatusAsync(PagedRequest request, short status, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("Status", status), "QuestionId", SortDirection.Ascending, cancellationToken);

    public async Task<Result<bool>> AnswerQuestionAsync(Guid questionId, string answer, Guid answeredBy,
        CancellationToken cancellationToken = default)
    {
        try {
            var entity = new ProductQuestionEntity(questionId);
            entity.Answer = answer;
            entity.AnsweredBy = answeredBy;
            entity.Status = 2;
            entity.UpdatedAt = DateTime.UtcNow;
            var adapter = GetAdapter();
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (updated) {
                await CacheService.RemoveAsync($"ProductQuestion_{questionId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductQuestions_ByProduct_{questionId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductQuestions_ByUser_{questionId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductQuestions_ByStatus_{questionId}", cancellationToken);
                logger.LogInformation("Product question answered: {QuestionId}", questionId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product question not answered: {QuestionId}", questionId);
            return Result<bool>.Failure("Product question not answered.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error answering product question: {QuestionId}", questionId);
            return Result<bool>.Failure("An error occurred while answering product question.", ex.Message);
        }
    }

    public async Task<Result<int>> GetQuestionCountByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty) {
            logger.LogWarning("Product id is required");
            return Result<int>.Failure("Invalid product ID.");
        }
        return await CountByFieldAsync(ProductQuestionFields.ProductId, productId, cancellationToken);
    }

    public async Task<Result<int>> GetUnansweredCountAsync(CancellationToken cancellationToken = default)
    {
        try {
            return await CountByFieldAsync(ProductQuestionFields.Status, 1, cancellationToken);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error getting unanswered question count");
            return Result<int>.Failure("An error occurred while getting unanswered question count.", ex.Message);
        }
    }
}