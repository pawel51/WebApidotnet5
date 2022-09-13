using AutoMapper;
using Entities.DTO.InDto;
using Entities.DTO.OutDto;
using Entities.Models;

namespace WebApidotnet5.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
             .ForMember(c => c.FullAddress,
             opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));

            CreateMap<Employee, EmployeeDto>();
            CreateMap<AddCompanyDto, Company>();
            CreateMap<AddEmployeeDto, Employee>();
            CreateMap<EditEmployeeDto, Employee>().ReverseMap();
            CreateMap<EditCompanyDto, Company>();
            CreateMap<UserForRegistrationDto, User>();

        }
    }
}
