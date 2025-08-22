using Benner.Models;
using Benner.Resources;
using Benner.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Benner.ViewModels
{
    public class PedidoViewModel : BaseViewModel
    {
        private DataService<Pessoa> _dataServicePessoa;
        private DataService<Produto> _dataServiceProduto;
        private DataService<Pedido> _dataServicePedido;

        public ObservableCollection<Pessoa> Clientes { get; set; }
        public ObservableCollection<Produto> Produtos { get; set; }
        public ObservableCollection<PedidoProduto> ItensPedido { get; set; } = new ObservableCollection<PedidoProduto>();
        public ObservableCollection<Pedido> PedidosFinalizados { get; set; } = new ObservableCollection<Pedido>();

        private Pessoa _clienteSelecionado;
        public Pessoa ClienteSelecionado
        {
            get => _clienteSelecionado;
            set
            {
                _clienteSelecionado = value;
                OnPropertyChanged(nameof(ClienteSelecionado));
                AtualizarPedidosCliente();
            }
        }

        private Produto _produtoSelecionado;
        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set { _produtoSelecionado = value; OnPropertyChanged(nameof(ProdutoSelecionado)); }
        }

        public int Quantidade { get; set; } = 1;
        public FormaPagamento? FormaPagamentoSelecionada { get; set; }
        public decimal ValorTotal => ItensPedido.Sum(i => i.Subtotal);

        // Comandos
        public RelayCommand AdicionarProdutoCommand { get; }
        public RelayCommand<PedidoProduto> RemoverProdutoCommand { get; }
        public RelayCommand FinalizarPedidoCommand { get; }
        public RelayCommand<Pedido> MarcarPagoCommand { get; }
        public RelayCommand<Pedido> MarcarEnviadoCommand { get; }
        public RelayCommand<Pedido> MarcarRecebidoCommand { get; }

        public PedidoViewModel()
        {
            _dataServicePessoa = new DataService<Pessoa>("pessoas.json");
            _dataServiceProduto = new DataService<Produto>("produtos.json");
            _dataServicePedido = new DataService<Pedido>("pedidos.json");

            Clientes = new ObservableCollection<Pessoa>(_dataServicePessoa.Carregar());
            Produtos = new ObservableCollection<Produto>(_dataServiceProduto.Carregar());

            AdicionarProdutoCommand = new RelayCommand(AdicionarProduto);
            RemoverProdutoCommand = new RelayCommand<PedidoProduto>(RemoverProduto);
            FinalizarPedidoCommand = new RelayCommand(FinalizarPedido);
            MarcarPagoCommand = new RelayCommand<Pedido>(p => AtualizarStatus(p, StatusPedido.Pago));
            MarcarEnviadoCommand = new RelayCommand<Pedido>(p => AtualizarStatus(p, StatusPedido.Enviado));
            MarcarRecebidoCommand = new RelayCommand<Pedido>(p => AtualizarStatus(p, StatusPedido.Recebido));
        }

        private void AdicionarProduto(object _)
        {
            if (ProdutoSelecionado == null)
            {
                MessageBox.Show("Selecione um produto.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Quantidade <= 0)
            {
                MessageBox.Show("Quantidade inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ItensPedido.Add(new PedidoProduto { Produto = ProdutoSelecionado, Quantidade = Quantidade });
            OnPropertyChanged(nameof(ValorTotal));
        }

        private void RemoverProduto(PedidoProduto item)
        {
            if (item == null) return;
            ItensPedido.Remove(item);
            OnPropertyChanged(nameof(ValorTotal));
        }

        private void FinalizarPedido(object _)
        {
            if (ClienteSelecionado == null)
            {
                MessageBox.Show("Selecione um cliente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ItensPedido.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um produto.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (FormaPagamentoSelecionada == null)
            {
                MessageBox.Show("Selecione a forma de pagamento.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var pedido = new Pedido
            {
                Pessoa = ClienteSelecionado,
                Produtos = ItensPedido.ToList(),
                FormaPagamento = FormaPagamentoSelecionada.Value,
                Status = StatusPedido.Pendente
            };

            var pedidos = _dataServicePedido.Carregar();
            pedidos.Add(pedido);
            _dataServicePedido.Salvar(pedidos);

            MessageBox.Show("Pedido finalizado!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            ItensPedido.Clear();
            OnPropertyChanged(nameof(ValorTotal));
            ClienteSelecionado = null;
            ProdutoSelecionado = null;
            Quantidade = 1;
            FormaPagamentoSelecionada = null;
            AtualizarPedidosCliente();
        }

        private void AtualizarPedidosCliente()
        {
            if (ClienteSelecionado != null)
            {
                var pedidos = _dataServicePedido.Carregar()
                                .Where(p => p.Pessoa.Id == ClienteSelecionado.Id)
                                .ToList();
                PedidosFinalizados = new ObservableCollection<Pedido>(pedidos);
            }
            else
            {
                PedidosFinalizados.Clear();
            }
            OnPropertyChanged(nameof(PedidosFinalizados));
        }

        private void AtualizarStatus(Pedido pedido, StatusPedido status)
        {
            if (pedido == null) return;
            pedido.Status = status;

            var pedidos = _dataServicePedido.Carregar();
            var p = pedidos.FirstOrDefault(x => x.Id == pedido.Id);
            if (p != null) p.Status = status;
            _dataServicePedido.Salvar(pedidos);

            AtualizarPedidosCliente();
        }
    }
}
