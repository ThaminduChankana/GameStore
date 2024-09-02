using GameStore.Api.Data;
using GameStore.Api.Endpoints;

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

//var connString = "Data Source=GameStore.db"; //connection string to the database
//builder.Services.AddSqlite<GameStoreContext>(connString); //AddSqlite() is an extension method that is used to add a SQLite database to the application (Dependency Injection)

var connString = builder.Configuration.GetConnectionString("GameStore"); //GetConnectionString() is a method that is used to get the connection string from the configuration file

builder.Services.AddSqlite<GameStoreContext>(connString); //AddSqlite() is an extension method that is used to add a SQLite database to the application (Dependency Injection)

app.MapGamesEndpoints(); //MapGamesEndpoints() is an extension method that is used to map the endpoints of the application

app.Run(); 

// configuration of request pipeline - how to handle requests and responses

//bootstrap application - first lines of code that are executed when the application starts
//use of bootstrap code is to configure the application and start the server


//