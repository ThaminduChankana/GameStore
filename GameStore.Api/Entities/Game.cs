// Using directive for fundamental .NET types
using System;
// Provides access to basic system types and functionality
// Required for DateOnly type and other fundamental .NET types
// Even with ImplicitUsings enabled, explicit using can be added for clarity

// Namespace for domain entities
namespace GameStore.Api.Entities;
// Entities namespace contains domain model classes that represent business objects
// These classes map directly to database tables in Entity Framework Core
// Follows Domain-Driven Design (DDD) principles for organizing business logic
// Separates entities from DTOs to maintain clean architecture boundaries

/*
Entity Class Design Philosophy:

Entities represent the core business objects in your domain model.
They are different from DTOs in several key ways:

1. **Business Logic**: Entities can contain business rules and behaviors
2. **Persistence**: Designed to be stored in and retrieved from databases
3. **Relationships**: Define associations with other entities
4. **Identity**: Have persistent identity that survives across sessions
5. **Mutable**: Properties can change during business operations
6. **Validation**: May include domain validation rules

Entity vs DTO Comparison:
- Entity: Internal business model, can change based on business needs
- DTO: External contract, should be stable for API consumers
- Entity: May have complex relationships and navigation properties
- DTO: Flattened structure optimized for data transfer
- Entity: Reflects database schema and business rules
- DTO: Reflects API requirements and client needs
*/

// Game entity class representing a video game in the system
public class Game
{
    /*
    Class Declaration Analysis:
    - 'public': Accessible from other assemblies (like API layer)
    - 'class': Reference type, supports inheritance and polymorphism
    - Traditional class syntax (not record) because entities are typically mutable
    
    Why Class Instead of Record:
    1. **Mutability**: Game properties change during business operations
    2. **Entity Framework**: EF Core works better with traditional classes
    3. **Business Logic**: Classes can contain methods and complex behaviors
    4. **Change Tracking**: EF change tracking designed for mutable objects
    5. **Performance**: Classes have better performance for frequent updates
    
    Entity Framework Mapping:
    By convention, this class will map to a "Games" table in the database
    - Class name "Game" → Table name "Games" (pluralized)
    - Properties → Table columns
    - Id property → Primary key (auto-increment)
    - Navigation properties → Foreign key relationships
    */
    
    // Primary key property
    public int Id { get; set; }
    /*
    Primary Key Design:
    
    Entity Framework Conventions:
    - Property named "Id" or "{ClassName}Id" automatically becomes primary key
    - int type maps to database auto-increment/identity column
    - Database generates values automatically on insert
    - Cannot be null (value type)
    
    Property Syntax: { get; set; }
    - Auto-implemented property with public getter and setter
    - Backing field created automatically by compiler
    - Both reading and writing allowed from outside the class
    - Entity Framework requires setter for property mapping
    
    Why int for ID:
    1. **Performance**: Fast indexing and joins in database
    2. **Sequential**: Auto-increment provides natural ordering
    3. **Size**: 4 bytes, efficient for most applications
    4. **Simplicity**: Easy to work with and understand
    5. **URL-Friendly**: Simple integer IDs in REST URLs
    
    Alternative ID Strategies:
    - long: For applications expecting millions+ records
    - Guid: For distributed systems, globally unique
    - string: For natural keys or complex identifier schemes
    - Composite keys: Multiple properties forming the key
    
    Database Configuration:
    ```sql
    CREATE TABLE Games (
        Id int IDENTITY(1,1) PRIMARY KEY,
        -- other columns...
    )
    ```
    */
    
