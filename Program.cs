
using Block_Country_IP.Repository.IRepo;
using Block_Country_IP.Repository;
using Block_Country_IP.Utility;

namespace Block_Country_IP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory()) // Ensure correct directory
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

            builder.Services.AddHostedService<Cleanup>();
            // Add services to the container.
            builder.Services.AddHttpClient<IpGeolocationService>(client =>
            {
                client.BaseAddress = new Uri("https://api.ipgeolocation.io/ipgeo");
            });

            builder.Services.AddSingleton<ICountryRepository, CountryRepository>();
            builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
