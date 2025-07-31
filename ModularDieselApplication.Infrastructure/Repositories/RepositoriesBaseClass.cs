
using ModularDieselApplication.Infrastructure.Persistence;
using AutoMapper;
public class RepositoriesBaseClass
{
    protected readonly ApplicationDbContext _context;
    protected readonly IMapper _mapper;

    public RepositoriesBaseClass(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
}