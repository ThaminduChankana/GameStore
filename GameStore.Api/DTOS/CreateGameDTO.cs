// Using directive for Data Annotations namespace
using System.ComponentModel.DataAnnotations;
// This namespace provides attributes for validation and metadata
// Data Annotations are used for:
// 1. Model validation (ensuring data meets business rules)
// 2. Database schema generation (when using Code First)
// 3. UI scaffolding (automatic form generation)
// 4. API documentation (OpenAPI/Swagger integration)

// Namespace declaration following project structure convention
namespace GameStore.Api.DTOS;
// DTOs (Data Transfer Objects) namespace contains classes used for data transfer
// This follows the separation of concerns principle by isolating data contracts
// from business entities and database models

/*
Data Transfer Object (DTO) Pattern Overview:
DTOs are objects specifically designed to carry data between application layers or systems.
They serve as contracts that define what data should be transferred and in what format.

Key purposes of DTOs:
1. **Decoupling**: Separate internal entity structure from external API contracts
2. **Security**: Prevent over-posting attacks by controlling what data is accepted
3. **Versioning**: API contracts can evolve independently from internal models
4. **Validation**: Centralized place to define input validation rules
5. **Performance**: Can reduce payload size by including only necessary fields
6. **Documentation**: Self-documenting API contracts
*/

// Record class declaration with primary constructor and validation attributes
public record class CreateGameDTO(
    /*
    Record Class Explanation:
    Records are a C# 9+ feature designed for immutable data containers.
    They are perfect for DTOs because:
    
    1. **Immutability**: Once created, properties cannot be changed (thread-safe)
    2. **Value Equality**: Two records with same values are considered equal
    3. **Concise Syntax**: Less boilerplate code than traditional classes
    4. **Built-in Methods**: Automatic ToString(), GetHashCode(), Equals() implementations
    5. **with Expression**: Easy creation of modified copies
    6. **Deconstruction**: Can be deconstructed into individual variables
    
    Primary Constructor:
    The parameters in parentheses after the record name become:
    - Public properties with init-only setters
    - Constructor parameters
    - Backing fields for the properties
    
    This eliminates the need for:
    - Manual property declarations
    - Constructor implementation
    - Backing field declarations
    */
    
    // Name property with validation attributes
    [Required] [StringLength(50)] string Name,
    /*
    [Required] Attribute:
    - Ensures the Name property cannot be null or empty
    - Validation occurs automatically during model binding in ASP.NET Core
    - If validation fails, ModelState.IsValid becomes false
    - API returns 400 Bad Request with validation error details
    
    [StringLength(50)] Attribute:
    - Limits the string length to maximum 50 characters
    - Prevents excessively long input that could cause database issues
    - Also used by Entity Framework for database column length constraints
    - Can optionally specify minimum length: [StringLength(50, MinimumLength = 3)]
    
    Why validate Name:
    1. Database constraints (VARCHAR(50) column limit)
    2. User interface limitations (display area constraints)
    3. Business rules (game names shouldn't be too long for marketing)
    4. Security (prevent potential buffer overflow or display issues)
    5. Data consistency (standardized name lengths across the system)
    */
    
    // Genre property with validation attributes
    [Required] [StringLength(20)] string Genre,
    /*
    Genre Validation Logic:
    - [Required]: Ensures every game has a categorization
    - [StringLength(20)]: Keeps genre names concise and consistent
    
    Design Considerations:
    1. **Reference Data**: In a real application, genres might be a separate entity
    2. **Standardization**: Limited length encourages standard genre names
    3. **UI Constraints**: Dropdown menus work better with shorter genre names
    4. **Database Normalization**: Could be foreign key to Genres table
    
    Alternative Approaches:
    - Enum: public enum GameGenre { Action, RPG, Strategy, ... }
    - Foreign Key: int GenreId with separate Genre entity
    - Validation Attribute: [RegularExpression] to limit to predefined values
    */
    
    // Price property with validation attributes
    [Required] [Range(1, 100)] decimal Price,
    /*
    [Required] for Value Types:
    - decimal is a value type, so it can't be null by default
    - [Required] ensures the value is explicitly provided (not default(decimal))
    - Prevents accidental submission of 0 price due to missing data
    
    [Range(1, 100)] Attribute:
    - Minimum value: 1 (games must have a positive price, no free games in this model)
    - Maximum value: 100 (reasonable upper limit for game pricing)
    - Prevents negative prices or unrealistic pricing
    
    decimal vs double/float for Price:
    - decimal: Exact decimal representation, no rounding errors (perfect for money)
    - double/float: Approximate representation, can cause rounding issues
    - Financial applications should always use decimal for monetary values
    
    Business Rules Encoded:
    1. No free games (minimum $1)
    2. Premium pricing cap ($100 maximum)
    3. Prevents data entry errors (negative prices, astronomical prices)
    4. Ensures realistic pricing for business logic
    
    Validation Error Examples:
    - Price = 0: "The field Price must be between 1 and 100."
    - Price = 150: "The field Price must be between 1 and 100."
    - Price = -5: "The field Price must be between 1 and 100."
    */
    
    // Release date property without validation (optional field)
    DateOnly ReleaseDate
    /*
    DateOnly Type (C# 10+):
    - Represents just a date without time component
    - Perfect for release dates where time is irrelevant
    - More semantically correct than DateTime for date-only scenarios
    - Prevents confusion about time zones and time components
    
    No Validation Attributes:
    - Not marked [Required], so it's optional in API requests
    - No range validation - release dates can be in past or future
    - Could add validation if needed: [Range(typeof(DateOnly), "1970-01-01", "2030-12-31")]
    
    Design Decisions:
    1. **Optional Field**: Games might not have confirmed release dates
    2. **Historical Data**: Old games need accurate release dates
    3. **Future Releases**: Upcoming games can have planned release dates
    4. **Date Only**: Time is irrelevant for game release dates
    
    Alternative Validation Approaches:
    - Custom Validation: Ensure date is not too far in future
    - Business Rule: Prevent dates before video games existed (1950s+)
    - Format Validation: Ensure proper date format in API requests
    */
);

