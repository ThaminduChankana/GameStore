using GameStore.Api.DTOS;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints //Extentions class which has static extension methods
//Extension class should be static and should have static methods
{
    const string GetGameEndPointName = "GetGameById";

    private static readonly List<GameDTO> games = [
        new (1, "The Witcher 3: Wild Hunt", "RPG", 39.99m, new DateOnly(2015, 5, 19)),
        new (2, "Cyberpunk 2077", "RPG", 59.99m, new DateOnly(2020, 12, 10)),
        new (3, "Red Dead Redemption 2", "Action-Adventure", 49.99m, new DateOnly(2018, 10, 26)),
        new (4, "Hades", "Roguelike", 24.99m, new DateOnly(2020, 9, 17)),
        new (5, "Stardew Valley", "Simulation", 14.99m, new DateOnly(2016, 2, 26))
    ];

    //Extensds the method for the WebApplication class
    //Used RouteGroupBuilder to group multiple endpoints under a common route prefix
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app) //this keyword makes extension class
    {
        var group = app.MapGroup("/games").WithParameterValidation(); //MapGroup() is used to group multiple endpoints under a common route prefix
        //Applying WithParameterValidation() to validate every incoming request for the group

        //GET /games
        group.MapGet("/", () => games);  //MapGet() is used to map a HTTP GET request to a specific route

        //GET /games/{id}
        group.MapGet("/{id}", (int id) =>
        {
            GameDTO? game = games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game);

        })
        .WithName(GetGameEndPointName); //WithName() is used to give a name to a specific route

        //POST /games
        group.MapPost("", (CreateGameDTO newGame) =>
        {
            //EndpointFilter is used to validate the incoming request
            //Nuget is the package manager for .NET

            GameDTO game = new
            (games.Count + 1,
            newGame.Name,
            newGame.Genre,
            newGame.Price,
            newGame.ReleaseDate);

            games.Add(game);

            return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game);
            //Results is a class that is used to return a specific HTTP status code and a value (prebuilt HTTP responses)
        });//using WithParameterValidation() to validate the incoming request. using nuget MinimalApis.Extensions

        //PUT /games/{id}
        group.MapPut("/{id}", (int id, UpdateGameDTO updatedGame) =>
        {
            GameDTO? game = games.Find(game => game.Id == id);

            if (game is null)
            {
                return Results.NotFound();
            }

            GameDTO updatedGameDTO = game with
            {
                Name = updatedGame.Name,
                Genre = updatedGame.Genre,
                Price = updatedGame.Price,
                ReleaseDate = updatedGame.ReleaseDate
            };

            int index = games.FindIndex(game => game.Id == id);
            games[index] = updatedGameDTO;

            return Results.Ok(updatedGameDTO);
        });

        //DELETE /games/{id}
        group.MapDelete("/{id}", (int id) =>
        {
            GameDTO? game = games.Find(game => game.Id == id);

            if (game == null)
            {
                return Results.NotFound();
            }

            games.Remove(game);

            return Results.Ok(game);

        });

        return group;

    }

}
