using EpamKse.GameStore.DataAccess.Repositories.Platform;
using EpamKse.GameStore.DataAccess.Repositories.Publisher;
using EpamKse.GameStore.Domain.DTO.Publisher;
using EpamKse.GameStore.Domain.Exceptions.Platform;
using EpamKse.GameStore.Domain.Exceptions.Publisher;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EpamKse.GameStore.Services.Services.Publisher;

using Domain.Entities;

public class PublisherService : IPublisherService
{
    private readonly IPublisherRepository _publisherRepository;
    private readonly IPlatformRepository _platformRepository;
    public PublisherService(IPublisherRepository publisherRepository, IPlatformRepository platformRepository)
    {
        _platformRepository = platformRepository;
        _publisherRepository = publisherRepository;
    }

    public async Task<Publisher> CreatePublisher(CreatePublisherDTO publisherDto)
    {
        if (await _publisherRepository.GetByNameAsync(publisherDto.Name) is not null)
        {
            throw new PublisherDuplicationException(publisherDto.Name);
        }

        var publisher = new Publisher
        {
            Name = publisherDto.Name,
            Description = publisherDto.Description,
            HomePageUrl = publisherDto.HomePageUrl
        };

        return await _publisherRepository.AddAsync(publisher);
    }

    public async Task<Publisher> UpdatePublisher(UpdatePublisherDTO updatePublisherDto)
    {
        var publisherToUpdate = await _publisherRepository.GetByIdAsync(updatePublisherDto.Id);
        if (publisherToUpdate is null)
        {
            throw new PublisherNotFoundException(updatePublisherDto.Id);
        }

        publisherToUpdate.Name = updatePublisherDto.Name;
        publisherToUpdate.Description = updatePublisherDto.Description;
        publisherToUpdate.HomePageUrl = updatePublisherDto.HomePageUrl;

        try
        {
            return await _publisherRepository.UpdateAsync(publisherToUpdate);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
                                           sqlEx.Message.Contains("IX_Publishers_Name"))
        {
            throw new PublisherDuplicationException(updatePublisherDto.Name);
        }
    }

    public async Task DeletePublisher(int publisherId)
    {
        var publisher = await _publisherRepository.GetByIdAsync(publisherId);
        if (publisher != null)
        {
            await _publisherRepository.RemoveAsync(publisher);
            return;
        }

        throw new PublisherNotFoundException(publisherId);
    }

    public async Task<List<PublisherDTO>> GetPaginatedFullPublishers(int page = 1, int limit = 10)
    {
        var skip = (page - 1) * limit;
        var publishers = await _publisherRepository.GetPaginatedFullAsync(skip, limit);

        return publishers.Select(p => new PublisherDTO
        {
            Id = p.Id,
            Name = p.Name,
            HomePageUrl = p.HomePageUrl,
            Description = p.Description,
            CreatedAt = p.CreatedAt,
            Games = p.Games.Select(g => new GameDTO
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Price = g.Price,
                ReleaseDate = g.ReleaseDate,
                Platforms = g.Platforms.Select(pl => new PlatformDTO
                {
                    Id = pl.Id,
                    Name = pl.Name
                }).ToList()
            }).ToList(),
            Platforms = p.PublisherPlatforms.Select(pl => new PlatformDTO
            {
                Id = pl.Id,
                Name = pl.Name
            }).ToList()
        }).ToList();
    }

    public async Task<PublisherDTO?> GetPublisherByIdWithFullData(int id)
    {
        var publisher = await _publisherRepository.GetByIdAsync(id);

        if (publisher == null) throw new PublisherNotFoundException(id);

        return new PublisherDTO
        {
            Id = publisher.Id,
            Name = publisher.Name,
            HomePageUrl = publisher.HomePageUrl,
            Description = publisher.Description,
            CreatedAt = publisher.CreatedAt,
            Games = publisher.Games.Select(g => new GameDTO
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Price = g.Price,
                ReleaseDate = g.ReleaseDate,
                Platforms = g.Platforms.Select(pl => new PlatformDTO
                {
                    Id = pl.Id,
                    Name = pl.Name
                }).ToList()
            }).ToList(),
            Platforms = publisher.PublisherPlatforms.Select(pl => new PlatformDTO
            {
                Id = pl.Id,
                Name = pl.Name
            }).ToList()
        };
    }

    public async Task AddPlatformToPublisher(PlatformToPublisherDTO dto)
    {
        var publisher = await _publisherRepository.GetByIdAsync(dto.Id);
        if (publisher is null)
        {
            throw new PublisherNotFoundException(dto.Id);
        }

        var platform = await _platformRepository.GetByIdAsync(dto.platformId);
        if (platform is null)
        {
            throw new PlatformNotFoundException(dto.platformId);
        }

        if (await _platformRepository.IsPlatformLinkedToPublisherAsync(dto.Id, dto.platformId))
        {
            throw new PlatformAlreadyLinkedException(dto.platformId);
        }

        await _platformRepository.AddPlatformToPublisherAsync(publisher, platform);
    }


    public async Task RemovePlatformFromPublisher(PlatformToPublisherDTO dto)
    {
        var publisher = await _publisherRepository.GetByIdAsync(dto.Id);
        if (publisher is null)
        {
            throw new PublisherNotFoundException(dto.Id);
        }

        var platform = await _platformRepository.GetByIdAsync(dto.platformId);
        if (platform is null)
        {
            throw new PlatformNotFoundException(dto.platformId);
        }

        if (!await _platformRepository.IsPlatformLinkedToPublisherAsync(dto.Id, dto.platformId))
        {
            throw new PlatformIsNotLinkedException(dto.platformId);
        }

        await _platformRepository.RemovePlatformFromPublisherAsync(publisher, platform);
    }
}