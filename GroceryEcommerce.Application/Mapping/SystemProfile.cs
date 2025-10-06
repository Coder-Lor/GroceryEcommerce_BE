using AutoMapper;
using GroceryEcommerce.Application.Models.System;
using GroceryEcommerce.Domain.Entities.System;

namespace GroceryEcommerce.Application.Mapping;

public class SystemProfile : Profile
{
    public SystemProfile()
    {
        // System Setting mappings
        CreateMap<SystemSetting, SystemSettingDto>()
            .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.UpdatedByUser != null ? $"{src.UpdatedByUser.FirstName} {src.UpdatedByUser.LastName}".Trim() : null));

        CreateMap<CreateSystemSettingRequest, SystemSetting>()
            .ForMember(dest => dest.SettingId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateSystemSettingRequest, SystemSetting>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Email Template mappings
        CreateMap<EmailTemplate, EmailTemplateDto>()
            .ForMember(dest => dest.AvailableVariables, opt => opt.Ignore()); // Will be set in service
            // .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.UpdatedByUser != null ? $"{src.UpdatedByUser.FirstName} {src.UpdatedByUser.LastName}".Trim() : null))

        CreateMap<CreateEmailTemplateRequest, EmailTemplate>()
            .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateEmailTemplateRequest, EmailTemplate>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Currency mappings
        CreateMap<Currency, CurrencyDto>();

        CreateMap<CreateCurrencyRequest, Currency>()
            .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => Guid.NewGuid()));
            // .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateCurrencyRequest, Currency>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}

