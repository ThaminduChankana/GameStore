// Using directive to import DTO types
using GameStore.Api.DTOS;
// Imports CreateGameDTO, GameDTO, UpdateGameDTO record classes
// These DTOs define the data contracts for API input and output
// Separation of DTOs from endpoints promotes clean architecture

// Namespace for API endpoint configuration
namespace GameStore.Api.Endpoints;
// Endpoints namespace contains classes that configure API routes and handlers
// This follows the principle of organizing code by feature/responsibility
// Makes endpoint configuration easy to locate and maintain

// Static class for Games API endpoints configuration
public static class GamesEndpoints
{
    /*
    Static Class Design Pattern:
    This class is static because:
    1. **No Instance State**: Doesn't need to maintain any instance data
    2. **Utility Functions**: Contains only utility methods for configuration
    3. **Extension Methods**: Provides extension methods for WebApplication
    4. **Stateless Operations**: All methods are stateless and side-effect free
    5. **Memory Efficiency**: No object instantiation overhead
    
    Extension Class Requirements:
    - Must be static (required for extension methods)
    - Must contain static methods (required for extension methods)
    - First parameter must use 'this' keyword (makes it an extension method)
    
    Benefits of Extension Methods:
    1. **Fluent API**: Allows method chaining on WebApplication
    2. **Discoverability**: Methods appear in IntelliSense on WebApplication
    3. **Organization**: Keeps related endpoint configuration together
    4. **Reusability**: Can be easily reused across different applications
    5. **Separation of Concerns**: Endpoint logic separate from startup logic
    */
    
    // Constant for endpoint naming to enable route linking
    const string GetGameEndPointName = "GetGameById";
    /*
    Named Routes Pattern:
    
    Purpose of Named Routes:
    1. **Link Generation**: Generate URLs to other endpoints
    2. **CreatedAtRoute**: Return 201 Created with location header
    3. **Refactoring Safety**: Change route patterns without breaking links
    4. **Documentation**: Self-documenting endpoint relationships
    
    Usage Example:
    When creating a game, we return:
    ```csharp
    return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game);
    ```
    
    This generates:
    - Status: 201 Created
    - Location Header: /games/{id}
    - Body: The created game object
    
    Benefits:
    - Follows REST conventions for resource creation
    - Provides client with URL to access created resource
    - Maintains loose coupling between endpoint definitions
    - Central place to manage route naming
    
    Alternative Approaches:
    - Hard-coded URLs: Results.Created("/games/42", game) - fragile
    - Route parameters: Less discoverable and maintainable
    - No location header: Doesn't follow REST conventions
    */
    
    // In-memory data store for demonstration purposes
    private static readonly List<GameDTO> games = [
        /*
        In-Memory Data Store:
        This is a simplified data store for demonstration/prototyping purposes.
        
        Collection Initialization Syntax (C# 12):
        The [item1, item2, item3] syntax is collection expression syntax
        Equivalent to: new List<GameDTO> { item1, item2, item3 }
        
        Production Considerations:
        1. **Persistence**: Real applications use databases (SQL Server, PostgreSQL, etc.)
        2. **Concurrency**: Multiple requests could modify this simultaneously
        3. **Scalability**: Memory usage grows with data size
        4. **Data Loss**: Application restart loses all data
        5. **Performance**: No indexing or query optimization
        
        Why Use In-Memory for Learning:
        - Simple to understand and debug
        - No database setup required
        - Fast development and testing
        - Clear demonstration of API concepts
        - Easy to see data changes during development
        
        Migration Path to Database:
        1. Add Entity Framework DbContext
        2. Replace List<GameDTO> with DbSet<Game>
        3. Add database configuration and connection string
        4. Replace direct list operations with EF queries
        5. Add proper async/await patterns
        */
        
        // Sample game data with diverse examples
        new (1, "The Witcher 3: Wild Hunt", "RPG", 39.99m, new DateOnly(2015, 5, 19)),
        /*
        Sample Data Analysis:
        - Mature game with reduced price point
        - Popular RPG demonstrating genre categorization
        - Historical release date for testing date handling
        */
        
        new (2, "Cyberpunk 2077", "RPG", 59.99m, new DateOnly(2020, 12, 10)),
        /*
        Recent AAA game at full price point
        - Tests upper price range validation
        - Same genre as Witcher 3 for filtering tests
        - Recent release date
        */
        
        new (3, "Red Dead Redemption 2", "Action-Adventure", 49.99m, new DateOnly(2018, 10, 26)),
        /*
        Different genre example
        - Mid-range pricing
        - Longer genre name testing string length handling
        - Tests genre variety in data set
        */
        
        new (4, "Hades", "Roguelike", 24.99m, new DateOnly(2020, 9, 17)),
        /*
        Indie game example
        - Lower price point
        - Unique genre category
        - Same year as Cyberpunk for date comparison testing
        */
        
        new (5, "Stardew Valley", "Simulation", 14.99m, new DateOnly(2016, 2, 26))
        /*
        Budget-friendly indie game
        - Tests minimum price range
        - Different genre category
        - Older release date
        - Popular farming simulation game
        */
    ];

