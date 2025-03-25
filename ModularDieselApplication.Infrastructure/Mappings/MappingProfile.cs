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
                .ForMember(dest => dest.IDodstavky, opt => opt.MapFrom(src => src.Odstavka.ID))
                .ForMember(dest => dest.IdTechnik, opt => opt.MapFrom(src => src.Technik.ID))
                .ForMember(dest => dest.Odstavka, opt => opt.Ignore())
                .ForMember(dest => dest.Technik, opt => opt.Ignore())
                .ReverseMap();

            // ----------------------------------------
            // Map between Odstavka and TableOdstavky.
            // ----------------------------------------
            CreateMap<Odstavka, TableOdstavky>()
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
            // Map between TableTechnici and Technik.
            // ----------------------------------------
            CreateMap<TableTechnici, Technik>().ReverseMap();

            // ----------------------------------------
            // Map between TableLokality and Lokalita.
            // ----------------------------------------
            CreateMap<TableLokality, Lokalita>().ReverseMap();

            // ----------------------------------------
            // Map between Pohotovosti and TablePohotovosti.
            // ----------------------------------------
            CreateMap<Pohotovosti, TablePohotovosti>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
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
            CreateMap<TableRegiony, Region>().ReverseMap();

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