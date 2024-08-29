namespace GameStore.Api.DTOS;

public record class UpdateGameDTO
(
    int Id,
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
);
