using AutoMapper;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TableDieslovani, Dieslovani>().ReverseMap();
            CreateMap<TableOdstavky, Odstavka>().ReverseMap();
            CreateMap<TableFirma, Firma>().ReverseMap();
            CreateMap<TableTechnici, Technik>().ReverseMap();
            CreateMap<TableLokality, Lokalita>().ReverseMap();
            CreateMap<TablePohotovosti, Pohotovosti>().ReverseMap();
            CreateMap<DebugLogModel, Log>().ReverseMap();
            CreateMap<TableRegiony, Region>().ReverseMap();
            CreateMap<TableUser, User>().ReverseMap();
            CreateMap<TableZdroj, Zdroj>().ReverseMap();
        }
    }
}