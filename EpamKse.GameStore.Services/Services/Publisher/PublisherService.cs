using EpamKse.GameStore.DataAccess.Migrations;
using EpamKse.GameStore.Domain.DTO.Publisher;
using EpamKse.GameStore.Domain.Exceptions;
using EpamKse.GameStore.Domain.Exceptions.Platform;
using EpamKse.GameStore.Domain.Exceptions.Publisher;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EpamKse.GameStore.Services.Services.Publisher;

using Domain.Entities;
using EpamKse.GameStore.DataAccess.Context;

public class PublisherService : IPublisherService
{
    private readonly GameStoreDbContext _dbContext;

    public PublisherService(GameStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Publisher> CreatePublisher(CreatePublisherDTO publisherDto)
    {
        if (await _dbContext.Publishers.FirstOrDefaultAsync(publisher =>
                publisher.Name.ToLower() == publisherDto.Name.ToLower()) is not null)
        {
            throw new PublisherDuplicationException(publisherDto.Name);
        }

        var publisher = new Publisher
        {
            Name = publisherDto.Name,
            Description = publisherDto.Description,
            HomePageUrl = publisherDto.HomePageUrl
        };

        _dbContext.Publishers.Add(publisher);
        await _dbContext.SaveChangesAsync();
        return publisher;
    }

    public async Task<Publisher> UpdatePublisher(UpdatePublisherDTO updatePublisherDto)
    {
        var publisherToUpdate =
            await _dbContext.Publishers.FirstOrDefaultAsync(publisher => publisher.Id == updatePublisherDto.Id);
        if (publisherToUpdate is null)
        {
            throw new PublisherNotFoundException(updatePublisherDto.Id);
        }

        publisherToUpdate.Name = updatePublisherDto.Name;
        publisherToUpdate.Description = updatePublisherDto.Description;
        publisherToUpdate.HomePageUrl = updatePublisherDto.HomePageUrl;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
                                           sqlEx.Message.Contains("IX_Publishers_Name"))
        {
            throw new PublisherDuplicationException(updatePublisherDto.Name);
        }

        return publisherToUpdate;
    }

    public async Task DeletePublisher(int publisherId)
    {
        var publisher = await _dbContext.Publishers.FindAsync(publisherId);
        if (publisher != null)
        {
            _dbContext.Publishers.Remove(publisher);
            await _dbContext.SaveChangesAsync();
            return;
        }

        throw new PublisherNotFoundException(publisherId);
    }

    public async Task<List<PublisherDTO>> GetPaginatedFullPublishers(int page = 1, int limit = 10)
    {
        var skip = (page - 1) * limit;

        var publishers = await _dbContext.Publishers
            .Include(p => p.Games)
            .ThenInclude(g => g.Platforms)
            .Include(p => p.PublisherPlatforms)
            .OrderBy(p => p.Id)
            .Skip(skip)
            .Take(limit)
            .ToListAsync();

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
        var publisher = await _dbContext.Publishers
            .Include(p => p.Games)
            .ThenInclude(g => g.Platforms)
            .Include(p => p.PublisherPlatforms)
            .FirstOrDefaultAsync(p => p.Id == id);

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
        var publisher = await _dbContext.Publishers
            .Include(p => p.PublisherPlatforms)
            .FirstOrDefaultAsync(p => p.Id == dto.Id);

        if (publisher == null)
        {
            throw new PublisherNotFoundException(dto.Id);
        }

        var platform = await _dbContext.Platforms.FindAsync(dto.platformId);
        if (platform == null)
        {
            throw new PlatformNotFoundException(dto.platformId);
        }

        if (publisher.PublisherPlatforms.Any(p => p.Id == dto.platformId))
        {
            throw new ConflictException($"Platform with ID {dto.platformId} is already linked to this publisher.");
        }

        publisher.PublisherPlatforms.Add(platform);
        await _dbContext.SaveChangesAsync();
    }


    public async Task RemovePlatformFromPublisher(PlatformToPublisherDTO dto)
    {
        var publisher = await _dbContext.Publishers
            .Include(p => p.PublisherPlatforms)
            .FirstOrDefaultAsync(p => p.Id == dto.Id);

        if (publisher == null)
        {
            throw new PublisherNotFoundException(dto.Id);
        }

        var platform = await _dbContext.Platforms.FindAsync(dto.platformId);
        if (platform == null)
        {
            throw new PlatformNotFoundException(dto.platformId);
        }

        if (!publisher.PublisherPlatforms.Any(p => p.Id == dto.platformId))
        {
            throw new ConflictException($"Platform with ID {dto.platformId} is not linked to this publisher.");
        }

        publisher.PublisherPlatforms.Remove(platform);
        await _dbContext.SaveChangesAsync();
    }
}