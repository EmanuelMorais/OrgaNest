using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpLogging;
using OrgaNestApi.Common.Domain;
using OrgaNestApi.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Use relative path for configuration files
string basePath = AppContext.BaseDirectory;

builder.Configuration
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Retrieve the secret key
var secretKey = Environment.GetEnvironmentVariable("Jwt__SecretKey")
                ?? builder.Configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey not found");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddCustomServices();

string dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OrgaNestApi");
Directory.CreateDirectory(dbFolder); // Ensure the folder exists

string dbPath = Path.Combine(dbFolder, "app.db"); 

builder.Services.AddDbContexts($"Data Source={dbPath}");

builder.Services.AddIdentityServices();

builder.Services.AddJwtAuthentication(
    builder.Configuration["Jwt:Issuer"],
    builder.Configuration["Jwt:Audience"],
    secretKey);

builder.Services.AddGoogleAuthentication(
    builder.Configuration["Google:ClientId"],
    builder.Configuration["Google:ClientSecret"]
);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Roles.Admin.ToString(), policy => policy.RequireRole(Roles.Admin.ToString()));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
    logging.CombineLogs = true;
});

// ✅ Add a health check for GitHub Actions
builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomMiddleware();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseCors("AllowAll");

// ✅ Map health check for CI verification
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
