using AutoMapper;
using EpamKse.GameStore.Domain.DTO.Game;
using EpamKse.GameStore.Domain.Entities;

namespace EpamKse.GameStore.Domain.Profiles;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<Game, GameViewDto>();
    }
}