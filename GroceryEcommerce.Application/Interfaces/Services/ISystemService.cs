using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using GroceryEcommerce.Application.Models.System;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface ISystemService
{
    // System Setting services
    Task<Result<List<SystemSettingDto>>> GetSystemSettingsAsync(CancellationToken cancellationToken = default);
    Task<Result<SystemSettingDto?>> GetSystemSettingByIdAsync(Guid settingId, CancellationToken cancellationToken = default);
    Task<Result<SystemSettingDto?>> GetSystemSettingByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<Result<SystemSettingDto>> CreateSystemSettingAsync(CreateSystemSettingRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateSystemSettingAsync(Guid settingId, UpdateSystemSettingRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteSystemSettingAsync(Guid settingId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateSystemSettingValueAsync(string key, string value, CancellationToken cancellationToken = default);
    Task<Result<string?>> GetSystemSettingValueAsync(string key, CancellationToken cancellationToken = default);
    Task<Result<Dictionary<string, string>>> GetSystemSettingsByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<Result<List<SystemSettingCategoryDto>>> GetSystemSettingsByCategoriesAsync(CancellationToken cancellationToken = default);

    // Email Template services
    Task<Result<List<EmailTemplateDto>>> GetEmailTemplatesAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<EmailTemplateDto>>> GetEmailTemplatesPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<EmailTemplateDto?>> GetEmailTemplateByIdAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Result<EmailTemplateDto?>> GetEmailTemplateByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<Result<List<EmailTemplateDto>>> GetEmailTemplatesByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<Result<EmailTemplateDto>> CreateEmailTemplateAsync(CreateEmailTemplateRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateEmailTemplateAsync(Guid templateId, UpdateEmailTemplateRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteEmailTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ActivateEmailTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeactivateEmailTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Result<List<EmailTemplateDto>>> GetActiveEmailTemplatesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<EmailTemplateCategoryDto>>> GetEmailTemplatesByCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> SendEmailAsync(SendEmailRequest request, CancellationToken cancellationToken = default);

    // Currency services
    Task<Result<List<CurrencyDto>>> GetCurrenciesAsync(CancellationToken cancellationToken = default);
    Task<Result<CurrencyDto?>> GetCurrencyByIdAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<CurrencyDto?>> GetCurrencyByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<CurrencyDto?>> GetDefaultCurrencyAsync(CancellationToken cancellationToken = default);
    Task<Result<List<CurrencyDto>>> GetActiveCurrenciesAsync(CancellationToken cancellationToken = default);
    Task<Result<CurrencyDto>> CreateCurrencyAsync(CreateCurrencyRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateCurrencyAsync(Guid currencyId, UpdateCurrencyRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<bool>> SetDefaultCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ActivateCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeactivateCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> ConvertCurrencyAsync(CurrencyConversionRequest request, CancellationToken cancellationToken = default);
    Task<Result<CurrencyConversionResponse>> GetCurrencyConversionAsync(CurrencyConversionRequest request, CancellationToken cancellationToken = default);

    // System utilities
    Task<Result<bool>> ClearSystemCacheAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> BackupSystemDataAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> RestoreSystemDataAsync(byte[] backupData, CancellationToken cancellationToken = default);
    Task<Result<Dictionary<string, object>>> GetSystemHealthAsync(CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetSystemLogsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}
