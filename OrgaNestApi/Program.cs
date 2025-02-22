using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpLogging;
using OrgaNestApi.Common.Domain;
using OrgaNestApi.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; });

builder.Services.AddCustomServices();

builder.Services.AddDbContexts(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddIdentityServices();

var secretKey = Environment.GetEnvironmentVariable("Jwt__SecretKey")
                ?? builder.Configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey not found");

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

// Register Swagger services
builder.Services.AddEndpointsApiExplorer(); // Adds necessary services for OpenAPI
builder.Services.AddSwaggerGen(); // Register Swagger generator

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomMiddleware();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();