using Microsoft.EntityFrameworkCore;
using NewsOzetleyici.Core.Interfaces;
using NewsOzetleyici.Data.Context;
using NewsOzetleyici.Data.Repositories;
using NewsOzetleyici.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<NewsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// HttpClient
builder.Services.AddHttpClient<IWebScrapingService, WebScrapingService>();
builder.Services.AddHttpClient<IAiSummarizationService, HuggingFaceService>();

// Repositories
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<CategoryRepository>();

// Services
builder.Services.AddScoped<IWebScrapingService, WebScrapingService>();
builder.Services.AddScoped<IAiSummarizationService, HuggingFaceService>();
builder.Services.AddScoped<NewsService>();

// CORS - Angular için
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NewsDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
