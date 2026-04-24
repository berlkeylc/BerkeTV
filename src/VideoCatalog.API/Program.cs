using Microsoft.EntityFrameworkCore;
using VideoCatalog.Application.Features.Videos.Queries;
using VideoCatalog.Domain.Interfaces;
using VideoCatalog.Infrastructure.Data;
using VideoCatalog.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure
builder.Services.AddDbContext<VideoCatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllVideosQuery).Assembly));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-create database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VideoCatalogDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
