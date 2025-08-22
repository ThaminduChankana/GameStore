// Using directives - Import necessary namespaces for Entity Framework functionality
using GameStore.Api.Entities;      // Imports our custom entity classes (Game, Genre, etc.)
                                   // These represent the domain objects that map to database tables
using Microsoft.EntityFrameworkCore; // Imports Entity Framework Core framework classes
                                     // Provides DbContext, DbSet<T>, and other EF Core functionality

// Namespace declaration - Organizes our data access layer classes
namespace GameStore.Api.Data;
// This namespace follows the convention of ProjectName.Layer.Sublayer
// Data layer contains all database-related classes (DbContext, repositories, configurations)

// GameStoreContext Class Declaration with Primary Constructor (C# 12 feature)
public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
    // Class Declaration Breakdown:
    // - 'public': Makes this class accessible from other assemblies/projects
    // - 'class GameStoreContext': The class name, following DbContext naming convention (ContextName + "Context")
    // - Primary Constructor: (DbContextOptions<GameStoreContext> options) - New C# 12 syntax
    //   Traditional syntax would be: public GameStoreContext(DbContextOptions<GameStoreContext> options) { }
    // - Inheritance: ': DbContext(options)' - inherits from Entity Framework's DbContext base class
    //   and passes the options parameter to the base constructor

    /* 
    DbContext Explanation:
    DbContext is the primary class responsible for interacting with the database in Entity Framework Core.
    It serves as a bridge between your .NET objects and the database.
    
    Key responsibilities of DbContext:
    1. **Connection Management**: Manages database connections (opening, closing, pooling)
    2. **Change Tracking**: Monitors changes to entities for automatic updates
    3. **Query Translation**: Converts LINQ queries to SQL queries
    4. **Transaction Management**: Handles database transactions and rollbacks
    5. **Caching**: Provides first-level caching of entities during the context lifetime
    6. **Schema Generation**: Can create database schema from entity models
    7. **Data Access**: Provides methods for CRUD operations (Create, Read, Update, Delete)
    
    DbContext Lifecycle:
    - Created: When instantiated (usually through dependency injection)
    - Used: For querying and saving data during request processing
    - Disposed: At the end of request scope, releasing database connections
    
    Unit of Work Pattern:
    DbContext implements the Unit of Work pattern where:
    - All changes are tracked in memory
    - SaveChanges() commits all changes in a single transaction
    - This ensures data consistency and performance optimization
    */

    /*
    DbContextOptions<GameStoreContext> Parameter:
    This parameter contains configuration information for the DbContext:
    
    What DbContextOptions provides:
    1. **Database Provider**: Specifies which database system to use (SQLite, SQL Server, PostgreSQL, etc.)
    2. **Connection String**: Database connection details (server, database name, credentials)
    3. **Logging Configuration**: How to log SQL queries and EF operations
    4. **Performance Settings**: Query tracking behavior, timeout settings
    5. **Development Features**: Sensitive data logging, detailed error messages
    6. **Model Configuration**: How entities map to database schema
    
    Why use generic DbContextOptions<GameStoreContext>:
    - Type safety: Ensures options are specifically for this context type
    - Dependency Injection: Allows DI container to provide correct configuration
    - Multiple Contexts: Enables different configurations for different DbContext types
    - Testing: Easy to provide mock or test-specific options
    
    Typical configuration (done in Program.cs):
    builder.Services.AddDbContext<GameStoreContext>(options =>
        options.UseSqlite(connectionString)
               .EnableSensitiveDataLogging()
               .LogTo(Console.WriteLine));
    */

    // DbSet Properties - Collections representing database tables
    
    // Games Table Representation
    public DbSet<Game> Games => Set<Game>();
    /*
    DbSet<Game> Explanation:
    DbSet<T> is a generic collection that represents a table in the database.
    It provides LINQ-queryable interface for database operations.
    
    What DbSet<Game> enables:
    1. **Querying**: LINQ queries are translated to SQL
       Example: Games.Where(g => g.Price < 20) becomes SELECT * FROM Games WHERE Price < 20
    
    2. **Adding Entities**: Games.Add(newGame) stages entity for insertion
    
    3. **Updating Entities**: EF tracks changes to loaded entities automatically
       Example: game.Price = 15; context.SaveChanges(); updates the database
    
    4. **Deleting Entities**: Games.Remove(game) stages entity for deletion
    
    5. **Lazy Loading**: Related entities can be loaded on-demand
       Example: game.Genre loads automatically when accessed (if configured)
    
    Set<Game>() Method:
    - Returns a DbSet<Game> for the Game entity type
    - This is the modern approach (alternative to declaring backing fields)
    - EF Core automatically creates and manages the DbSet instance
    - Provides better performance and memory usage than property backing fields
    
    Property Expression Syntax:
    'Games => Set<Game>()' is an expression-bodied property (C# 7+)
    Equivalent to:
    public DbSet<Game> Games 
    { 
        get { return Set<Game>(); } 
    }
    
    LINQ to SQL Translation Examples:
    - Games.ToList() → SELECT * FROM Games
    - Games.FirstOrDefault(g => g.Id == 1) → SELECT TOP 1 * FROM Games WHERE Id = 1
    - Games.Where(g => g.Price > 10).OrderBy(g => g.Name) → 
      SELECT * FROM Games WHERE Price > 10 ORDER BY Name
    - Games.Include(g => g.Genre) → SELECT * FROM Games g LEFT JOIN Genres gen ON g.GenreId = gen.Id
    */
    
    // Genres Table Representation  
    public DbSet<Genre> Genres => Set<Genre>();
    /*
    DbSet<Genre> Explanation:
    Similar to Games DbSet, but represents the Genres table.
    
    Entity Relationships:
    If Game has a GenreId foreign key and Genre navigation property:
    - EF Core automatically detects the relationship
    - LINQ queries can navigate: Games.Include(g => g.Genre)
    - Changes to Genre entities are tracked automatically
    
    Common Genre Operations:
    1. **Seeding Data**: Genres.AddRange(predefinedGenres) for initial data
    2. **Lookup Operations**: Genres.FirstOrDefault(g => g.Name == "Action")
    3. **Reference Data**: Genres are often used as lookup/reference tables
    4. **Cascade Operations**: Deleting a Genre might affect related Games (depends on configuration)
    
    Performance Considerations:
    - Genres are typically small reference tables - good candidates for caching
    - Use .AsNoTracking() for read-only scenarios to improve performance
    - Consider eager loading with .Include() for frequently accessed relationships
    */
}

