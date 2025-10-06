using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.System;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IEmailTemplateRepository
{
    // Basic CRUD operations
    Task<Result<EmailTemplate?>> GetByIdAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Result<EmailTemplate?>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<List<EmailTemplate>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<EmailTemplate>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<EmailTemplate>> CreateAsync(EmailTemplate template, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(EmailTemplate template, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid templateId, CancellationToken cancellationToken = default);
    
    // Template management operations
    Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Result<List<EmailTemplate>>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<EmailTemplate>>> GetTemplatesByTypeAsync(short templateType, CancellationToken cancellationToken = default);
    Task<Result<EmailTemplate?>> GetDefaultTemplateByTypeAsync(short templateType, CancellationToken cancellationToken = default);
    Task<Result<bool>> SetDefaultTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
}
