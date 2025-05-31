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
    //[Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> CreatePublisher([FromBody] CreatePublisherDTO request)
    {
        try
        {
            var platform = await _publisherService.CreatePublisher(request);
            return Ok(platform);
        }
        catch (PublisherDuplicationException e)
        {
            return Conflict(new
            {
                error = "Conflict",
                message = e.Message
            });
        }
    }

    [HttpPut]
   // [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> UpdatePublisher([FromBody] UpdatePublisherDTO request)
    {
        try
        {
            var platform = await _publisherService.UpdatePublisher(request);
            return Ok(platform);
        }
        catch (PublisherNotFoundException e)
        {
            return NotFound(new
            {
                error = "Not Found",
                message = e.Message
            });
        }
        catch (PublisherDuplicationException e) 
        {
            return Conflict(new
            {
                error = "Conflict",
                message = "Publisher with this name already exists."
            });
        }
    }

    [HttpDelete("{id:int}")]
   // [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> DeletePublisher(int id)
    {
        try
        {
            await _publisherService.DeletePublisher(id);
            return Ok("publisher deleted");
        }
        catch (PublisherNotFoundException e)
        {
            return NotFound(new
            {
                error = "Not Found",
                message = e.Message
            });
        }
    }

    [HttpGet]
  //  [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetPaginatedFullPublishers(int page = 1, int limit = 10)
    {
        if (page < 1 || limit < 1)
        {
            return BadRequest(new
            {
                error = "Invalid Pagination",
                message = "Page and limit must be greater than zero."
            });
        }

        var publishers = await _publisherService.GetPaginatedFullPublishers(page, limit);
        return Ok(publishers);
    }

    [HttpGet("specificPublisher")]
    //[Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetFullPublisher(int id)
    {
        try
        {
            var publisher = await _publisherService.GetPublisherByIdWithFullData(id);
            return Ok(publisher);
        }
        catch (NotFoundException e)
        {
            return NotFound(new
            {
                error = "Not Found",
                message = e.Message
            });
        }
    }

    [HttpPatch("addPlatform")]
   // [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> AddPlatform([FromBody] PlatformToPublisherDTO request)
    {
        try
        {
            await _publisherService.AddPlatformToPublisher(request);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(new
            {
                error = "Not Found",
                message = e.Message
            });
        }
        catch (ConflictException e)
        {
            return Conflict(new
            {
                error = "Conflict",
                message = e.Message
            });
        }
    }

    [HttpPatch("removePlatform")]
   // [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> RemovePlatform([FromBody] PlatformToPublisherDTO request)
    {
        try
        {
            await _publisherService.RemovePlatformFromPublisher(request);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(new
            {
                error = "Not Found",
                message = e.Message
            });
        }
        catch (ConflictException e)
        {
            return Conflict(new
            {
                error = "Conflict",
                message = e.Message
            });
        }
    }
}