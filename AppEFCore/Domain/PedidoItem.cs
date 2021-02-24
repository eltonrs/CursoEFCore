namespace AppEFCore.Domain
{
    public class PedidoItem
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; } // Propriedade de navegação (EF)
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; } // Propriedade de navegação (EF)
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorDesconto { get; set; }
    }
}