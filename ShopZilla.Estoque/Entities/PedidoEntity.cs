namespace ShopZilla.Estoque.Entities
{
    public class PedidoEntity
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int IdCliente { get; set; }
        public IList<ProdutoEntity> Produtos { get; set; }

        public void AprovarPedido() => Status = "APROVADO";
        public void RecusarPedido() => Status = "RECUSADO";
    }
}
