using AutoMapper;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Dieslovani, TableDieslovani>()
            .ForMember(dest => dest.IDodstavky, opt => opt.MapFrom(src => src.Odstavka.ID))
            .ForMember(dest => dest.IdTechnik, opt => opt.MapFrom(src => src.Technik.ID))
            .ForMember(dest => dest.Odstavka, opt => opt.Ignore())
            .ForMember(dest => dest.Technik, opt => opt.Ignore())
            .ReverseMap();
            CreateMap<Odstavka, TableOdstavky>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.LokalitaID, opt => opt.MapFrom(src => src.Lokality.ID))
            .ForMember(dest => dest.Lokality, opt => opt.Ignore())
            .ForMember(dest => dest.DieslovaniList, opt => opt.Ignore())
            .ReverseMap();
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