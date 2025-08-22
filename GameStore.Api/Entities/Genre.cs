// Namespace for domain entities
namespace GameStore.Api.Entities;
// Same namespace as other entities for logical grouping
// Entities represent the core business objects in the domain model
// This namespace isolates domain logic from API contracts (DTOs) and data access

/*
Genre Entity Design Philosophy:

The Genre entity represents a reference data entity - a lookup table that provides
standardized categorization for games. This is a common pattern in domain modeling
where certain properties need to be normalized and controlled.

Reference Data Characteristics:
1. **Relatively Static**: Genres don't change frequently
2. **Shared**: Multiple games can belong to the same genre
3. **Controlled Vocabulary**: Limited, predefined set of values
4. **Lookup Purpose**: Primarily used for categorization and filtering
5. **Small Dataset**: Typically hundreds of records, not thousands

Genre as an Entity vs String:
Alternative approach would be to use string properties:
```csharp
public string Genre { get; set; } // Simple but problematic
```

Why Entity is Better:
1. **Data Consistency**: Prevents typos and variations ("Action" vs "action" vs "ACTION")
2. **Referential Integrity**: Database enforces valid genre references
3. **Centralized Management**: One place to manage genre information
4. **Extensibility**: Can add more properties (description, icon, etc.)
5. **Performance**: Normalized storage, efficient joins
6. **Maintenance**: Easy to rename genres across all games
*/

// Genre entity class representing game categories
public class Genre
{
    /*
    Simple Entity Design:
    Genre is intentionally simple because it serves as reference data.
    Complex business logic typically resides in aggregate roots (like Game)
    while reference entities provide stable, controlled vocabularies.
    
    Class vs Record Consideration:
    Although Genre data is relatively static, it's still an entity because:
    1. **Database Mapping**: EF Core works best with classes for entities
    2. **Mutability**: Genre names might need updates (renaming, standardization)
    3. **Relationships**: Participates in entity relationships
    4. **Consistency**: Keeps all entities as classes for architectural consistency
    
    Future Evolution:
    This simple structure can evolve to include:
    - Description property for detailed genre information
    - Icon/Image properties for UI representation
    - Parent/Child relationships for genre hierarchies
    - Metadata like creation date, popularity metrics
    - Soft delete functionality
    */
    
    // Primary key property
    public int Id { get; set; }
    /*
    Primary Key Analysis:
    
    Auto-Increment ID Strategy:
    - Database generates sequential integer IDs automatically
    - Simple and efficient for reference data
    - Good performance for foreign key relationships
    - Easy to work with in application code
    
    Alternative ID Strategies for Reference Data:
    
    1. **Natural Keys** (Genre name as primary key):
    ```csharp
    [Key]
    public string Name { get; set; } // "Action", "RPG", etc.
    ```
    Pros: More meaningful, no joins needed
    Cons: Harder to change names, longer foreign keys
    
    2. **Enum-Based IDs** (predefined integer values):
    ```csharp
    public enum GenreId { Action = 1, RPG = 2, Strategy = 3 }
    ```
    Pros: Compile-time safety, very fast
    Cons: Requires code changes to add genres
    
    3. **GUID IDs** (globally unique identifiers):
    ```csharp
    public Guid Id { get; set; }
    ```
    Pros: Globally unique, good for distributed systems
    Cons: Larger storage, less human-readable
    
    Chosen Approach Benefits:
    - Simple integer arithmetic and comparisons
    - Efficient storage and indexing (4 bytes)
    - Standard pattern familiar to developers
    - Easy to seed with initial data
    - Compatible with auto-increment database features
    
    Entity Framework Configuration:
    By convention, this maps to:
    ```sql
    CREATE TABLE Genres (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(max) NOT NULL
    )
    ```
    
    Seeding Reference Data:
    ```csharp
    // In DbContext.OnModelCreating():
    modelBuilder.Entity<Genre>().HasData(
        new Genre { Id = 1, Name = "Action" },
        new Genre { Id = 2, Name = "RPG" },
        new Genre { Id = 3, Name = "Strategy" },
        new Genre { Id = 4, Name = "Simulation" },
        new Genre { Id = 5, Name = "Sports" }
    );
    ```
    */
    
