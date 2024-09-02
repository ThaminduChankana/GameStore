using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public class GameStoreContext(DbContextOptions <GameStoreContext> options) : DbContext(options) //DbContext is an object which represents a relation with a database and can be used to query and save data to a database

//Should receive DbContextOptions<GameStoreContext> options as a parameter
{
    public DbSet<Game> Games => Set<Game>(); //Object that can be used to query and save instances of Game entity. Any Link queries against the Games collection will be translated into queries against the games table in the database

    public DbSet<Genre> Genres => Set<Genre>(); //Object that can be used to query and save instances of Genre entity. Any Link queries against the Genres collection will be translated into queries against the genres table in the database
}


