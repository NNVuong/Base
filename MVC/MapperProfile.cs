using AutoMapper;
using DataBase.Base;
using DataBase.Entities;
using DataTransferObjects.Base;
using DataTransferObjects.Request;
using DataTransferObjects.Response;

namespace MVC;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        MapperEntityToRequest();
        MapperEntityToResponse();
        MapperRequestToEntity();
    }

    private void MapperEntityToResponse()
    {
        CreateMap<Campus, CampusResponse>();

        CreateMap<Role, RoleResponse>();

        CreateMap<User, UserResponse>();

        CreateMap<UserCampus, UserCampusResponse>()
            .ForMember(response => response.CampusCode,
                opt => opt.MapFrom(src => src.Campus.Code))
            .ForMember(response => response.CampusName,
                opt => opt.MapFrom(src => src.Campus.Name));

        CreateMap<UserRole, UserRoleResponse>()
            .ForMember(response => response.UserEmail,
                opt => opt.MapFrom(src => src.User.Email))
            .ForMember(response => response.RoleName,
                opt => opt.MapFrom(src => src.Role.Name));

        CreateMap<Category, CategoryResponse>();

        CreateMap<CategoryDetails, CategoryDetailsResponse>()
            .ForMember(response => response.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name));
    }

    private void MapperEntityToRequest()
    {
        CreateMap<Campus, CampusRequest>();

        CreateMap<Role, RoleRequest>();

        CreateMap<User, UserRequest>();

        CreateMap<UserCampus, UserCampusRequest>();

        CreateMap<UserRole, UserRoleRequest>();

        CreateMap<Category, CategoryRequest>();

        CreateMap<CategoryDetails, CategoryDetailsRequest>();
    }

    private void MapperRequestToEntity()
    {
        CreateMap<BaseRequest, BaseEntity>()
            .ForMember(entity => entity.CreatedBy, opt => opt.Ignore())
            .ForMember(entity => entity.CreatedAt, opt => opt.Ignore())
            .ForMember(entity => entity.ModifiedBy, opt => opt.Ignore())
            .ForMember(entity => entity.ModifiedAt, opt => opt.Ignore());

        CreateMap<CampusRequest, Campus>().IncludeBase<BaseRequest, BaseEntity>();

        CreateMap<RoleRequest, Role>().IncludeBase<BaseRequest, BaseEntity>();

        CreateMap<UserRequest, User>().IncludeBase<BaseRequest, BaseEntity>();

        CreateMap<UserCampusRequest, UserCampus>().IncludeBase<BaseRequest, BaseEntity>();

        CreateMap<UserRoleRequest, UserRole>().IncludeBase<BaseRequest, BaseEntity>();

        CreateMap<CategoryRequest, Category>().IncludeBase<BaseRequest, BaseEntity>();

        CreateMap<CategoryDetailsRequest, CategoryDetails>().IncludeBase<BaseRequest, BaseEntity>();
    }
}