using Microsoft.EntityFrameworkCore;
using MoviesBooks.Api.Data;

// ============================================================
// Program.cs — MoviesBooks.Api
// This file uses "top-level statements": there is no Main() method.
// The compiler wraps everything below in one, in order.
// Rule: all statements first, type declarations (record/class) last.
// ============================================================

// Creates the host builder. This is where configuration is loaded
// (appsettings.json, appsettings.Development.json, user-secrets,
// environment variables, command-line args) and where the dependency
// injection (DI) container begins. Everything between here and
// builder.Build() is REGISTRATION: "here are the services my app needs."
var builder = WebApplication.CreateBuilder(args);

// Registers a service that scans the app's endpoints so the Swagger
// generator can discover them. Minimal APIs (MapGet etc.) need this;
// controller-based apps get it implicitly.
builder.Services.AddEndpointsApiExplorer();

// Registers the Swashbuckle generator that turns the discovered endpoints
// into an OpenAPI/Swagger JSON document. Registered here, produced later
// on request at /swagger/v1/swagger.json.
builder.Services.AddSwaggerGen();

// Registers AppDbContext in the DI container as a scoped service (one per
// HTTP request) and tells it to use SQL Server with the connection string
// named "Default" — which in Development resolves from user-secrets.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// THE DIVIDING LINE.
// Before: describe what you need (services). After: define how requests
// flow (middleware + endpoints). Once built, the DI container is sealed
// and no more services can be registered.
var app = builder.Build();

// The environment name comes from the ASPNETCORE_ENVIRONMENT variable.
// Properties/launchSettings.json sets it to "Development" for dotnet run.
// Dev-only tooling goes inside this block so production doesn't expose
// the API's internals.
if (app.Environment.IsDevelopment())
{
    // Middleware that serves the generated OpenAPI JSON document at
    // /swagger/v1/swagger.json. This is the raw data; not meant for humans.
    app.UseSwagger();

    // Middleware that serves the Swagger UI web page at /swagger.
    // It reads the JSON above and renders the clickable "Try it out" page.
    app.UseSwaggerUI();
}

// Middleware that redirects http:// requests to https://.
// Middleware is a pipeline: each request passes through these in order,
// so ordering will matter when CORS and auth are added later.
app.UseHttpsRedirection();

// A plain string[] created once at startup, not per request.
// Same idea as registering a DbContext once instead of newing it up
// inside every handler.
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// A minimal-API endpoint. MapGet says: "when a GET arrives at this path,
// run this lambda and serialize whatever it returns to JSON."
// Step 7 will use this exact shape with a database instead of Random.
app.MapGet("/weatherforecast", () =>
{
    // LINQ. Range(1, 5) yields 1..5. Select transforms each number into a
    // WeatherForecast. Nothing runs until ToArray() forces it; before that
    // it is just a recipe. In Step 7, db.Movies.Select(...) is a recipe
    // that EF Core translates to SQL, and ToListAsync() is what runs it.
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            // Random.Shared is a thread-safe app-wide instance. Web servers
            // handle requests concurrently, so a shared plain Random would
            // be a bug.
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
// Attaches metadata to the endpoint. Becomes the operationId in the
// OpenAPI document; Swagger displays it, and client generators (used in
// Phase 3 for React) use it as the method name.
.WithName("GetWeatherForecast");

// Starts Kestrel (the web server) and blocks until shutdown (Ctrl+C).
// Nothing above this line handles a request; this is where listening begins.
app.Run();

// A positional record: the compiler generates the constructor, read-only
// properties, value equality, and ToString(). Ideal shape for a DTO
// ("a bundle of data going out the door").
// string? = "may be null" (nullable reference annotation). DateOnly and int
// have no ? because value types cannot be null anyway.
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    // Computed property (expression-bodied). Recalculated on every read and
    // still serialized to JSON, so the client gets F without storing it.
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}