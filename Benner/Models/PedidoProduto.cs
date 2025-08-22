namespace Benner.Models
{
    public class PedidoProduto
    {
        public Produto Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal Subtotal => Produto.Valor * Quantidade;
    }
}
