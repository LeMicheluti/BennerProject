using Benner.Resources;
using Benner.Views;
using MvvmHelpers;

namespace Benner.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public RelayCommand AbrirPessoaCommand { get; }
        public RelayCommand AbrirProdutoCommand { get; }
        public RelayCommand AbrirPedidoCommand { get; }

        public MainWindowViewModel()
        {
            AbrirPessoaCommand = new RelayCommand(AbrirPessoa);
            AbrirProdutoCommand = new RelayCommand(AbrirProduto);
            AbrirPedidoCommand = new RelayCommand(AbrirPedido);
        }

        private void AbrirPessoa()
        {
            var window = new PessoaView();
            window.ShowDialog();
        }

        private void AbrirProduto()
        {
            var window = new ProdutoView();
            window.ShowDialog();
        }

        private void AbrirPedido()
        {
            var window = new PedidoView();
            window.ShowDialog();
        }
    }
}
