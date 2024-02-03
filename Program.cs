using AirportLogic.Services;
using AirportWebApi;
using AirportWebApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true);

string connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"]!;
builder.Services.AddDbContext<AirportContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddTransient<Logic>();
builder.Services.AddTransient<Locations>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddSignalR();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AirportContext>();
    ctx.Database.EnsureDeleted();
    ctx.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(builder =>
{
    builder.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("http://localhost:5173").WithOrigins("http://localhost:5174").WithOrigins("https://localhost:7130")
    .AllowCredentials();
});

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    // Map the hub endpoint
    endpoints.MapHub<FlightUpdateHub>("/flightUpdateHub");

    // Map other endpoints
    endpoints.MapControllers();
});

app.Run();
