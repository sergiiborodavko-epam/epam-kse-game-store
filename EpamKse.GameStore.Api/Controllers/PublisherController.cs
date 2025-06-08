using EpamKse.GameStore.Domain.DTO.Publisher;
using EpamKse.GameStore.Domain.Exceptions;
using EpamKse.GameStore.Domain.Exceptions.Publisher;
using EpamKse.GameStore.Services.Services.Publisher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[Route("publishers")]
public class PublisherController : ControllerBase
{
    private readonly IPublisherService _publisherService;

    public PublisherController(IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePublisher([FromBody] CreatePublisherDTO request)
    {
            var platform = await _publisherService.CreatePublisher(request);
            return Ok(platform);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePublisher([FromBody] UpdatePublisherDTO request)
    {
            var platform = await _publisherService.UpdatePublisher(request);
            return Ok(platform);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePublisher(int id)
    {
            await _publisherService.DeletePublisher(id);
            return Ok("publisher deleted");
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedFullPublishers(int page = 1, int limit = 10)
    {
        var publishers = await _publisherService.GetPaginatedFullPublishers(page, limit);
        return Ok(publishers);
    }
    
    [HttpGet("specificPublisher")]
    public async Task<IActionResult> GetFullPublisher(int id)
    {
            var publisher = await _publisherService.GetPublisherByIdWithFullData(id);
            return Ok(publisher);
    }

    [HttpPatch("addPlatform")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddPlatform([FromBody] PlatformToPublisherDTO request)
    {
            await _publisherService.AddPlatformToPublisher(request);
            return NoContent();
    }

    [HttpPatch("removePlatform")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemovePlatform([FromBody] PlatformToPublisherDTO request)
    {
            await _publisherService.RemovePlatformFromPublisher(request);
            return NoContent();
    }
}