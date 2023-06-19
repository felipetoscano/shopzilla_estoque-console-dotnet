using ShopZilla.Estoque.Dal;
using ShopZilla.Estoque.Entities;
using ShopZilla.Estoque.Models.Consts;

namespace ShopZilla.Estoque
{
    public class ProcessadorPedidos
    {
        private readonly EstoqueDal _estoqueDal;

        public ProcessadorPedidos(EstoqueDal estoqueDal)
        {
            _estoqueDal = estoqueDal;
        }

        public PedidoEntity Processar(PedidoEntity pedido)
        {
            foreach (var produto in pedido.Produtos)
            {
                var estoque = _estoqueDal.BuscarEstoquePorSku(produto.Sku);

                if (!PossuiEstoqueDisponivel(produto, estoque))
                {
                    pedido.Status = StatusPedido.RECUSADO;
                    return pedido;
                }

                estoque.Quantidade -= produto.Quantidade;
                _estoqueDal.AlterarEstoque(estoque);
            }

            _estoqueDal.SalvarAlteracoes();

            pedido.Status = StatusPedido.APROVADO;
            return pedido;
        }

        private static bool PossuiEstoqueDisponivel(ProdutoEntity produto, EstoqueEntity estoque)
        {
            return estoque?.Quantidade >= produto?.Quantidade;
        }
    }
}
