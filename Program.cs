using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "DishesAPI",
            Version = "v1",
            Description = "An API for managing dishes and their ingredients."
        };
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter a valid JWT bearer token."
        };
        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer")] = new List<string>()
        });

        return Task.CompletedTask;
    });
});






builder.Services.AddProblemDetails();
builder.Services.AddValidation();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddAuthorizationBuilder().AddPolicy("RequiredAdminFromBelgium", policy =>
    policy.RequireAuthenticatedUser()
        .RequireRole("admin")
        .RequireClaim("country", "Belgium"));

builder.Services.AddDbContext<DishesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DishesDBConnectionString")));



// Organizing Endpoints at Scale
builder.Services.RegisterAllEndpoints();




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // opeapi/v1.json
    app.MapOpenApi();

    // Configure the HTTP request pipeline. lo usi in Development al posto di swagger
    app.MapScalarApiReference();
}
else
{
    // Configure the HTTP request pipeline. lo usi in Production
    app.UseExceptionHandler();
}

/*
 ho organizzato gli Endopoints in classi separate
 che implementano IEndpointDefinition, quindi non ho più bisogno di questo codice

app.UseHttpsRedirection();
app.UseStatusCodePages();

*/

// autenticazione  ho commentato per evitare di creare i token in fase di sviluppo
// app.UseAuthentication();
//app.UseAuthorization();




app.MapGet("/testerror", () =>
{
    throw new NotImplementedException();
});

app.MapEndpoints();

// chiamo i EndpointRouterBuilderExtensions
app.RegisterDishesEndPoints();
app.RegisterIngredientsEndPoints();

app.Run();
