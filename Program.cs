using Social_Module.Services.Interface;
using Social_Module.Services.Social;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalAndProd", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "https://tu-dominio-produccion.com") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); 
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<SocialService>();
builder.Services.AddScoped<EmpresasService>();
builder.Services.AddScoped<ISocialEjecucionService, SocialEjecucionService>();



var app = builder.Build();

// Middleware de Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// **Importante: el orden**
app.UseCors("AllowLocalAndProd"); 

app.UseAuthorization();

app.MapControllers();

app.Run();