    // Game name property with required constraint
    public required string Name { get; set; }
    /*
    Required Keyword (C# 11+):
    
    What 'required' Does:
    - Compiler enforces that this property must be set during object construction
    - Prevents creating Game objects without specifying a Name
    - Compile-time error if Name is not initialized
    - Runtime exception if not set through reflection or unsafe code
    
    Required vs Traditional Validation:
    Traditional approach:
    ```csharp
    public string Name { get; set; } = string.Empty;
    // Requires Data Annotations: [Required]
    // Runtime validation only
    ```
    
    Required keyword approach:
    ```csharp
    public required string Name { get; set; }
    // Compile-time enforcement
    // No need for default value
    // Cannot create object without setting Name
    ```
    
    Object Creation Examples:
    
    Valid (required property set):
    ```csharp
    var game = new Game { 
        Name = "Elden Ring",
        GenreId = 1,
        Price = 59.99m,
        ReleaseDate = new DateOnly(2022, 2, 25)
    };
    ```
    
    Invalid (compile error):
    ```csharp
    var game = new Game { 
        GenreId = 1,
        Price = 59.99m
        // Name not set - COMPILE ERROR
    };
    ```
    
    Entity Framework Considerations:
    - EF Core can handle required properties during materialization
    - Database should have NOT NULL constraint for consistency
    - Ensures data integrity at both application and database levels
    
    Business Rules Enforced:
    1. **Every game must have a name** - fundamental business requirement
    2. **No anonymous games** - games are identified by their names
    3. **Data consistency** - prevents incomplete objects in the system
    
    String Reference Type:
    - Reference type, can theoretically be null
    - 'required' keyword prevents null assignment during construction
    - Combined with nullable reference types for complete null safety
    */
    
    // Foreign key property referencing Genre entity
    public int GenreId { get; set; }
    /*
    Foreign Key Property:
    
    Entity Framework Foreign Key Convention:
    - Property named "{NavigationProperty}Id" automatically becomes foreign key
    - Links to the Id property of the referenced entity (Genre)
    - Creates database foreign key constraint automatically
    - Enables referential integrity at database level
    
    Database Relationship:
    ```sql
    ALTER TABLE Games 
    ADD CONSTRAINT FK_Games_Genres_GenreId 
    FOREIGN KEY (GenreId) REFERENCES Genres(Id);
    ```
    
    Why Separate GenreId and Genre Properties:
    1. **Performance**: Can work with GenreId without loading Genre entity
    2. **Flexibility**: Can set genre without loading full Genre object
    3. **Database Mapping**: Matches database foreign key column structure
    4. **EF Conventions**: Standard pattern for EF Core relationships
    
    int Type (Not Nullable):
    - Every game must belong to a genre (business rule)
    - Cannot be null, prevents orphaned games
    - Database enforces NOT NULL constraint
    - Referential integrity maintained
    
    Usage Patterns:
    
    Setting genre by ID only:
    ```csharp
    game.GenreId = 3; // Set to Action genre
    // No need to load Genre entity
    ```
    
    Setting genre with navigation property:
    ```csharp
    game.Genre = actionGenre;
    game.GenreId = actionGenre.Id; // EF sets automatically
    ```
    
    Querying with foreign key:
    ```csharp
    var rpgGames = context.Games.Where(g => g.GenreId == rpgGenreId);
    ```
    
    Alternative Nullable Design:
    ```csharp
    public int? GenreId { get; set; } // Allow null
    ```
    This would allow games without genres, but violates business rules
    */
    
