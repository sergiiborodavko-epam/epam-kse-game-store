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
            var platform = await _publisherService.CreatePublisher(request);
            return Ok(platform);
    }

    [HttpPut]
    //[Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> UpdatePublisher([FromBody] UpdatePublisherDTO request)
    {
            var platform = await _publisherService.UpdatePublisher(request);
            return Ok(platform);
    }

    [HttpDelete("{id:int}")]
   // [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> DeletePublisher(int id)
    {
            await _publisherService.DeletePublisher(id);
            return Ok("publisher deleted");
    }

    [HttpGet]
   // [Authorize(AuthenticationSchemes = "Access")]
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
            var publisher = await _publisherService.GetPublisherByIdWithFullData(id);
            return Ok(publisher);
    }

    [HttpPatch("addPlatform")]
    //[Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> AddPlatform([FromBody] PlatformToPublisherDTO request)
    {
            await _publisherService.AddPlatformToPublisher(request);
            return NoContent();
    }

    [HttpPatch("removePlatform")]
    //[Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> RemovePlatform([FromBody] PlatformToPublisherDTO request)
    {
            await _publisherService.RemovePlatformFromPublisher(request);
            return NoContent();
    }
}