    // Genre name property with required constraint
    public required string Name { get; set; }
    /*
    Required Name Property:
    
    Required Keyword Benefits:
    - Compile-time enforcement of essential business rule
    - Prevents creation of unnamed genres
    - Eliminates need for runtime null checks in many scenarios
    - Clear intention that every genre must have a name
    
    Business Rules Enforced:
    1. **Every genre must have a name** - fundamental requirement
    2. **No anonymous categories** - genres are identified by names
    3. **Data integrity** - prevents incomplete reference data
    
    String Type Considerations:
    - Reference type allows for descriptive names
    - Unicode support for international genre names
    - Variable length efficient for different name lengths
    - Can store names in multiple languages if needed
    
    Naming Conventions for Genres:
    - Standardized capitalization: "Action-Adventure", not "action-adventure"
    - Consistent terminology: "Role-Playing" vs "RPG"
    - Avoid abbreviations unless widely understood
    - Consider localization for international markets
    
    Database Constraints:
    ```sql
    ALTER TABLE Genres 
    ADD CONSTRAINT CK_Genre_Name_NotEmpty 
    CHECK (LEN(TRIM(Name)) > 0);
    
    CREATE UNIQUE INDEX IX_Genres_Name 
    ON Genres(Name);
    ```
    
    Validation Considerations:
    While entities don't typically include validation attributes,
    you might consider adding them for certain scenarios:
    
    ```csharp
    [MaxLength(50)]
    [Index(IsUnique = true)]
    public required string Name { get; set; }
    ```
    
    Alternative Validation Approaches:
    1. **Domain Validation**: Add validation methods to entity
    2. **Value Objects**: Wrap Name in a GenreName value object
    3. **Database Constraints**: Rely on database-level validation
    4. **Application Layer**: Validate in services before persistence
    
    Usage Patterns:
    
    Creating new genre:
    ```csharp
    var genre = new Genre { Name = "Battle Royale" };
    context.Genres.Add(genre);
    await context.SaveChangesAsync();
    ```
    
    Finding genre by name:
    ```csharp
    var genre = await context.Genres
        .FirstOrDefaultAsync(g => g.Name == "Action");
    ```
    
    Case-insensitive search:
    ```csharp
    var genre = await context.Genres
        .FirstOrDefaultAsync(g => g.Name.ToLower() == searchName.ToLower());
    ```
    
    Genre standardization:
    ```csharp
    // Normalize different variations to standard names
    var variations = new Dictionary<string, string> {
        { "rpg", "RPG" },
        { "role-playing", "RPG" },
        { "action-adventure", "Action-Adventure" }
    };
    ```
    */
}

