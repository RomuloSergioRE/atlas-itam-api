using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using AutoMapper;

namespace Atlas.Itam.Application.Common.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Asset, DTOs.Assets.AssetDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : null))
            .ForMember(d => d.LocationName, opt => opt.MapFrom(s => s.Location != null ? s.Location.Name : null))
            .ForMember(d => d.CurrentUserName, opt => opt.MapFrom(s => s.CurrentUser != null ? s.CurrentUser.Name : null));

        CreateMap<Asset, DTOs.Assets.AssetSummaryDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : null))
            .ForMember(d => d.CurrentUserName, opt => opt.MapFrom(s => s.CurrentUser != null ? s.CurrentUser.Name : null));

        CreateMap<Request, DTOs.Requests.RequestDto>()
            .ForMember(d => d.AssetName, opt => opt.MapFrom(s => s.Asset != null ? s.Asset.Name : null))
            .ForMember(d => d.AssetPatrimonyNumber, opt => opt.MapFrom(s => s.Asset != null ? s.Asset.PatrimonyNumber : null))
            .ForMember(d => d.RequestedByName, opt => opt.MapFrom(s => s.RequestedBy != null ? s.RequestedBy.Name : null))
            .ForMember(d => d.RequestedByEmail, opt => opt.MapFrom(s => s.RequestedBy != null ? s.RequestedBy.Email : null))
            .ForMember(d => d.ApprovedByName, opt => opt.MapFrom(s => s.ApprovedBy != null ? s.ApprovedBy.Name : null));

        CreateMap<Request, DTOs.Requests.RequestSummaryDto>()
            .ForMember(d => d.AssetName, opt => opt.MapFrom(s => s.Asset != null ? s.Asset.Name : null))
            .ForMember(d => d.RequestedByName, opt => opt.MapFrom(s => s.RequestedBy != null ? s.RequestedBy.Name : null));

        CreateMap<User, DTOs.Users.UserDto>()
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.Name : null));
    }
}
