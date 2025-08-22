// Using directive for Data Annotations validation attributes
using System.ComponentModel.DataAnnotations;
// Required for validation attributes like [Required], [StringLength], [Range]
// These provide automatic model validation in ASP.NET Core applications
// Same namespace imported as CreateGameDTO since update operations need validation too

// Namespace declaration consistent with other DTOs
namespace GameStore.Api.DTOS;
// Grouped with other Data Transfer Objects for logical organization
// Maintains consistency in project structure and makes DTOs easy to locate

// Record class declaration for update operations
public record class UpdateGameDTO
(
    /*
    UpdateGameDTO Design Philosophy:
    
    This DTO represents the "Update" operation in CRUD (Create, Read, Update, Delete).
    It's specifically designed for HTTP PUT requests where clients send complete
    replacement data for an existing resource.
    
    Key Design Decisions:
    1. **Include ID**: Unlike CreateGameDTO, this includes the ID for identification
    2. **Complete Replacement**: All fields included for full entity replacement
    3. **Validation**: Same validation as CreateGameDTO to ensure data integrity
    4. **Immutability**: Record type ensures the DTO cannot be modified after creation
    
    Alternative Update Patterns:
    - PATCH (partial updates): Would need different DTO with nullable properties
    - Delta objects: Track only changed properties
    - Command objects: Separate DTOs for each type of update operation
    
    This implementation follows the PUT semantics of HTTP:
    - Client provides complete representation of the resource
    - Server replaces entire resource with provided data
    - Idempotent operation (same request multiple times has same effect)
    */
    
    // Resource identifier property
    int Id,
    /*
    Id Property in Update Context:
    
    Purpose and Usage:
    1. **Resource Identification**: Identifies which game to update
    2. **URL Binding**: Often bound from route parameter (/games/{id})
    3. **Consistency Check**: Can verify URL ID matches body ID
    4. **Database Operation**: Used in WHERE clause of UPDATE statement
    
    Why Include ID in Body:
    1. **Complete Representation**: PUT should include all resource data
    2. **Validation**: Ensures client knows which resource they're updating
    3. **Consistency**: Prevents mismatched route parameter and body data
    4. **API Design**: Makes the operation more explicit and self-contained
    
    Alternative Approaches:
    - Route Only: Take ID from URL route parameter only
    - Header: Pass ID in custom header (less RESTful)
    - Separate Parameter: Keep ID separate from DTO in controller action
    
    Security Considerations:
    - ID tampering: Client could change ID to update different resource
    - Authorization: Must verify client can update the specific game ID
    - Validation: Should match route parameter ID for consistency
    
    Implementation Pattern:
    ```csharp
    app.MapPut("/games/{id}", (int id, UpdateGameDTO updateGame) => {
        if (id != updateGame.Id) {
            return Results.BadRequest("Route ID and body ID must match");
        }
        // Proceed with update...
    });
    ```
    */
    
    // Game name with validation constraints
    [Required] [StringLength(50)] string Name,
    /*
    Name Validation (Update Context):
    
    Validation Attributes Explained:
    - [Required]: Prevents null, empty string, or whitespace-only values
    - [StringLength(50)]: Maximum length constraint for database compatibility
    
    Why Validate on Updates:
    1. **Data Integrity**: Ensure updated data meets business rules
    2. **Database Constraints**: Prevent database errors from constraint violations
    3. **Consistency**: Same validation rules as creation for consistency
    4. **User Experience**: Early validation provides immediate feedback
    
    Business Rules Enforced:
    - Games must have meaningful names (not empty)
    - Names must fit in UI components and database columns
    - Consistent naming standards across all operations
    
    Update-Specific Considerations:
    - Original name might have been valid under old rules
    - New validation might be stricter than when record was created
    - Need to handle legacy data that might not meet current standards
    
    Validation Error Handling:
    If validation fails:
    ```json
    HTTP 400 Bad Request
    {
      "errors": {
        "Name": ["The Name field is required."]
      }
    }
    ```
    */
    
    // Game genre with validation constraints
    [Required] [StringLength(20)] string Genre,
    /*
    Genre Validation Strategy:
    
    Current Implementation:
    - String field with length constraint
    - Required to ensure proper categorization
    - 20 character limit for UI and database efficiency
    
    Alternative Validation Approaches:
    1. **Enum Validation**: 
       ```csharp
       [Required] [EnumDataType(typeof(GameGenre))] string Genre
       ```
    
    2. **Custom Validation**:
       ```csharp
       [Required] [AllowedGenres("Action", "RPG", "Strategy")] string Genre
       ```
    
    3. **Foreign Key Validation**:
       ```csharp
       [Required] [Range(1, int.MaxValue)] int GenreId
       ```
    
    Update Scenarios:
    - Genre standardization: "Action-Adventure" → "Action"
    - New genre categories: Adding support for new genres
    - Genre merging: Combining similar genres
    
    Data Migration Considerations:
    - What happens to games with deprecated genres?
    - How to handle genre updates that affect multiple games?
    - Maintaining referential integrity during genre changes
    */
    
    // Game price with validation constraints
    [Required] [Range(1, 100)] decimal Price,
    /*
    Price Validation (Update Context):
    
    Range Validation Logic:
    - Minimum $1: Prevents free games, ensures revenue model
    - Maximum $100: Reasonable upper bound for pricing strategy
    - decimal type: Exact monetary calculation without floating-point errors
    
    Update-Specific Price Considerations:
    1. **Price Changes**: Games can be discounted or have price increases
    2. **Market Dynamics**: Prices may need adjustment based on demand
    3. **Currency Fluctuations**: Might need price updates for international markets
    4. **Promotional Pricing**: Temporary price changes for sales events
    
    Business Rules for Price Updates:
    - No negative prices (business logic)
    - Reasonable maximum to prevent data entry errors
    - Audit trail might be needed for price change history
    - Regional pricing variations might need different validation
    
    Price Update Patterns:
    ```csharp
    // Price increase
    updateGame = game with { Price = 29.99m };
    
    // Promotional discount
    updateGame = game with { Price = 19.99m };
    
    // Price adjustment for market conditions
    updateGame = game with { Price = 24.99m };
    ```
    
    Validation Scenarios:
    - Price = 0: "The field Price must be between 1 and 100."
    - Price = 150: "The field Price must be between 1 and 100."
    - Price = -10: "The field Price must be between 1 and 100."
    */
    
    // Release date without validation constraints
    DateOnly ReleaseDate
    /*
    ReleaseDate (Update Context):
    
    No Validation Rationale:
    - Historical accuracy: Release dates are factual, not business rules
    - Future releases: Games can have planned release dates
    - Date corrections: Sometimes release dates need correction
    - Regional variations: Different release dates in different markets
    
    Update Scenarios for Release Dates:
    1. **Delay Announcements**: Moving future release dates later
       ```csharp
       // Original: 2024-03-15, Updated: 2024-06-20
       updateGame = game with { ReleaseDate = new DateOnly(2024, 6, 20) };
       ```
    
    2. **Historical Corrections**: Fixing incorrect historical data
       ```csharp
       // Correcting historical inaccuracy
       updateGame = game with { ReleaseDate = new DateOnly(2015, 5, 19) };
       ```
    
    3. **Regional Releases**: Different dates for different markets
       ```csharp
       // JP release vs US release dates
       updateGame = game with { ReleaseDate = jpReleaseDate };
       ```
    
    DateOnly Benefits for Updates:
    - No time zone complications during updates
    - Clear semantics (date only, not datetime)
    - Consistent serialization format (ISO 8601 date)
    - No time component to worry about during comparisons
    
    Potential Validation Considerations:
    - Could add reasonable bounds (e.g., 1970-2050)
    - Might validate future dates for upcoming releases
    - Could ensure date consistency with other fields
    
    Example with optional validation:
    ```csharp
    [Range(typeof(DateOnly), "1970-01-01", "2030-12-31")]
    DateOnly ReleaseDate
    ```
    */
);