/*
Entity Relationships Analysis:

Genre's Role in Domain Model:
- **Reference Entity**: Provides controlled vocabulary for game categorization
- **One-to-Many**: One genre can be associated with many games
- **Lookup Table**: Primary purpose is to normalize and control genre values
- **Stable Data**: Changes infrequently compared to transactional entities

Relationship with Game Entity:
```
Genre (One) ←─────── Game (Many)
   ↑                   ↓
  Id  ←─────────── GenreId (FK)
                  Genre (Nav)
```

Database Relationship:
```sql
-- Genres table
CREATE TABLE Genres (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(50) NOT NULL UNIQUE
);

-- Games table with foreign key
CREATE TABLE Games (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(max) NOT NULL,
    GenreId int NOT NULL,
    Price decimal(8,2) NOT NULL,
    ReleaseDate date NOT NULL,
    FOREIGN KEY (GenreId) REFERENCES Genres(Id)
);
```

Missing Navigation Property:
Notice that Genre doesn't have a navigation property back to Games:
```csharp
// Genre doesn't have:
public ICollection<Game> Games { get; set; }
```

Why No Reverse Navigation:
1. **Simplicity**: Genre is a lookup entity, doesn't need complex relationships
2. **Performance**: Avoid loading all games when working with genres
3. **Single Responsibility**: Genre's job is to provide classification, not manage games
4. **Query Flexibility**: Can query games by genre without navigation property

If needed, reverse navigation can be added:
```csharp
public class Genre
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Game> Games { get; set; } = new List<Game>();
}
```

Entity Framework Configuration:

By Convention:
- Table name: "Genres" (pluralized)
- Primary key: Id property (auto-increment)
- Required: Name property (NOT NULL constraint)
- Relationships: Discovered through Game.Genre navigation property

Fluent API Configuration:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Genre>(entity =>
    {
        entity.HasKey(g => g.Id);
        
        entity.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(50);
            
        entity.HasIndex(g => g.Name)
            .IsUnique();
            
        // Seed data
        entity.HasData(
            new Genre { Id = 1, Name = "Action" },
            new Genre { Id = 2, Name = "RPG" },
            new Genre { Id = 3, Name = "Strategy" }
        );
    });
    
    // Configure relationship from Game side
    modelBuilder.Entity<Game>()
        .HasOne(g => g.Genre)
        .WithMany() // No reverse navigation
        .HasForeignKey(g => g.GenreId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent deleting genres with games
}
```

CRUD Operations for Reference Data:

Create (Administrative):
```csharp
var newGenre = new Genre { Name = "Puzzle" };
context.Genres.Add(newGenre);
await context.SaveChangesAsync();
```

Read (Common):
```csharp
// Get all genres for dropdown
var genres = await context.Genres
    .OrderBy(g => g.Name)
    .ToListAsync();

// Find specific genre
var actionGenre = await context.Genres
    .FirstOrDefaultAsync(g => g.Name == "Action");
```

Update (Rare):
```csharp
var genre = await context.Genres.FindAsync(id);
if (genre != null)
{
    genre.Name = "Action-Adventure"; // Rename genre
    await context.SaveChangesAsync();
}
```

Delete (Very Rare):
```csharp
// Only if no games reference this genre
var genre = await context.Genres.FindAsync(id);
if (genre != null)
{
    context.Genres.Remove(genre);
    await context.SaveChangesAsync(); // Will fail if games reference this genre
}
```

Common Queries:

Games by genre:
```csharp
var actionGames = await context.Games
    .Where(g => g.Genre.Name == "Action")
    .ToListAsync();
```

Genre statistics:
```csharp
var genreStats = await context.Games
    .GroupBy(g => g.Genre.Name)
    .Select(group => new {
        Genre = group.Key,
        GameCount = group.Count(),
        AveragePrice = group.Average(g => g.Price)
    })
    .ToListAsync();
```

Performance Considerations:

1. **Caching**: Genres are perfect candidates for caching
```csharp
private static readonly MemoryCache _genreCache = new MemoryCache();

public async Task<List<Genre>> GetGenresAsync()
{
    return await _genreCache.GetOrCreateAsync("all-genres", async entry =>
    {
        entry.SlidingExpiration = TimeSpan.FromHours(1);
        return await context.Genres.ToListAsync();
    });
}
```

2. **Eager Loading**: Usually not needed due to small dataset
3. **Indexing**: Index on Name for lookups
4. **Denormalization**: Consider including genre name in Game for read-heavy scenarios

Design Evolution:

Simple Genre (Current):
```csharp
public class Genre
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
```

Enhanced Genre (Future):
```csharp
public class Genre
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public ICollection<Game> Games { get; set; } = new List<Game>();
}
```

Hierarchical Genres:
```csharp
public class Genre
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? ParentGenreId { get; set; }
    public Genre? ParentGenre { get; set; }
    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();
}
```

This Genre entity provides a clean, simple foundation for game categorization
that can evolve with business requirements while maintaining data integrity
and performance through proper database design and Entity Framework configuration.
*/