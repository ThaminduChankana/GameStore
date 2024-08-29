namespace GameStore.Api.DTOS;
//Records are immutable, meaning that once they are created, their properties cannot be changed.
//DTOs are used to transfer data between software application subsystems.
//Records reduces boilerplate code that that is used for data holding

public record class GameDTO(
    int Id,
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
);

//Record class attributes should start with a capital letter