    // Extension method to configure Games API endpoints
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    /*
    Extension Method Signature Breakdown:
    - 'public static': Required for extension methods
    - 'RouteGroupBuilder': Return type allows method chaining
    - 'this WebApplication app': Makes this an extension method on WebApplication
    - Method name follows convention: Map{Feature}Endpoints
    
    RouteGroupBuilder Benefits:
    1. **Grouping**: Logically groups related endpoints
    2. **Common Configuration**: Apply settings to all endpoints in group
    3. **Route Prefixes**: Shared URL prefix for all group endpoints
    4. **Middleware**: Apply middleware to entire group
    5. **Validation**: Apply validation rules to all group endpoints
    
    Usage in Program.cs:
    ```csharp
    app.MapGamesEndpoints(); // Extension method call
    ```
    
    Method Chaining Pattern:
    Return RouteGroupBuilder allows:
    ```csharp
    app.MapGamesEndpoints()
       .RequireAuthorization()  // Add auth to all game endpoints
       .WithOpenApi();          // Add OpenAPI docs to all endpoints
    ```
    */
    {
        // Create route group with common prefix and validation
        var group = app.MapGroup("/games").WithParameterValidation();
        /*
        Route Group Configuration:
        
        MapGroup("/games"):
        - Creates a route group with "/games" prefix
        - All endpoints in group will have URLs starting with /games
        - Promotes RESTful URL structure
        
        WithParameterValidation():
        - Extension method from MinimalApis.Extensions NuGet package
        - Automatically validates request parameters and DTOs
        - Returns 400 Bad Request for validation failures
        - Eliminates need for manual validation in each endpoint
        
        Route Group Benefits:
        1. **DRY Principle**: Don't repeat "/games" prefix in each endpoint
        2. **Consistency**: Ensures all game endpoints follow same pattern
        3. **Maintainability**: Change prefix in one place affects all endpoints
        4. **Feature Organization**: Groups related functionality together
        
        Alternative Organization:
        - Individual MapGet/MapPost calls on app (less organized)
        - Controller classes (heavier, more complex for simple APIs)
        - Separate minimal API modules (good for larger applications)
        */
        
        // GET /games - Retrieve all games
        group.MapGet("/", () => games);
        /*
        Get All Games Endpoint:
        
        Route Pattern: "/"
        - Combined with group prefix becomes: GET /games
        - Root of the games resource collection
        - Follows REST convention for collection endpoints
        
        Handler: () => games
        - Simple lambda expression returning the games collection
        - No parameters needed for "get all" operation
        - Returns List<GameDTO> which is automatically serialized to JSON
        
        HTTP Response:
        - Status: 200 OK (automatic for successful return)
        - Content-Type: application/json (automatic for object return)
        - Body: JSON array of game objects
        
        Response Example:
        ```json
        [
          {
            "id": 1,
            "name": "The Witcher 3: Wild Hunt",
            "genre": "RPG",
            "price": 39.99,
            "releaseDate": "2015-05-19"
          },
          // ... more games
        ]
        ```
        
        Production Considerations:
        1. **Pagination**: Large collections should be paginated
        2. **Filtering**: Add query parameters for filtering
        3. **Sorting**: Allow sorting by different properties
        4. **Performance**: Consider caching for frequently accessed data
        5. **Security**: May need authorization for sensitive data
        
        Enhanced Version Example:
        ```csharp
        group.MapGet("/", (int page = 1, int size = 10, string? genre = null) => {
            var query = games.AsQueryable();
            if (genre != null) query = query.Where(g => g.Genre == genre);
            return query.Skip((page - 1) * size).Take(size);
        });
        ```
        */
        
        // GET /games/{id} - Retrieve specific game by ID
        group.MapGet("/{id}", (int id) =>
        {
            /*
            Get Single Game Endpoint:
            
            Route Pattern: "/{id}"
            - Combined with group prefix becomes: GET /games/{id}
            - {id} is a route parameter that gets bound to the 'id' parameter
            - Follows REST convention for individual resource access
            
            Parameter Binding:
            - ASP.NET Core automatically binds route parameter to method parameter
            - Type conversion happens automatically (string "42" → int 42)
            - Invalid types return 400 Bad Request automatically
            
            Route Examples:
            - GET /games/1 → id = 1
            - GET /games/42 → id = 42
            - GET /games/abc → 400 Bad Request (invalid int)
            */
            
            // Find game in collection using LINQ
            GameDTO? game = games.Find(game => game.Id == id);
            /*
            Data Lookup Logic:
            
            List<T>.Find() Method:
            - Searches for first element matching the predicate
            - Returns the element if found, null if not found
            - O(n) time complexity - scans through list sequentially
            
            Lambda Expression: game => game.Id == id
            - Predicate function that tests each game
            - Returns true when game.Id matches the requested id
            - Captures the 'id' variable from the outer scope
            
            Nullable Reference Type: GameDTO?
            - The ? indicates this variable can be null
            - Enables compile-time null safety checking
            - Makes null handling explicit and safer
            
            Production Database Equivalent:
            ```csharp
            var game = await context.Games.FirstOrDefaultAsync(g => g.Id == id);
            ```
            */

            // Return appropriate HTTP response based on whether game was found
            return game is null ? Results.NotFound() : Results.Ok(game);
            /*
            Conditional Response Logic:
            
            Pattern Matching: game is null
            - Modern C# syntax for null checking
            - More readable than: game == null
            - Supports more complex pattern matching scenarios
            
            Results.NotFound():
            - Returns HTTP 404 Not Found status
            - Empty response body
            - Follows REST convention for missing resources
            
            Results.Ok(game):
            - Returns HTTP 200 OK status
            - Serializes game object to JSON response body
            - Sets Content-Type: application/json automatically
            
            Response Examples:
            
            Found (HTTP 200):
            ```json
            {
              "id": 1,
              "name": "The Witcher 3: Wild Hunt",
              "genre": "RPG", 
              "price": 39.99,
              "releaseDate": "2015-05-19"
            }
            ```
            
            Not Found (HTTP 404):
            ```
            (empty body)
            ```
            
            Alternative Response Patterns:
            - Throw exceptions: Not recommended for expected scenarios like 404
            - Custom error objects: More complex but can provide detailed error info
            - Problem Details: RFC 7807 standard for API error responses
            */
        })
        .WithName(GetGameEndPointName);
        /*
        Named Route Assignment:
        
        WithName() Purpose:
        - Assigns a name to this endpoint for link generation
        - Enables CreatedAtRoute() calls from other endpoints
        - Provides compile-time safety for route references
        - Improves maintainability of inter-endpoint links
        
        Usage in POST Endpoint:
        When creating a game, we can generate a link to this GET endpoint:
        ```csharp
        return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game);
        ```
        
        Generated Response Headers:
        ```
        Location: /games/42
        ```
        
        Benefits:
        1. **Loose Coupling**: Endpoints don't hard-code URLs to other endpoints
        2. **Refactoring Safety**: Route pattern changes don't break other endpoints
        3. **REST Compliance**: Proper Location headers in 201 Created responses
        4. **Documentation**: Makes endpoint relationships explicit
        */
        
        // POST /games - Create new game
        group.MapPost("", (CreateGameDTO newGame) =>
        {
            /*
            Create Game Endpoint:
            
            Route Pattern: ""
            - Empty string means just the group prefix: POST /games
            - Follows REST convention: POST to collection creates new resource
            - Request body contains the data for the new resource
            
            Parameter Binding: CreateGameDTO newGame
            - ASP.NET Core automatically deserializes JSON request body to CreateGameDTO
            - Model validation happens automatically due to WithParameterValidation()
            - Validation attributes on CreateGameDTO are checked automatically
            
            JSON Request Body Example:
            ```json
            {
              "name": "Elden Ring",
              "genre": "Action RPG",
              "price": 59.99,
              "releaseDate": "2022-02-25"
            }
            ```
            
            Automatic Validation:
            - [Required] attributes checked for null/empty values
            - [StringLength] attributes check string length constraints
            - [Range] attributes check numeric ranges
            - If validation fails, 400 Bad Request returned automatically
            */
            
            // Create new GameDTO with generated ID
            GameDTO game = new
            (games.Count + 1,          // Simple ID generation
            newGame.Name,              // Copy from input DTO
            newGame.Genre,             // Copy from input DTO  
            newGame.Price,             // Copy from input DTO
            newGame.ReleaseDate);      // Copy from input DTO
            /*
            Object Creation and ID Generation:
            
            ID Generation Strategy: games.Count + 1
            - Simple auto-incrementing ID for demonstration
            - Not thread-safe (multiple requests could get same ID)
            - Not production-ready (deleted items leave gaps)
            
            Production ID Generation:
            - Database auto-increment: IDENTITY columns in SQL Server
            - GUIDs: Globally unique, good for distributed systems
            - Sequence generators: Database sequences for guaranteed uniqueness
            - Snowflake IDs: Distributed ID generation for microservices
            
            DTO Mapping Pattern:
            - Convert CreateGameDTO (input) → GameDTO (internal/output)
            - Separates API contract from internal representation
            - Allows different validation rules for input vs output
            - Enables API evolution without breaking internal code
            
            Constructor Syntax:
            Using target-typed new expression (C# 9+):
            ```csharp
            GameDTO game = new(id, name, genre, price, date);
            ```
            Equivalent to:
            ```csharp
            GameDTO game = new GameDTO(id, name, genre, price, date);
            ```
            */
            
            // Add to in-memory collection
            games.Add(game);
            /*
            Data Persistence:
            
            List<T>.Add() Operation:
            - Appends new game to end of collection
            - O(1) amortized time complexity
            - Not thread-safe in production scenarios
            
            Production Database Equivalent:
            ```csharp
            context.Games.Add(new Game { ... });
            await context.SaveChangesAsync();
            ```
            
            Concurrency Considerations:
            - Multiple simultaneous requests could corrupt the list
            - Production needs proper transaction handling
            - Database systems handle concurrency automatically
            */
            
            // Return 201 Created with location header and created resource
            return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game);
            /*
            REST-Compliant Response:
            
            CreatedAtRoute() Method:
            - Returns HTTP 201 Created status
            - Sets Location header to URL of created resource
            - Includes created resource in response body
            - Follows REST conventions for resource creation
            
            Parameters Breakdown:
            1. GetGameEndPointName: Named route to link to
            2. new { id = game.Id }: Route values for URL generation
            3. game: Object to serialize in response body
            
            Generated Response:
            ```
            HTTP 201 Created
            Location: /games/42
            Content-Type: application/json
            
            {
              "id": 42,
              "name": "Elden Ring",
              "genre": "Action RPG",
              "price": 59.99,
              "releaseDate": "2022-02-25"
            }
            ```
            
            Client Benefits:
            1. **Immediate Access**: Location header provides URL to access created resource
            2. **Confirmation**: Response body confirms what was created
            3. **ID Discovery**: Client learns the generated ID for future operations
            4. **REST Compliance**: Standard HTTP semantics for resource creation
            
            Alternative Response Patterns:
            - Results.Ok(game): Less RESTful, doesn't indicate creation
            - Results.Created($"/games/{game.Id}", game): Hard-coded URL, less maintainable
            - No response body: Minimalist but less useful for clients
            */
        });
        /*
        Endpoint Filter Application:
        
        WithParameterValidation() (Applied to Group):
        - Automatically validates CreateGameDTO using Data Annotations
        - Returns detailed validation errors for failed validation
        - Eliminates need for manual ModelState checking
        
        Validation Error Response Example:
        ```json
        HTTP 400 Bad Request
        {
          "errors": {
            "Name": ["The Name field is required."],
            "Price": ["The field Price must be between 1 and 100."]
          }
        }
        ```
        
        MinimalApis.Extensions Benefits:
        - Reduces boilerplate validation code
        - Consistent error response format
        - Automatic integration with Data Annotations
        - Better developer experience with less manual work
        */
        
