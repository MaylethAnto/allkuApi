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
                // Configura la URL para que escuche en todas las interfaces de red (0.0.0.0) y en el puerto 5000
                webBuilder.UseUrls("http://0.0.0.0:5138");  // O puedes cambiar el puerto si lo deseas
                webBuilder.UseStartup<Startup>();
            });

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Configurar conexi�n a SQLServer
        services.AddDbContext<AllkuDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // Inyecci�n de dependencias
        services.AddScoped<AutenticacionService>();
        services.AddScoped<HashService>();

        // Configuraci�n de CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllkuPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        services.AddControllers();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("AllkuPolicy");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}