using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.System;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface ISystemSettingRepository
{
    // Basic CRUD operations
    Task<Result<SystemSetting?>> GetByIdAsync(Guid settingId, CancellationToken cancellationToken = default);
    Task<Result<SystemSetting?>> GetByKeyAsync(string settingKey, CancellationToken cancellationToken = default);
    Task<Result<List<SystemSetting>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<SystemSetting>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<SystemSetting>> CreateAsync(SystemSetting setting, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(SystemSetting setting, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid settingId, CancellationToken cancellationToken = default);
    
    // Setting management operations
    Task<Result<bool>> ExistsAsync(string settingKey, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid settingId, CancellationToken cancellationToken = default);
    Task<Result<List<SystemSetting>>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<Result<List<SystemSetting>>> GetPublicSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result<string?>> GetSettingValueAsync(string settingKey, CancellationToken cancellationToken = default);
    Task<Result<bool>> SetSettingValueAsync(string settingKey, string value, CancellationToken cancellationToken = default);
    Task<Result<Dictionary<string, string>>> GetSettingsByCategoryAsync(string category, CancellationToken cancellationToken = default);
}
