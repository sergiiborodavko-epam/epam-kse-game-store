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
    public async Task GetAllGenres_ReturnsOkWithGenreHierarchy() {
        var genres = new List<Genre> {
            new() { Id = 1, Name = "Strategy", ParentGenreId = null, SubGenres = new List<Genre>() },
            new() { Id = 2, Name = "RTS", ParentGenreId = 1, ParentGenre = new Genre { Name = "Strategy" } },
            new() { Id = 3, Name = "Action", ParentGenreId = null, SubGenres = new List<Genre>() }
        };
        _mockService.Setup(s => s.GetAllGenresAsync()).ReturnsAsync(genres);

        var result = await _controller.GetAllGenres();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGenres = Assert.IsType<IEnumerable<Genre>>(okResult.Value, exactMatch: false);
        Assert.Equal(3, returnedGenres.Count());
    }

    [Fact]
    public async Task GetGenreById_ReturnsGenreWithRelations() {
        var genre = new Genre { 
            Id = 2, 
            Name = "RTS", 
            ParentGenreId = 1,
            ParentGenre = new Genre { Id = 1, Name = "Strategy" }
        };
        _mockService.Setup(s => s.GetGenreByIdAsync(2)).ReturnsAsync(genre);

        var result = await _controller.GetGenreById(2);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGenre = Assert.IsType<Genre>(okResult.Value);
        Assert.Equal("RTS", returnedGenre.Name);
        Assert.Equal(1, returnedGenre.ParentGenreId);
    }

    [Fact]
    public async Task CreateGenre_MainGenre_ReturnsCreated() {
        var genreDto = new GenreDto { Name = "RPG", ParentGenreName = null };
        var createdGenre = new Genre { Id = 4, Name = "RPG", ParentGenreId = null };
        _mockService.Setup(s => s.CreateGenreAsync(genreDto)).ReturnsAsync(createdGenre);

        var result = await _controller.CreateGenre(genreDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedGenre = Assert.IsType<Genre>(createdResult.Value);
        Assert.Equal("RPG", returnedGenre.Name);
        Assert.Null(returnedGenre.ParentGenreId);
    }

    [Fact]
    public async Task CreateGenre_SubGenre_ReturnsCreated() {
        var genreDto = new GenreDto { Name = "TBS", ParentGenreName = "Strategy" };
        var createdGenre = new Genre { Id = 5, Name = "TBS", ParentGenreId = 1 };
        _mockService.Setup(s => s.CreateGenreAsync(genreDto)).ReturnsAsync(createdGenre);

        var result = await _controller.CreateGenre(genreDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedGenre = Assert.IsType<Genre>(createdResult.Value);
        Assert.Equal("TBS", returnedGenre.Name);
        Assert.Equal(1, returnedGenre.ParentGenreId);
    }

    [Fact]
    public async Task UpdateGenre_ValidData_ReturnsUpdated() {
        var genreDto = new GenreDto { Name = "Updated Strategy", ParentGenreName = null };
        var updatedGenre = new Genre { Id = 1, Name = "Updated Strategy", ParentGenreId = null };
        _mockService.Setup(s => s.UpdateGenreAsync(1, genreDto)).ReturnsAsync(updatedGenre);

        var result = await _controller.UpdateGenre(1, genreDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGenre = Assert.IsType<Genre>(okResult.Value);
        Assert.Equal("Updated Strategy", returnedGenre.Name);
    }

    [Fact]
    public async Task DeleteGenre_ValidId_ReturnsNoContent() {
        _mockService.Setup(s => s.DeleteGenreAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteGenre(1);

        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.DeleteGenreAsync(1), Times.Once);
    }
}
