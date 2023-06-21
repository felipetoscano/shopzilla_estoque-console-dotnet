using Microsoft.EntityFrameworkCore;
using ShopZilla.Estoque;
using ShopZilla.Estoque.Dal;
using ShopZilla.Estoque.Models;
using ShopZilla.Estoque.Services;
using ShopZilla.Estoque.Services.BackgroundServices;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddDbContext<EstoqueDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("EstoqueDb")));

        services.AddSingleton(configuration.GetSection(nameof(ConnectionStrings)).Get<ConnectionStrings>());
        services.AddSingleton(configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>());
        services.AddSingleton<KafkaProducerService>();

        services.AddScoped<EstoqueDal>();

        services.AddHostedService<KafkaConsumerService>();
    })
.Build().Run();
