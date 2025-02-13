using AllkuApi.Data;
using AllkuApi.Services;
using Microsoft.EntityFrameworkCore;

public class Startup
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
           .ConfigureWebHostDefaults(webBuilder =>
           {
               var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
               webBuilder.UseUrls($"http://0.0.0.0:{port}", "https://0.0.0.0:443");
               webBuilder.UseStartup<Startup>();
           });

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Configurar conexión a SQL Server
        services.AddDbContext<AllkuDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // Inyección de dependencias
        services.AddScoped<AutenticacionService>();
        services.AddScoped<HashService>();

        // Configuración de CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllkuPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Configuración de controladores y Swagger
        services.AddControllers();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Habilitar Swagger siempre
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Allku API v1");
            // Hacer que Swagger UI sea la página principal
            c.RoutePrefix = string.Empty;
        });

        app.UseRouting();
        app.UseCors("AllkuPolicy");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    



    // Middleware para enrutar solicitudes
    app.UseRouting();

        // Middleware para habilitar CORS
        app.UseCors("AllkuPolicy");

        // Middleware para mapear controladores
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}