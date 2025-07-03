using Microsoft.EntityFrameworkCore;
using Plane_Crash_Visualization.Data;
using Plane_Crash_Visualization.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddDbContext<Plane_Crash_Visualization.Data.CrashDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HTTP client and geocoding service
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>();
builder.Services.AddScoped<IGeocodingService, GeocodingService>();

// Add aircraft analysis services
builder.Services.AddScoped<AircraftTypeService>();
builder.Services.AddScoped<DatabaseUpdateService>();
builder.Services.AddScoped<IContinentService, ContinentService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