/*
Entity Framework Core Architecture Overview:

1. **Entities (Domain Models)**:
   - Game, Genre classes represent business objects
   - Map to database tables through conventions or explicit configuration
   - Can contain navigation properties for relationships

2. **DbContext (Data Access Layer)**:
   - GameStoreContext provides database access
   - Manages entity lifecycle and change tracking
   - Translates LINQ queries to SQL

3. **DbSet Collections**:
   - Games, Genres provide typed access to tables
   - Support LINQ querying and CRUD operations
   - Handle automatic change tracking

4. **Configuration Sources**:
   - Conventions: EF uses naming patterns to infer mappings
   - Data Annotations: Attributes on entity properties ([Key], [Required], etc.)
   - Fluent API: Explicit configuration in OnModelCreating method

5. **Query Execution Process**:
   a. LINQ query written against DbSet
   b. EF builds expression tree
   c. Query provider translates to SQL
   d. Database executes SQL
   e. Results materialized back to entities
   f. Change tracking enabled for returned entities

6. **Change Tracking Workflow**:
   a. Entities loaded from database are tracked
   b. Property changes are detected automatically
   c. SaveChanges() generates appropriate SQL (INSERT/UPDATE/DELETE)
   d. All changes executed in single transaction
   e. Database-generated values (like IDs) propagated back to entities

Design Patterns Implemented:
- **Repository Pattern**: DbSet<T> acts as repository for each entity type
- **Unit of Work**: DbContext coordinates changes across multiple repositories
- **Active Record**: Entities can be directly saved through DbContext
- **Identity Map**: DbContext maintains identity map to prevent duplicate entity instances
- **Lazy Loading**: Related entities loaded on-demand (if enabled)

Benefits of This Architecture:
1. **Separation of Concerns**: Data access logic isolated in Data namespace
2. **Testability**: DbContext can be mocked or use in-memory database for testing
3. **Performance**: EF optimizes queries and provides change tracking
4. **Maintainability**: Entity relationships managed automatically
5. **Database Agnostic**: Can switch database providers without changing business logic
6. **LINQ Integration**: Strongly-typed queries with IntelliSense support
7. **Migration Support**: Schema changes managed through code-first migrations
*/