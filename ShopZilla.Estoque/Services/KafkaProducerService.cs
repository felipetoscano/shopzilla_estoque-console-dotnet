using Confluent.Kafka;
using ShopZilla.Estoque.Entities;
using ShopZilla.Estoque.Models;
using System.Text.Json;

namespace ShopZilla.Estoque.Services
{
    public class KafkaProducerService
    {
        private readonly ConnectionStrings _connectionStrings;
        private readonly KafkaSettings _kafkaSettings;

        public KafkaProducerService(ConnectionStrings connectionStrings, KafkaSettings kafkaSettings)
        {
            _connectionStrings = connectionStrings;
            _kafkaSettings = kafkaSettings;
        }

        public async void AdicionarTopicoConfirmacaoPedido(PedidoEntity pedido)
        {
            var config = ObterConfiguracaoConsumidor();

            using (var produtor = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    await EnviarPedidoTopicoConfirmacaoPedido(pedido, produtor);

                    Console.WriteLine("Registro da fila adicionado com sucesso");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Erro no envio: {e.Error.Reason}");
                }
            }
        }

        private ConsumerConfig ObterConfiguracaoConsumidor()
        {
            return new ConsumerConfig
            {
                BootstrapServers = _connectionStrings.Kafka
            };
        }

        private Task EnviarPedidoTopicoConfirmacaoPedido(PedidoEntity pedido, IProducer<Null, string> produtor)
        {
            var pedidoSerializado = JsonSerializer.Serialize(pedido);
            var mensagem = new Message<Null, string>() { Value = pedidoSerializado };
            return produtor.ProduceAsync(_kafkaSettings.Topics.ConfirmacaoPedido, mensagem);
        }
    }
}
