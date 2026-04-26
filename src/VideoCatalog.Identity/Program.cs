using Microsoft.EntityFrameworkCore;
using VideoCatalog.Identity.Data;
using VideoCatalog.Identity;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await context.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    // No Swagger for identity service
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
