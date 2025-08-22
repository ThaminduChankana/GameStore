// Using directives - these import namespaces that contain classes and methods we need
// Think of these as "include" statements that give us access to pre-built functionality
using GameStore.Api.Data;      // Imports our custom data layer classes (likely contains GameStoreContext)
using GameStore.Api.Endpoints; // Imports our custom endpoint configuration classes (likely contains MapGamesEndpoints extension method)

// WebApplicationBuilder Creation - This is the foundation of our ASP.NET Core application
// CreateBuilder() is a factory method that creates a pre-configured builder with sensible defaults
var builder = WebApplication.CreateBuilder(args); // args contains command line arguments passed to the application

// What CreateBuilder() sets up automatically:
// 1. Kestrel web server configuration (the HTTP server that will handle requests)
// 2. Default logging providers (Console, Debug, EventSource, EventLog on Windows)
// 3. Configuration system (appsettings.json, environment variables, user secrets, command line args)
// 4. Dependency Injection container (used for managing object lifetimes and dependencies)
// 5. Default web host environment detection (Development, Staging, Production)

// The builder follows the Builder Pattern - it allows us to configure various aspects
// of our application before actually creating the WebApplication instance
// This separation allows for flexible configuration and testing

// Database Configuration Section
// Connection strings are typically stored in configuration files (appsettings.json) for security and flexibility
// This allows different connection strings for different environments (dev, staging, production)
var connString = builder.Configuration.GetConnectionString("GameStore"); 
// GetConnectionString() is a convenience method that looks for "ConnectionStrings:GameStore" in configuration
// It searches through the configuration hierarchy: appsettings.json, environment variables, user secrets, etc.
// Configuration hierarchy (in order of precedence):
// 1. Command line arguments
// 2. Environment variables  
// 3. User secrets (development only)
// 4. appsettings.{Environment}.json
// 5. appsettings.json

// Dependency Injection Registration
// AddSqlite<T>() registers Entity Framework Core with SQLite provider in the DI container
builder.Services.AddSqlite<GameStoreContext>(connString);
// Why we use Dependency Injection:
// 1. Loose coupling - classes don't create their own dependencies
// 2. Testability - we can easily inject mock dependencies for testing
// 3. Lifetime management - DI container manages when objects are created and disposed
// 4. Configuration - dependencies can be configured in one place

// GameStoreContext is likely our Entity Framework DbContext class that:
// 1. Represents a session with the database
// 2. Contains DbSet<T> properties for each entity (table)
// 3. Handles database connection management
// 4. Provides change tracking and transaction support

// Build the Application Instance
// Build() creates the actual WebApplication instance with all configured services and middleware
var app = builder.Build(); 
// At this point:
// 1. All services are registered and the DI container is finalized (no more services can be added)
// 2. The HTTP request pipeline is ready to be configured
// 3. Kestrel server is configured but not yet started
// 4. The application is ready to have middleware and endpoints configured

// Endpoint Configuration
// MapGamesEndpoints() is an extension method (defined in GameStore.Api.Endpoints namespace)
app.MapGamesEndpoints(); 
// This method likely contains code like:
// app.MapGet("/games", GetAllGames);
// app.MapPost("/games", CreateGame);
// app.MapPut("/games/{id}", UpdateGame);
// app.MapDelete("/games/{id}", DeleteGame);

// Extension methods for endpoint mapping promote:
// 1. Organization - keeps related endpoints grouped together
// 2. Reusability - endpoint groups can be easily added to multiple applications
// 3. Separation of concerns - endpoint logic is separate from application startup logic
// 4. Maintainability - easier to find and modify specific endpoint configurations

// Start the Application
app.Run(); 
// Run() starts the web server and begins listening for HTTP requests
// This is a blocking call - the application will continue running until:
// 1. The process is terminated (Ctrl+C, process kill, etc.)
// 2. IHostApplicationLifetime.StopApplication() is called
// 3. An unhandled exception occurs that crashes the application

// What happens when Run() is called:
// 1. Kestrel server starts listening on configured ports (default: 5000 for HTTP, 5001 for HTTPS)
// 2. The HTTP request pipeline is activated and ready to process requests
// 3. Background services (if any) are started
// 4. Application lifetime events are triggered (ApplicationStarted)

// Request Pipeline Overview:
// When an HTTP request comes in, it flows through the middleware pipeline:
// 1. Routing middleware determines which endpoint should handle the request
// 2. Authentication/Authorization middleware (if configured)
// 3. CORS middleware (if configured)
// 4. Exception handling middleware (if configured)
// 5. Finally reaches the mapped endpoint (from MapGamesEndpoints)

// Application Architecture Summary:
// This is a minimal API application following these patterns:
// 1. Dependency Injection - for managing dependencies and object lifetimes
// 2. Configuration Pattern - for managing application settings
// 3. Extension Methods - for organizing and reusing configuration code
// 4. Entity Framework Core - for data access and database operations
// 5. Minimal APIs - for lightweight HTTP API creation without heavy MVC overhead

// Bootstrap Process Summary:
// 1. Create builder with default configuration
// 2. Register services (database context, custom services)
// 3. Build application instance
// 4. Configure request pipeline (middleware, endpoints)
// 5. Start server and begin processing requests