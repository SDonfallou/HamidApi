using AutoMapper;
using bookShareBEnd.Database;

namespace bookShareBEnd.Services
{
    public class LikesServices
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public LikesServices(AppDbContext context,IMapper mapper)
        {
            _context= context;
            _mapper= mapper;
        }


        


    }
}
