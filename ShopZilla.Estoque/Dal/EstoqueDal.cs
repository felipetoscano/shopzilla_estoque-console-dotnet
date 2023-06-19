using ShopZilla.Estoque.Entities;

namespace ShopZilla.Estoque.Dal
{
    public class EstoqueDal
    {
        private readonly EstoqueDbContext _dbContext;

        public EstoqueDal(EstoqueDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public EstoqueEntity BuscarEstoquePorSku(string sku) => _dbContext.Estoque.FirstOrDefault(p => p.Sku == sku);

        public void AlterarEstoque(EstoqueEntity estoqueEntity) => _dbContext.Estoque.Update(estoqueEntity);

        public void SalvarAlteracoes() => _dbContext.SaveChanges();
    }
}
