namespace EpamKse.GameStore.Api.Tests.Controllers;

using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

using Api.Controllers;
using Domain.DTO.Genre;
using Domain.Entities;
using Services.Services.Genre;

public class GenresControllerTests {
    private readonly Mock<IGenreService> _mockService;
    private readonly GenresController _controller;

    public GenresControllerTests() {
        _mockService = new Mock<IGenreService>();
        _controller = new GenresController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllGenres_ReturnsOkResult_WithListOfGenres() {
        var genres = new List<Genre> {
            new() { Id = 1, Name = "Strategy" },
            new() { Id = 2, Name = "RPG" }
        };
        _mockService.Setup(service => service.GetAllGenresAsync())
            .ReturnsAsync(genres);

        var result = await _controller.GetAllGenres();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGenres = Assert.IsType<IEnumerable<Genre>>(okResult.Value, exactMatch: false);
        Assert.Equal(2, returnedGenres.Count());
    }

    [Fact]
    public async Task GetGenreById_ReturnsOkResult_WithGenre_WhenGenreExists() {
        const int genreId = 1;
        var genre = new Genre { Id = genreId, Name = "Strategy" };
        _mockService.Setup(service => service.GetGenreByIdAsync(genreId))
            .ReturnsAsync(genre);

        var result = await _controller.GetGenreById(genreId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGenre = Assert.IsType<Genre>(okResult.Value);
        Assert.Equal(genreId, returnedGenre.Id);
    }

    [Fact]
    public async Task CreateGenre_ReturnsCreatedAtAction_WithNewGenre() {
        var genreDto = new GenreDto {
            Name = "Action"
        };
        var createdGenre = new Genre {
            Id = 3,
            Name = genreDto.Name
        };
        _mockService.Setup(service => service.CreateGenreAsync(genreDto))
            .ReturnsAsync(createdGenre);

        var result = await _controller.CreateGenre(genreDto);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedGenre = Assert.IsType<Genre>(createdAtActionResult.Value);
        Assert.Equal(createdGenre.Id, returnedGenre.Id);
        Assert.Equal(genreDto.Name, returnedGenre.Name);
    }

    [Fact]
    public async Task UpdateGenre_ReturnsOkResult_WithUpdatedGenre_WhenGenreExists() {
        const int genreId = 1;
        var genreDto = new GenreDto {
            Name = "Updated Strategy"
        };
        var updatedGenre = new Genre {
            Id = genreId,
            Name = genreDto.Name
        };
        _mockService.Setup(service => service.UpdateGenreAsync(genreId, genreDto))
            .ReturnsAsync(updatedGenre);

        var result = await _controller.UpdateGenre(genreId, genreDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGenre = Assert.IsType<Genre>(okResult.Value);
        Assert.Equal(genreId, returnedGenre.Id);
        Assert.Equal(genreDto.Name, returnedGenre.Name);
    }

    [Fact]
    public async Task DeleteGenre_ReturnsNoContent_WhenGenreExists() {
        const int genreId = 1;
        _mockService.Setup(service => service.DeleteGenreAsync(genreId))
            .Returns(Task.CompletedTask);

        var result = await _controller.DeleteGenre(genreId);

        Assert.IsType<NoContentResult>(result);
    }
}
