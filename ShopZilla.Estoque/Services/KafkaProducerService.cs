using Confluent.Kafka;
using ShopZilla.Estoque.Entities;
using ShopZilla.Estoque.Models;
using System.Text.Json;

namespace ShopZilla.Estoque.Services
{
    public class KafkaProducerService
    {
        private readonly ConnectionStrings _connectionStrings;

        public KafkaProducerService(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async void ConfirmarPedido(PedidoEntity pedido)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _connectionStrings.Kafka
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var pedidoSerializado = JsonSerializer.Serialize(pedido);
                    var mensagem = new Message<Null, string>() { Value = pedidoSerializado };
                    var response = await producer.ProduceAsync("CONFIRMACAO_PEDIDO", mensagem);

                    Console.WriteLine("Registro da fila adicionado com sucesso");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Erro no envio: {e.Error.Reason}");
                }
            }
        }
    }
}
