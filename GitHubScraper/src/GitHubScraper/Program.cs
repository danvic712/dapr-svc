using System.Reflection;
using Microsoft.OpenApi.Models;
using PuppeteerSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBrowserFetcher, BrowserFetcher>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "GitHub Scraper API",
        Description = "An ASP.NET Core Web API for scrapping GitHub resources",
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://github.com/danvic712/github-scraper/blob/main/LICENSE")
        }
    });
    
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Generate lowercase URLs
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "GitHub Scraper API v1");
    options.RoutePrefix = string.Empty;
});

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();