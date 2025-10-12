using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.System;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Systems;

public interface ISystemRepository
{
    // System Setting operations
    Task<Result<List<SystemSetting>>> GetSystemSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result<SystemSetting?>> GetSystemSettingByIdAsync(Guid settingId, CancellationToken cancellationToken = default);
    Task<Result<SystemSetting?>> GetSystemSettingByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<Result<SystemSetting>> CreateSystemSettingAsync(SystemSetting setting, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateSystemSettingAsync(SystemSetting setting, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteSystemSettingAsync(Guid settingId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateSystemSettingValueAsync(string key, string value, CancellationToken cancellationToken = default);
    Task<Result<string?>> GetSystemSettingValueAsync(string key, CancellationToken cancellationToken = default);
    Task<Result<Dictionary<string, string>>> GetSystemSettingsByCategoryAsync(string category, CancellationToken cancellationToken = default);

    // Email Template operations
    Task<Result<List<EmailTemplate>>> GetEmailTemplatesAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<EmailTemplate>>> GetEmailTemplatesPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<EmailTemplate?>> GetEmailTemplateByIdAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Result<EmailTemplate?>> GetEmailTemplateByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<Result<List<EmailTemplate>>> GetEmailTemplatesByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<Result<EmailTemplate>> CreateEmailTemplateAsync(EmailTemplate template, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateEmailTemplateAsync(EmailTemplate template, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteEmailTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ActivateEmailTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeactivateEmailTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Result<List<EmailTemplate>>> GetActiveEmailTemplatesAsync(CancellationToken cancellationToken = default);

    // Currency operations
    Task<Result<List<Currency>>> GetCurrenciesAsync(CancellationToken cancellationToken = default);
    Task<Result<Currency?>> GetCurrencyByIdAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<Currency?>> GetCurrencyByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<Currency?>> GetDefaultCurrencyAsync(CancellationToken cancellationToken = default);
    Task<Result<List<Currency>>> GetActiveCurrenciesAsync(CancellationToken cancellationToken = default);
    Task<Result<Currency>> CreateCurrencyAsync(Currency currency, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateCurrencyAsync(Currency currency, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<bool>> SetDefaultCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ActivateCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeactivateCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency, CancellationToken cancellationToken = default);

    // System utilities
    Task<Result<bool>> ClearSystemCacheAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> BackupSystemDataAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> RestoreSystemDataAsync(byte[] backupData, CancellationToken cancellationToken = default);
    Task<Result<Dictionary<string, object>>> GetSystemHealthAsync(CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetSystemLogsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}