        // PUT /games/{id} - Update existing game
        group.MapPut("/{id}", (int id, UpdateGameDTO updatedGame) =>
        {
            /*
            Update Game Endpoint:
            
            Route Pattern: "/{id}"
            - Combined with group prefix: PUT /games/{id}
            - Follows REST convention: PUT to specific resource updates it
            - Route parameter provides resource identification
            
            Parameters:
            1. int id: Resource identifier from URL route
            2. UpdateGameDTO updatedGame: Complete replacement data from request body
            
            PUT Semantics:
            - Complete replacement of resource (not partial update)
            - Idempotent: Same request multiple times has same effect
            - Should include all resource properties in request body
            
            Request Example:
            ```
            PUT /games/42
            {
              "id": 42,
              "name": "Updated Game Name",
              "genre": "Updated Genre", 
              "price": 29.99,
              "releaseDate": "2024-01-01"
            }
            ```
            */
            
            // Find existing game by ID
            GameDTO? game = games.Find(game => game.Id == id);
            /*
            Existence Check:
            - Same lookup pattern as GET endpoint
            - Must verify resource exists before updating
            - Nullable reference type enables safe null handling
            
            Alternative Lookup Strategies:
            - FirstOrDefault with LINQ: games.FirstOrDefault(g => g.Id == id)
            - Dictionary lookup: O(1) if using Dictionary<int, GameDTO>
            - Database query: context.Games.FindAsync(id)
            */
            
            // Return 404 if game doesn't exist
            if (game is null)
            {
                return Results.NotFound();
            }
            /*
            Early Return Pattern:
            - Guard clause prevents further processing if resource not found
            - Returns 404 Not Found immediately
            - Follows REST convention for missing resources
            - Prevents null reference exceptions in subsequent code
            
            Alternative Error Handling:
            - Throw NotFoundException: Handled by global exception handler
            - Return Problem(): RFC 7807 Problem Details response
            - Custom error response: More detailed error information
            */
            
            // Create updated game using 'with' expression (record syntax)
            GameDTO updatedGameDTO = game with
            {
                Name = updatedGame.Name,
                Genre = updatedGame.Genre,
                Price = updatedGame.Price,
                ReleaseDate = updatedGame.ReleaseDate
            };
            /*
            Record 'with' Expression:
            
            Immutable Update Pattern:
            - Creates new record instance with modified properties
            - Original 'game' record remains unchanged
            - Only specified properties are updated
            - Unspecified properties (like Id) are copied from original
            
            Benefits of 'with' Expression:
            1. **Immutability**: Original record is not modified
            2. **Thread Safety**: Safe for concurrent access
            3. **Selective Updates**: Only change specified properties
            4. **Type Safety**: Compile-time checking of property names
            5. **Performance**: Efficient copying of unchanged properties
            
            Traditional Class Equivalent:
            ```csharp
            var updatedGame = new GameDTO(
                game.Id,              // Keep original ID
                updatedGame.Name,     // Update name
                updatedGame.Genre,    // Update genre
                updatedGame.Price,    // Update price
                updatedGame.ReleaseDate // Update release date
            );
            ```
            
            Why Not Update in Place:
            - Records are immutable by design
            - Immutability prevents accidental state changes
            - Safer in multi-threaded environments
            - Aligns with functional programming principles
            */
            
            // Replace game in collection
            int index = games.FindIndex(game => game.Id == id);
            games[index] = updatedGameDTO;
            /*
            Collection Update Process:
            
            FindIndex() Method:
            - Finds the index of the item matching the predicate
            - Returns -1 if no match found (shouldn't happen here since we found it above)
            - O(n) time complexity for List<T>
            
            Index Assignment:
            - Replaces the item at the specific index
            - Maintains the same position in the collection
            - O(1) operation for List<T> index access
            
            Production Database Equivalent:
            ```csharp
            var entity = await context.Games.FindAsync(id);
            entity.Name = updatedGame.Name;
            entity.Genre = updatedGame.Genre;
            entity.Price = updatedGame.Price;
            entity.ReleaseDate = updatedGame.ReleaseDate;
            await context.SaveChangesAsync();
            ```
            
            Alternative Collection Strategies:
            - Dictionary<int, GameDTO>: O(1) lookup and update
            - Replace with LINQ: games = games.Select(g => g.Id == id ? updatedGame : g).ToList()
            - Remove and re-add: Less efficient but simpler logic
            */
            
            // Return updated game
            return Results.Ok(updatedGameDTO);
            /*
            Success Response:
            
            Results.Ok() with Object:
            - Returns HTTP 200 OK status
            - Serializes updatedGameDTO to JSON response body
            - Confirms the update operation succeeded
            - Provides client with the updated resource state
            
            Response Example:
            ```json
            HTTP 200 OK
            {
              "id": 42,
              "name": "Updated Game Name",
              "genre": "Updated Genre",
              "price": 29.99,
              "releaseDate": "2024-01-01"
            }
            ```
            
            Alternative Response Patterns:
            - Results.NoContent(): 204 No Content (successful update, no body)
            - Results.Accepted(): 202 Accepted (for async operations)
            - Include metadata: Timestamps, version numbers, etc.
            
            REST Conventions:
            - 200 OK: Successful update with response body
            - 204 No Content: Successful update without response body
            - Both are valid; choice depends on client needs
            */
        });
        
