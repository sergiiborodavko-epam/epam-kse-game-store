using AutoMapper;
using EpamKse.GameStore.Domain.DTO.Genre;
using EpamKse.GameStore.Domain.Entities;

namespace EpamKse.GameStore.Domain.Profiles;

public class GenreProfile : Profile
{
    public GenreProfile()
    {
        CreateMap<Genre, GenreViewDto>();
    }
}