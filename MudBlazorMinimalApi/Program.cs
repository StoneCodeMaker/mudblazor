using Microsoft.Data.SqlClient;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy for Blazor WebAssembly app
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5150")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Enable CORS
app.UseCors("BlazorPolicy");

app.MapGet("/", () => "Minimal API is running!");

// Add endpoint for Country data
app.MapGet("/api/country", async () =>
{
    var countries = new List<CountryModel>();
    
    try
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            return Results.BadRequest("Database connection string is not configured.");
        }

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand("SELECT Country_ID, Name, Verfied_Mode_ID FROM Country", connection);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                countries.Add(new CountryModel
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Verified = reader.GetInt16(2) == 1
                });
            }
        }

        return Results.Ok(countries);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching countries: {ex.Message}");
        // Provide mock data as a fallback for testing
        return Results.Ok(new List<CountryModel>
        {
            new CountryModel { Id = 1, Name = "United States (Mock)", Verified = true },
            new CountryModel { Id = 2, Name = "Canada (Mock)", Verified = true },
            new CountryModel { Id = 3, Name = "Mexico (Mock)", Verified = true },
            new CountryModel { Id = 4, Name = "United Kingdom (Mock)", Verified = true },
        });
    }
});

app.Run();

class CountryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Verified { get; set; }
}
