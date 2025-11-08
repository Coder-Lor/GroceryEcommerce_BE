using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Application.Mapping;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        // PagedResult mapping for User
        CreateMap<PagedResult<User>, PagedResult<UserDto>>()
            .ConvertUsing((src, dest, context) => new PagedResult<UserDto>(
                context.Mapper.Map<List<UserDto>>(src.Items),
                src.TotalCount,
                src.Page,
                src.PageSize
            ));

        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == 1))
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => src.EmailVerified))
            .ForMember(dest => dest.IsPhoneVerified, opt => opt.MapFrom(src => src.PhoneVerified))
            .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.LockedUntil.HasValue && src.LockedUntil > DateTime.UtcNow));

        CreateMap<UserAddress, UserAddressDto>()
            .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src => $"{src.AddressLine1}, {src.City}, {src.State} {src.ZipCode}, {src.Country}".Trim()));

        CreateMap<UserRole, UserRoleDto>();

        CreateMap<UserRoleAssignment, UserRoleAssignmentDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));

        CreateMap<UserSession, UserSessionDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}".Trim() : string.Empty));

        CreateMap<AuditLog, AuditLogDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}".Trim() : null));

        CreateMap<RefreshToken, RefreshTokenDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()));

        // Commands -> Domain entities (UserAddress)
        CreateMap<Features.Auth.UserAddresses.Commands.CreateUserAddressCommand, UserAddress>()
            .ForMember(dest => dest.AddressId, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.PostalCode))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => (DateTime?)null));

        CreateMap<Features.Auth.UserAddresses.Commands.UpdateUserAddressCommand, UserAddress>()
            .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.PostalCode))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }

    private static string GetUserStatusName(short status)
    {
        return status switch
        {
            1 => "Active",
            0 => "Inactive",
            -1 => "Banned",
            _ => "Unknown"
        };
    }
}