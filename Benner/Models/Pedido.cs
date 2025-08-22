using System;
using System.Collections.Generic;
using System.Linq;

namespace Benner.Models
{
    public enum StatusPedido { Pendente, Pago, Enviado, Recebido }
    public enum FormaPagamento { Dinheiro, Cartao, Boleto }

    public class Pedido
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Pessoa Pessoa { get; set; }
        public List<PedidoProduto> Produtos { get; set; } = new List<PedidoProduto>();
        public decimal ValorTotal => Produtos.Sum(p => p.Subtotal);
        public DateTime DataVenda { get; set; } = DateTime.Now;
        public FormaPagamento FormaPagamento { get; set; }
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;
    }
}
