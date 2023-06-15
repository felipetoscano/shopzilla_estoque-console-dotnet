namespace ShopZilla.Estoque.Entities
{
    public class EstoqueEntity
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public int Quantidade { get; set; }

        public void SubtrairQuantidadeComprada(int quantidadeComprada) => Quantidade -= quantidadeComprada;
    }
}
