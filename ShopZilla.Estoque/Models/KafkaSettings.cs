namespace ShopZilla.Estoque.Models
{
    public class KafkaSettings
    {
        public string GroupId { get; init; }
        public Topics Topics { get; init; }
    }

    public class Topics
    {
        public string NovoPedido { get; init; }
        public string ConfirmacaoPedido { get; init; }
    }
}
