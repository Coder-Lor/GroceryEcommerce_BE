using AutoMapper;
using GroceryEcommerce.Domain.Entities.System;
using GroceryEcommerce.EntityClasses;

namespace GroceryEcommerce.Infrastructure.Mapping;

public class SystemMappingProfile : Profile
{
    public SystemMappingProfile()
    {
        // Currency mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<Currency, CurrencyEntity>();

        CreateMap<CurrencyEntity, Currency>();

        // EmailTemplate mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<EmailTemplate, EmailTemplateEntity>();

        CreateMap<EmailTemplateEntity, EmailTemplate>();

        // SystemSetting mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<SystemSetting, SystemSettingEntity>()
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<SystemSettingEntity, SystemSetting>()
            .ForMember(dest => dest.UpdatedByUser, opt => opt.Ignore());
    }
}