        // DELETE /games/{id} - Delete existing game
        group.MapDelete("/{id}", (int id) =>
        {
            /*
            Delete Game Endpoint:
            
            Route Pattern: "/{id}"
            - Combined with group prefix: DELETE /games/{id}
            - Follows REST convention: DELETE to specific resource removes it
            - Route parameter identifies which resource to delete
            
            DELETE Semantics:
            - Removes the specified resource
            - Idempotent: Deleting same resource multiple times has same effect
            - Should return appropriate status even if resource doesn't exist
            
            Request Example:
            ```
            DELETE /games/42
            ```
            */
            
            // Find game to delete
            GameDTO? game = games.Find(game => game.Id == id);
            /*
            Pre-Delete Lookup:
            - Find the game before attempting deletion
            - Allows returning the deleted resource in response
            - Enables proper 404 handling for missing resources
            - Common pattern for DELETE operations
            
            Alternative Approaches:
            - Try to remove directly: games.RemoveAll(g => g.Id == id)
            - Check remove result: bool removed = games.Remove(game)
            - Skip lookup: Assume resource exists (less user-friendly)
            */
            
            // Return 404 if game doesn't exist
            if (game == null)
            {
                return Results.NotFound();
            }
            /*
            Missing Resource Handling:
            
            404 vs 204 Debate:
            - Some prefer 404: Resource doesn't exist, can't delete
            - Others prefer 204: Delete is idempotent, end state achieved
            - This implementation chooses 404 for clarity
            
            Idempotency Consideration:
            - DELETE should be idempotent in HTTP semantics
            - Deleting already-deleted resource should succeed
            - 404 approach makes client aware of resource state
            
            Alternative Implementation:
            ```csharp
            if (game == null) {
                return Results.NoContent(); // Idempotent approach
            }
            ```
            */
            
            // Remove game from collection
            games.Remove(game);
            /*
            Collection Removal:
            
            List<T>.Remove() Method:
            - Removes first occurrence of the specified item
            - Uses equality comparison (records have value equality)
            - Returns true if item was found and removed
            - O(n) time complexity for List<T>
            
            Equality for Records:
            - Records automatically implement value-based equality
            - Two records with same property values are considered equal
            - Remove() will find the matching record correctly
            
            Production Database Equivalent:
            ```csharp
            var entity = await context.Games.FindAsync(id);
            if (entity != null) {
                context.Games.Remove(entity);
                await context.SaveChangesAsync();
            }
            ```
            
            Soft Delete Alternative:
            Instead of removing, mark as deleted:
            ```csharp
            game = game with { IsDeleted = true };
            ```
            */
            
            // Return deleted game
            return Results.Ok(game);
            /*
            Delete Response Options:
            
            Results.Ok(game) - Chosen Approach:
            - Returns HTTP 200 OK with deleted resource
            - Provides confirmation of what was deleted
            - Useful for undo operations or audit logging
            - Client gets final state of deleted resource
            
            Alternative Response Patterns:
            1. Results.NoContent(): 
               - HTTP 204 No Content (no response body)
               - More RESTful according to some interpretations
               - Less informative for client
            
            2. Results.Accepted():
               - HTTP 202 Accepted (for async deletion)
               - Used when deletion is queued/background process
            
            Response Example:
            ```json
            HTTP 200 OK
            {
              "id": 42,
              "name": "Deleted Game",
              "genre": "Action",
              "price": 19.99,
              "releaseDate": "2020-01-01"  
            }
            ```
            
            Client Benefits:
            - Confirmation of deletion
            - Data for undo operations
            - Audit trail information
            - Error detection (wrong resource deleted)
            
            Business Considerations:
            - Some applications need soft deletes (mark as deleted)
            - Others need hard deletes (permanent removal)
            - Audit requirements may dictate response format
            - GDPR compliance might require data removal confirmation
            */
        });