    // Navigation property to Genre entity
    public Genre? Genre { get; set; } // 1 to 1 relationship between Game and Genre
    /*
    Navigation Property Explanation:
    
    What Navigation Properties Do:
    - Represent relationships between entities in object-oriented way
    - Allow traversing from Game to related Genre object
    - Enable lazy loading, eager loading, and explicit loading
    - Provide strongly-typed access to related data
    
    Relationship Type: Many-to-One (not 1-to-1 as comment suggests)
    - Many games can have the same genre
    - Each game has exactly one genre
    - Correct relationship: Game (Many) → Genre (One)
    - One-to-one would mean each genre has exactly one game
    
    Nullable Reference Type: Genre?
    - Genre property can be null even though GenreId cannot
    - Null indicates the Genre entity hasn't been loaded yet
    - EF Core uses lazy loading or explicit loading to populate
    - Prevents automatic database queries for every Genre access
    
    Entity Framework Loading Patterns:
    
    1. **Lazy Loading** (if enabled):
    ```csharp
    var game = context.Games.First();
    var genreName = game.Genre.Name; // Triggers database query
    ```
    
    2. **Eager Loading**:
    ```csharp
    var games = context.Games.Include(g => g.Genre).ToList();
    // Genre is loaded with initial query
    ```
    
    3. **Explicit Loading**:
    ```csharp
    var game = context.Games.First();
    context.Entry(game).Reference(g => g.Genre).Load();
    var genreName = game.Genre.Name; // No additional query
    ```
    
    4. **Projection (No Navigation)**:
    ```csharp
    var gameData = context.Games
        .Select(g => new { g.Name, GenreName = g.Genre.Name })
        .ToList();
    ```
    
    Property Configuration in EF Core:
    By convention, EF Core configures this as:
    - Foreign key: GenreId property
    - Navigation property: Genre property
    - Relationship: Many Games to One Genre
    - Cascading: Depends on configuration (typically restrict)
    
    Fluent API Configuration:
    ```csharp
    modelBuilder.Entity<Game>()
        .HasOne(g => g.Genre)
        .WithMany() // Genre has many Games
        .HasForeignKey(g => g.GenreId);
    ```
    
    Business Logic Usage:
    ```csharp
    // Access genre information
    if (game.Genre != null) {
        Console.WriteLine($"Game: {game.Name}, Genre: {game.Genre.Name}");
    }
    
    // Filter by genre
    var actionGames = context.Games
        .Where(g => g.Genre.Name == "Action")
        .ToList();
    ```
    
    Performance Considerations:
    - Navigation properties add complexity to queries
    - Can cause N+1 query problems if not handled carefully
    - Use Include() for eager loading when needed
    - Consider projection for read-only scenarios
    - Monitor query execution plans for performance
    */
    
    // Game price property
    public decimal Price { get; set; }
    /*
    Price Property Design:
    
    decimal Type for Money:
    - Exact decimal representation (no floating-point errors)
    - Essential for financial calculations
    - Maps to database decimal/numeric columns
    - Prevents rounding errors in price calculations
    
    Why Not double or float:
    ```csharp
    // Problems with float/double for money:
    double price = 19.99;
    double total = price * 3; // Might be 59.96999999999999
    
    // Correct with decimal:
    decimal price = 19.99m;
    decimal total = price * 3; // Exactly 59.97
    ```
    
    Database Mapping:
    - Maps to DECIMAL(18,2) or similar in most databases
    - Precision and scale can be configured with Data Annotations or Fluent API
    - NOT NULL constraint (decimal is value type)
    
    Configuration Options:
    ```csharp
    [Column(TypeName = "decimal(8,2)")]
    public decimal Price { get; set; }
    
    // Or with Fluent API:
    modelBuilder.Entity<Game>()
        .Property(g => g.Price)
        .HasColumnType("decimal(8,2)");
    ```
    
    Business Rules Considerations:
    - No validation attributes here (unlike DTOs)
    - Validation handled at API boundary (DTOs)
    - Entity represents raw business data
    - Database constraints provide final validation
    
    Price Evolution Scenarios:
    - Price changes over time (sales, adjustments)
    - Multiple currencies (might need separate Price entity)
    - Regional pricing (different prices per region)
    - Historical pricing (audit trail of price changes)
    
    Value Object Alternative:
    ```csharp
    public Money Price { get; set; } // Custom Money value object
    // Could include currency, formatting, etc.
    ```
    */
    
