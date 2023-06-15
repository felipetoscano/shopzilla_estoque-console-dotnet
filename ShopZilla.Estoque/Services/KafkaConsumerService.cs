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
        private readonly IServiceProvider _serviceProvider;

        public KafkaConsumerService(KafkaProducerService kafkaProducer, ConnectionStrings connectionStrings, IServiceProvider serviceProvider)
        {
            _kafkaProducer = kafkaProducer;
            _connectionStrings = connectionStrings;
            _serviceProvider = serviceProvider;
        }

        public void ConsumirNovosPedidos(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _connectionStrings.Kafka,
                GroupId = "ESTOQUE"
            };

            var consumidor = new ConsumerBuilder<Ignore, string>(config).Build();
            consumidor.Subscribe("NOVO_PEDIDO");

            try
            {
                Console.WriteLine("Consumo iniciado");
                while (!cancellationToken.IsCancellationRequested)
                {
                    var mensagem = consumidor.Consume(cancellationToken);
                    var pedido = JsonSerializer.Deserialize<PedidoEntity>(mensagem.Message.Value);

                    var pedidoProcessado = ProcessarPedido(pedido);

                    _kafkaProducer.ConfirmarPedido(pedidoProcessado);

                    Console.WriteLine("Registro da fila consumido com sucesso");
                }
            }
            catch (OperationCanceledException)
            {
                consumidor.Close();
            }
        }

        private PedidoEntity ProcessarPedido(PedidoEntity pedido)
        {
            using var scope = _serviceProvider.CreateScope();
            var estoqueDal = scope.ServiceProvider.GetRequiredService<EstoqueDal>();

            foreach (var produto in pedido.Produtos)
            {
                var estoque = estoqueDal.BuscarEstoquePorSku(produto.Sku);

                if (estoque is null || estoque.Quantidade < produto.Quantidade)
                {
                    pedido.RecusarPedido();
                    return pedido;
                }

                estoque.SubtrairQuantidadeComprada(produto.Quantidade);

                estoqueDal.AlterarEstoque(estoque);
                estoqueDal.SalvarAlteracoes();
            }

            pedido.AprovarPedido();
            return pedido;
        }
    }
}
