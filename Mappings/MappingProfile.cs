using AutoMapper;
using FormEngineAPI.Models;
using FormEngineAPI.DTOs;

namespace FormEngineAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<RegisterDto, User>();

        // Form mappings
        CreateMap<Form, FormDto>();
        CreateMap<CreateFormDto, Form>();
        CreateMap<UpdateFormDto, Form>();

        // FormSubmission mappings
        CreateMap<FormSubmission, FormSubmissionDto>()
            .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Form.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));
        CreateMap<CreateSubmissionDto, FormSubmission>();

        // Menu mappings
        CreateMap<Menu, MenuDto>()
            .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children));
        CreateMap<CreateMenuDto, Menu>();
        CreateMap<UpdateMenuDto, Menu>();
    }
}
