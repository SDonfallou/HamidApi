using AutoMapper;
using bookShareBEnd.Database.DTO;
using bookShareBEnd.Database.Model;

namespace bookShareBEnd.AutoMapper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Users, UserDTO>().ReverseMap();
            CreateMap<Books, BookDTO>().ReverseMap();
            CreateMap<Roles,RolesDTO>().ReverseMap();

        }
    }
}
