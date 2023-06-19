using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using ShopZilla.Estoque.Dal;
using ShopZilla.Estoque.Entities;
using ShopZilla.Estoque.Models;
using System.Text.Json;

namespace ShopZilla.Estoque.Services
{
    public class KafkaConsumerService
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

        public void ConsumirNovosPedidos(CancellationToken cancellationToken)
        {
            var config = ObterConfiguracaoConsumidor();
            var consumidor = ObterConsumidorTopicoNovoPedido(config);

            try
            {
                Console.WriteLine("Consumo iniciado");
                IniciarConsumoTopico(consumidor, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                consumidor.Close();
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

        private void IniciarConsumoTopico(IConsumer<Ignore, string> consumidor, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var mensagem = consumidor.Consume(cancellationToken);
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
        }
    }
}