/*
Complete Update Operation Flow:

1. **Client Prepares Update**:
   - Retrieves current game data (GET /games/42)
   - Modifies desired properties
   - Sends complete updated representation

2. **HTTP PUT Request**:
   ```http
   PUT /games/42
   Content-Type: application/json
   
   {
     "id": 42,
     "name": "Elden Ring: Game of the Year Edition",
     "genre": "Action RPG",
     "price": 49.99,
     "releaseDate": "2022-02-25"
   }
   ```

3. **Model Binding and Validation**:
   - ASP.NET Core deserializes JSON to UpdateGameDTO
   - Validation attributes are automatically checked
   - ModelState populated with any validation errors

4. **Business Logic Processing**:
   ```csharp
   app.MapPut("/games/{id}", (int id, UpdateGameDTO updateGame) => {
       // Validate ID consistency
       if (id != updateGame.Id) {
           return Results.BadRequest("ID mismatch");
       }
       
       // Find existing game
       var existingGame = games.Find(g => g.Id == id);
       if (existingGame is null) {
           return Results.NotFound();
       }
       
       // Create updated version using 'with' expression
       var updatedGame = existingGame with {
           Name = updateGame.Name,
           Genre = updateGame.Genre,
           Price = updateGame.Price,
           ReleaseDate = updateGame.ReleaseDate
       };
       
       // Replace in collection
       var index = games.FindIndex(g => g.Id == id);
       games[index] = updatedGame;
       
       return Results.Ok(updatedGame);
   });
   ```

5. **Response**:
   ```http
   HTTP 200 OK
   Content-Type: application/json
   
   {
     "id": 42,
     "name": "Elden Ring: Game of the Year Edition",
     "genre": "Action RPG", 
     "price": 49.99,
     "releaseDate": "2022-02-25"
   }
   ```

Record 'with' Expression Benefits:

The 'with' expression is particularly useful for updates:
```csharp
// Creates new record with modified properties
var updatedGame = existingGame with {
    Price = updateGame.Price,  // Only change price
    Name = updateGame.Name     // And name
    // Other properties copied unchanged
};
```

Benefits:
1. **Immutability Preserved**: Original record unchanged
2. **Partial Updates**: Only specify changed properties
3. **Type Safety**: Compile-time checking of property names
4. **Performance**: Efficient copying of unchanged properties
5. **Thread Safety**: Original record safe for concurrent access

Error Handling Scenarios:

1. **Validation Errors**:
   ```json
   HTTP 400 Bad Request
   {
     "errors": {
       "Name": ["The Name field is required."],
       "Price": ["The field Price must be between 1 and 100."]
     }
   }
   ```

2. **Resource Not Found**:
   ```json
   HTTP 404 Not Found
   {
     "detail": "Game with ID 42 was not found."
   }
   ```

3. **ID Mismatch**:
   ```json
   HTTP 400 Bad Request
   {
     "detail": "Route ID and body ID must match."
   }
   ```

Design Patterns Demonstrated:

1. **Data Transfer Object**: Clean separation of API contract from domain model
2. **Immutable Updates**: Using records and 'with' expressions
3. **Input Validation**: Declarative validation with attributes
4. **HTTP Semantics**: Proper PUT operation implementation
5. **Error Handling**: Comprehensive validation and error responses
6. **Type Safety**: Strong typing throughout the update process

This UpdateGameDTO provides a robust, validated, and type-safe way to handle
game updates while maintaining immutability and clear separation of concerns.
*/