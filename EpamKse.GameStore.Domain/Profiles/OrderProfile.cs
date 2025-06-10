using AutoMapper;
using EpamKse.GameStore.Domain.DTO.Game;
using EpamKse.GameStore.Domain.DTO.Order;
using EpamKse.GameStore.Domain.Entities;

namespace EpamKse.GameStore.Domain.Profiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
    }
}