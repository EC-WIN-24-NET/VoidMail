using System.Reflection;
using Azure.Identity;
using Core;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCoreServices();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Let's add support for Entity Framework Core and connect to a database
// --- Add Azure Key Vault configuration ---
var keyVaultUri = builder.Configuration["EcWin24KeyVaultUri"];

if (
    !string.IsNullOrEmpty(keyVaultUri)
    && Uri.TryCreate(keyVaultUri, UriKind.Absolute, out var validUri)
)
{
    builder.Configuration.AddAzureKeyVault(validUri, new DefaultAzureCredential());
}
else
{
    Console.WriteLine("Key Vault URI is not valid");
}

// --- End of Azure Key Vault configuration ---

// https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0&tabs=visual-studio
builder.Services.AddSwaggerGen(c =>
{
    // Adding a new Swagger document
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "VoidMail API",
            Version = "v1",
            Description = "An simple API for Email sending",
            Contact = new OpenApiContact()
            {
                Name = "Georgi Sundberg",
                Email = "georgi.sundberg@outlook.com",
                Url = new Uri("https://www.google.se"),
            },
        }
    );

    // Adding XML comments to Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Adding support for CORS
builder.Services.AddCors(options =>
{
    // Adding a new policy
    options.AddPolicy(
        "AllowAllNoSecurity",
        policy =>
        {
            // Minimal without security...duh
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    );
});

// Registering the CORS policy
var app = builder.Build();

// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Adding a new Swagger endpoint
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Forge API v1");
        // Adding OAuth support
        c.OAuthClientId("swagger-ui");
        // Adding OAuth scopes
        c.OAuthScopes("api1", "api2");
    });

    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Enable CORS after the HTTPS redirection
app.UseCors("AllowAllNoSecurity");

app.UseAuthorization();

app.MapControllers();

app.Run();
