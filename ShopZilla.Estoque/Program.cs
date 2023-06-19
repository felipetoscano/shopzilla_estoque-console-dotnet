using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopZilla.Estoque.Dal;
using ShopZilla.Estoque.Models;
using ShopZilla.Estoque.Services;

namespace ShopZilla.Estoque
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
            var configuration = configurationBuilder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configuration);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var kafkaConsumer = serviceProvider.GetRequiredService<KafkaConsumerService>();
            var cts = new CancellationTokenSource();
            kafkaConsumer.ConsumirNovosPedidos(cts.Token);
        }

        private static void ConfigureServices(IServiceCollection services, IConfigurationRoot config)
        {
            services.AddDbContext<EstoqueDbContext>(options => options.UseSqlServer(config.GetConnectionString("EstoqueDb")));
            services.AddScoped<EstoqueDal>();
            services.AddSingleton(config.GetSection(nameof(ConnectionStrings)).Get<ConnectionStrings>());
            services.AddSingleton(config.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>());
            services.AddSingleton<KafkaConsumerService>();
            services.AddSingleton<KafkaProducerService>();
        }
    }
}