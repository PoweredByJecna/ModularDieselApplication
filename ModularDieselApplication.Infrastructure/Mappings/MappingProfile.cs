using AutoMapper;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ----------------------------------------
            // Map between Dieslovani and TableDieslovani.
            // ----------------------------------------
            CreateMap<Dieslovani, TableDieslovani>()
                .ForMember(dest => dest.IdOdstavky, opt => opt.MapFrom(src => src.Odstavka.ID))
                .ForMember(dest => dest.IdTechnik, opt => opt.MapFrom(src => src.Technik.ID))
                .ForMember(dest => dest.Odstavka, opt => opt.Ignore())
                .ForMember(dest => dest.Technik, opt => opt.Ignore())
                .ReverseMap();

            // ----------------------------------------
            // Map between Odstavka and TableOdstavky.
            // ----------------------------------------
            CreateMap<Odstavka, TableOdstavka>()
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForMember(dest => dest.LokalitaID, opt => opt.MapFrom(src => src.Lokality.ID))
                .ForMember(dest => dest.Lokality, opt => opt.Ignore())
                .ForMember(dest => dest.DieslovaniList, opt => opt.Ignore())
                .ReverseMap();

            // ----------------------------------------
            // Map between TableFirma and Firma.
            // ----------------------------------------
            CreateMap<TableFirma, Firma>().ReverseMap();

            // ----------------------------------------
            // Map between TableTechnik and Technik.
            // ----------------------------------------
            CreateMap<TableTechnik, Technik>().ReverseMap();

            // ----------------------------------------
            // Map between TableLokality and Lokalita.
            // ----------------------------------------
            CreateMap<TableLokalita, Lokalita>().ReverseMap();

            // ----------------------------------------
            // Map between Pohotovosti and TablePohotovosti.
            // ----------------------------------------
            CreateMap<Pohotovosti, TablePohotovost>()
                .ForMember(dest => dest.Technik, opt => opt.Ignore())
                .ForMember(dest => dest.IdTechnik, opt => opt.MapFrom(src => src.IdTechnik))
                .ReverseMap();

            // ----------------------------------------
            // Map between DebugLogModel and Log.
            // ----------------------------------------
            CreateMap<DebugLogModel, Log>().ReverseMap();

            // ----------------------------------------
            // Map between TableRegiony and Region.
            // ----------------------------------------
            CreateMap<TableRegion, Region>().ReverseMap();

            // ----------------------------------------
            // Map between TableUser and User.
            // ----------------------------------------
            CreateMap<TableUser, User>().ReverseMap();

            // ----------------------------------------
            // Map between TableZdroj and Zdroj.
            // ----------------------------------------
            CreateMap<TableZdroj, Zdroj>().ReverseMap();
        }
    }
}