    // Game release date property
    public DateOnly ReleaseDate { get; set; }
    /*
    DateOnly Type (C# 10+):
    
    Why DateOnly Instead of DateTime:
    1. **Semantic Correctness**: Release dates don't have time components
    2. **No Time Zone Issues**: Date-only values avoid timezone complexity
    3. **Clear Intent**: Makes it obvious this is date-only data
    4. **Correct Comparisons**: Date comparisons without time interference
    5. **Better Serialization**: Consistent date format without time
    
    DateTime Problems for Dates:
    ```csharp
    // DateTime complications:
    var releaseDate = new DateTime(2022, 2, 25, 0, 0, 0); // Midnight in what timezone?
    var tomorrow = DateTime.Now.AddDays(1); // What time tomorrow?
    
    // DateOnly clarity:
    var releaseDate = new DateOnly(2022, 2, 25); // Unambiguous date
    var tomorrow = DateOnly.FromDateTime(DateTime.Now).AddDays(1); // Clear date arithmetic
    ```
    
    Database Mapping:
    - Maps to DATE column type (not DATETIME)
    - Stores only date information
    - More efficient storage than full datetime
    - Database queries work naturally with date-only operations
    
    Entity Framework Configuration:
    ```csharp
    modelBuilder.Entity<Game>()
        .Property(g => g.ReleaseDate)
        .HasColumnType("date");
    ```
    
    Business Scenarios:
    1. **Historical Games**: Accurate release dates for classic games
    2. **Future Releases**: Planned release dates for upcoming games
    3. **Regional Releases**: Different release dates per region (might need separate entity)
    4. **Anniversary Calculations**: Easy date arithmetic for anniversaries
    
    Common Operations:
    ```csharp
    // Date calculations
    var age = DateOnly.FromDateTime(DateTime.Now) - game.ReleaseDate;
    var isClassic = game.ReleaseDate.Year < 2000;
    var releaseYear = game.ReleaseDate.Year;
    
    // Filtering by date ranges
    var recentGames = context.Games
        .Where(g => g.ReleaseDate >= new DateOnly(2020, 1, 1))
        .ToList();
    ```
    
    JSON Serialization:
    - Serializes as ISO 8601 date string: "2022-02-25"
    - No time zone information included
    - Consistent format across different locales
    - Easy to parse in client applications
    
    Validation Considerations:
    - No automatic validation at entity level
    - Could add business rules (e.g., not in far future)
    - Database constraints could enforce reasonable ranges
    - API DTOs handle input validation
    */
}

/*
Complete Entity Analysis:

Domain Model Responsibilities:
1. **Data Storage**: Properties map to database columns
2. **Relationships**: Navigation properties define entity associations  
3. **Business Identity**: Id property provides unique identification
4. **Data Integrity**: required keyword enforces essential properties
5. **Type Safety**: Strong typing prevents many runtime errors

Entity Framework Core Integration:

Table Mapping:
- Game class → Games table
- Properties → Columns
- Navigation properties → Foreign key relationships
- Conventions handle most configuration automatically

Relationship Summary:
```
Games Table:
- Id (Primary Key, int, auto-increment)
- Name (nvarchar(max), NOT NULL)
- GenreId (int, NOT NULL, Foreign Key → Genres.Id)
- Price (decimal(18,2), NOT NULL)
- ReleaseDate (date, NOT NULL)

Relationships:
- Game.GenreId → Genre.Id (Many-to-One)
- Game.Genre navigation property (loaded on demand)
```

CRUD Operations with Entity:

Create:
```csharp
var game = new Game {
    Name = "New Game",
    GenreId = 1,
    Price = 29.99m,
    ReleaseDate = new DateOnly(2024, 6, 1)
};
context.Games.Add(game);
await context.SaveChangesAsync();
```

Read:
```csharp
var game = await context.Games
    .Include(g => g.Genre)
    .FirstOrDefaultAsync(g => g.Id == id);
```

Update:
```csharp
game.Price = 19.99m;
game.Name = "Updated Name";
await context.SaveChangesAsync();
```

Delete:
```csharp
context.Games.Remove(game);
await context.SaveChangesAsync();
```

Design Patterns Demonstrated:

1. **Domain Model**: Rich objects with business meaning
2. **Data Mapper**: EF Core maps between objects and database
3. **Identity**: Each entity has unique, persistent identity
4. **Association**: Relationships between entities via navigation properties
5. **Value Objects**: Properties like Price could evolve to value objects

Entity vs DTO Mapping:

Game Entity → GameDTO:
```csharp
var gameDto = new GameDTO(
    game.Id,
    game.Name,
    game.Genre?.Name ?? "Unknown", // Flatten relationship
    game.Price,
    game.ReleaseDate
);
```

CreateGameDTO → Game Entity:
```csharp
var game = new Game {
    Name = createDto.Name,
    GenreId = GetGenreId(createDto.Genre), // Resolve genre name to ID
    Price = createDto.Price,
    ReleaseDate = createDto.ReleaseDate
};
```

This Game entity provides a clean, strongly-typed representation of game data
that integrates seamlessly with Entity Framework Core while maintaining
clear business semantics and relationships.
*/