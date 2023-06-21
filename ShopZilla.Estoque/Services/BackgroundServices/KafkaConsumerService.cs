using Confluent.Kafka;
using ShopZilla.Estoque.Dal;
using ShopZilla.Estoque.Entities;
using ShopZilla.Estoque.Models;
using System.Text.Json;

namespace ShopZilla.Estoque.Services.BackgroundServices
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly KafkaProducerService _kafkaProducer;
        private readonly ConnectionStrings _connectionStrings;
        private readonly KafkaSettings _kafkaSettings;
        private readonly IServiceProvider _serviceProvider;

        public KafkaConsumerService(KafkaProducerService kafkaProducer, ConnectionStrings connectionStrings, KafkaSettings kafkaSettings, IServiceProvider serviceProvider)
        {
            _kafkaProducer = kafkaProducer;
            _connectionStrings = connectionStrings;
            _kafkaSettings = kafkaSettings;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = ObterConfiguracaoConsumidor();
            var consumidor = ObterConsumidorTopicoNovoPedido(config);

            try
            {
                Console.WriteLine("Consumo iniciado");

                return ObterTarefaConsumoTopicoNovoPedido(consumidor, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                consumidor.Close();

                throw;
            }
        }

        private ConsumerConfig ObterConfiguracaoConsumidor()
        {
            return new ConsumerConfig
            {
                BootstrapServers = _connectionStrings.Kafka,
                GroupId = _kafkaSettings.GroupId
            };
        }

        private IConsumer<Ignore, string> ObterConsumidorTopicoNovoPedido(ConsumerConfig config)
        {
            var consumidor = new ConsumerBuilder<Ignore, string>(config).Build();
            consumidor.Subscribe(_kafkaSettings.Topics.NovoPedido);

            return consumidor;
        }

        private Task ObterTarefaConsumoTopicoNovoPedido(IConsumer<Ignore, string> consumidor, CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var mensagem = consumidor.Consume(stoppingToken);
                    var pedido = JsonSerializer.Deserialize<PedidoEntity>(mensagem.Message.Value);

                    PedidoEntity pedidoProcessado;
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var estoqueDal = scope.ServiceProvider.GetRequiredService<EstoqueDal>();
                        pedidoProcessado = new ProcessadorPedidos(estoqueDal).Processar(pedido);
                    }

                    _kafkaProducer.AdicionarTopicoConfirmacaoPedido(pedidoProcessado);

                    Console.WriteLine("Registro da fila consumido com sucesso");
                }
            }, CancellationToken.None);
        }
    }
}
