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
                // Configura la URL para que escuche en todas las interfaces de red (0.0.0.0) y en el puerto 8080
                var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
                webBuilder.UseUrls($"http://0.0.0.0:{port}");
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
        if (env.IsDevelopment())
        {
            // Middleware para mostrar errores en desarrollo
            app.UseDeveloperExceptionPage();

            // Middleware para habilitar Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Allku API v1");
            });
        }

        // Middleware para redirigir HTTP a HTTPS
        app.UseHttpsRedirection();

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