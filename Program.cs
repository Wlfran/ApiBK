using Microsoft.Extensions.FileProviders;
using Social_Module.Services.Interface;
using Social_Module.Services.Social;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalAndProd", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                var uri = new Uri(origin);
                return uri.Host == "localhost";
            })
            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS SIEMPRE antes de static + controllers
app.UseCors("AllowLocalAndProd");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
    ),
    RequestPath = "",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "http://localhost:4200";
        ctx.Context.Response.Headers["Access-Control-Allow-Headers"] = "*";
        ctx.Context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, OPTIONS";
    }
});

app.UseAuthorization();

app.MapControllers();

app.Run();