/*
How This DTO is Used in the Application Flow:

1. **API Request**: Client sends POST request to create a game
   Example JSON:
   {
     "name": "Elden Ring",
     "genre": "Action RPG", 
     "price": 59.99,
     "releaseDate": "2022-02-25"
   }

2. **Model Binding**: ASP.NET Core automatically deserializes JSON to CreateGameDTO
   - JSON property names are matched to DTO properties (case-insensitive)
   - Type conversion happens automatically (string "59.99" → decimal 59.99)

3. **Validation**: Data Annotations are automatically validated
   - [Required] checks for null/empty values
   - [StringLength] checks string length constraints  
   - [Range] checks numeric range constraints
   - If any validation fails, 400 Bad Request is returned with error details

4. **Business Logic**: Valid DTO is passed to endpoint handler
   - DTO data is used to create internal entities
   - Mapping occurs from CreateGameDTO → Game entity → Database

5. **Response**: Created game is returned (usually as GameDTO)
   - Different DTO type for response (includes generated ID)
   - Follows DTO pattern of specific types for specific operations

Validation Flow Example:
```
Request: { "name": "", "genre": "RPG", "price": 150, "releaseDate": "2024-01-01" }

Validation Results:
- Name: FAIL - [Required] violation (empty string)
- Genre: PASS - [Required] and [StringLength(20)] satisfied  
- Price: FAIL - [Range(1,100)] violation (150 > 100)
- ReleaseDate: PASS - No validation attributes

API Response: 400 Bad Request
{
  "errors": {
    "Name": ["The Name field is required."],
    "Price": ["The field Price must be between 1 and 100."]
  }
}
```

Benefits of This DTO Design:
1. **Type Safety**: Compile-time checking of property types
2. **Automatic Validation**: No manual validation code needed
3. **Self-Documenting**: Validation rules are clear from attributes
4. **API Documentation**: Swagger/OpenAPI automatically documents constraints
5. **Immutability**: Thread-safe, prevents accidental modifications
6. **Performance**: Minimal memory allocation, efficient copying
7. **Maintainability**: Single place to change validation rules
8. **Security**: Prevents over-posting by limiting input fields
*/