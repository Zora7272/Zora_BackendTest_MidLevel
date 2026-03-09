using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Services;
using WebApplication1.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Register DbContext (update the connection string in appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// register service
builder.Services.AddScoped<IAcpdService, AcpdService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Include XML comments if available (enable XML docs generation in csproj)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    c.SchemaFilter<SampleSchemaFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger / SwaggerUI for all environments so APIs can be explored and tested
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Serve Swagger UI at /swagger so /swagger/index.html works
    c.RoutePrefix = "swagger";
    // Explicit endpoint for the generated swagger JSON
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
