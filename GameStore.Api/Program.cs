using GameStore.Api.DTOS;

var builder = WebApplication.CreateBuilder(args); //create a new instance of the WebApplication class
//WebApplication is a class that is used to configure and run a web application web application is known as host of the application

//Purpose of the host is to introduce a http server implementation and start listening for incoming requests
//Sums up some mutable components (login services, configuration, DI etc) and some immutable components (routing, middleware, etc)

//builder is an instance of the WebApplicationBuilder class and is used to configure the application builder is use to introduce services, middleware, and other components to the application

//var app = builder.Build(); //Build() builds the instance of the web application                //
//sets up Kestrel server as in process web server                                              //
                                                                                               // Configurartion of request Pipeline
//app.MapGet("/", () => "Hello World!");                                                         //
                                                                                               //
//app.Run();                                                                                     //

var app = builder.Build(); 

const string GetGameEndPointName = "GetGameById";

List<GameDTO> games = [
    new (1, "The Witcher 3: Wild Hunt", "RPG", 39.99m, new DateOnly(2015, 5, 19)),
    new (2, "Cyberpunk 2077", "RPG", 59.99m, new DateOnly(2020, 12, 10)),
    new (3, "Red Dead Redemption 2", "Action-Adventure", 49.99m, new DateOnly(2018, 10, 26)),
    new (4, "Hades", "Roguelike", 24.99m, new DateOnly(2020, 9, 17)),
    new (5, "Stardew Valley", "Simulation", 14.99m, new DateOnly(2016, 2, 26))
];

//GET /games
app.MapGet("games", () => games);  //MapGet() is used to map a HTTP GET request to a specific route

//GET /games/{id}
app.MapGet("games/{id}", (int id) => games.Find(game=> game.Id == id))
.WithName(GetGameEndPointName); //WithName() is used to give a name to a specific route

//POST /games
app.MapPost("games", (CreateGameDTO newGame) => 
{
    GameDTO game = new 
    (games.Count + 1, 
    newGame.Name, 
    newGame.Genre, 
    newGame.Price, 
    newGame.ReleaseDate);

    games.Add(game);

    return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game); 
    //Results is a class that is used to return a specific HTTP status code and a value (prebuilt HTTP responses)
});

//PUT /games/{id}
app.MapPut("games/{id}", (int id, UpdateGameDTO updatedGame) => 
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
app.MapDelete("games/{id}", (int id) => 
{
    GameDTO? game = games.Find(game => game.Id == id);

    if(game == null){
        return Results.NotFound();
    }

    games.Remove(game);

    return Results.Ok(game);

});

app.Run(); 

// configuration of request pipeline - how to handle requests and responses

//bootstrap application - first lines of code that are executed when the application starts
//use of bootstrap code is to configure the application and start the server