        // Return the configured route group
        return group;
        /*
        Method Return Value:
        
        Returning RouteGroupBuilder:
        - Enables method chaining on the result
        - Allows additional configuration after endpoint mapping
        - Follows fluent API pattern
        - Maintains consistency with ASP.NET Core conventions
        
        Usage Examples:
        ```csharp
        app.MapGamesEndpoints()
           .RequireAuthorization()     // Add auth to all endpoints
           .WithOpenApi()              // Add OpenAPI documentation
           .WithTags("Games");         // Group endpoints in Swagger UI
        ```
        
        Extension Pattern Benefits:
        1. **Composability**: Can chain multiple configuration methods
        2. **Flexibility**: Caller can add additional configuration
        3. **Consistency**: Follows ASP.NET Core fluent API patterns
        4. **Discoverability**: IntelliSense shows available options
        */
    }
}

/*
Complete API Design Analysis:

REST API Patterns Implemented:
1. **Resource-Based URLs**: /games, /games/{id}
2. **HTTP Verbs**: GET (read), POST (create), PUT (update), DELETE (delete)
3. **Status Codes**: 200 OK, 201 Created, 404 Not Found, 400 Bad Request
4. **Content Negotiation**: JSON request/response bodies
5. **Idempotency**: GET, PUT, DELETE are idempotent operations
6. **Location Headers**: CreatedAtRoute provides resource location

CRUD Operations Mapping:
- Create: POST /games (CreateGameDTO → GameDTO)
- Read: GET /games, GET /games/{id} (→ GameDTO)
- Update: PUT /games/{id} (UpdateGameDTO → GameDTO)  
- Delete: DELETE /games/{id} (→ GameDTO)

Data Flow Architecture:
1. **Request**: HTTP request with JSON body
2. **Model Binding**: JSON → DTO objects
3. **Validation**: Data Annotations automatically checked
4. **Business Logic**: Endpoint handlers process requests
5. **Data Access**: In-memory list operations (database in production)
6. **Response**: DTO objects → JSON HTTP response

Error Handling Strategy:
- 400 Bad Request: Invalid input data (validation failures)
- 404 Not Found: Resource doesn't exist
- 200 OK: Successful operations
- 201 Created: Successful resource creation

Production Enhancements Needed:
1. **Database Integration**: Replace in-memory list with EF Core
2. **Authentication/Authorization**: Secure endpoints appropriately
3. **Pagination**: Handle large datasets efficiently
4. **Logging**: Add structured logging for monitoring
5. **Error Handling**: Global exception handling middleware
6. **Validation**: Custom validation rules beyond Data Annotations
7. **Caching**: Add response caching for performance
8. **Rate Limiting**: Prevent API abuse
9. **API Versioning**: Handle API evolution over time
10. **Documentation**: OpenAPI/Swagger integration

This implementation demonstrates solid API design principles while remaining
simple enough for learning and development. The extension method pattern
promotes clean code organization and follows ASP.NET Core conventions.
*/