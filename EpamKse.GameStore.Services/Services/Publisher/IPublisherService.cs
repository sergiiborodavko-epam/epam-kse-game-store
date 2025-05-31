using EpamKse.GameStore.Domain.DTO.Publisher;

namespace EpamKse.GameStore.Services.Services.Publisher;
using Domain.Entities;


public interface IPublisherService
{
    Task<List<PublisherDTO>> GetPaginatedFullPublishers(int page, int limit);
    Task<PublisherDTO> GetPublisherByIdWithFullData(int id);
    Task<Publisher> CreatePublisher(CreatePublisherDTO publisherDto);
    Task<Publisher> UpdatePublisher(UpdatePublisherDTO publisherDto);
    Task DeletePublisher(int id);

    Task AddPlatformToPublisher(PlatformToPublisherDTO platformToPublisherDto);
    Task RemovePlatformFromPublisher(PlatformToPublisherDTO platformToPublisherDto);
}