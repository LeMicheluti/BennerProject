using Benner.Models;
using Benner.ViewModels;
using System.Linq;
using System.Windows;

namespace Benner.Views
{
    public partial class PedidoView : Window
    {
        public PedidoView()
        {
            InitializeComponent();
            DataContext = new PedidoViewModel();
        }

        public PedidoView(Pessoa clienteSelecionado = null)
        {
            InitializeComponent();

            var vm = new PedidoViewModel();

            if (clienteSelecionado != null)
                vm.ClienteSelecionado = vm.Clientes.FirstOrDefault(c => c.Id == clienteSelecionado.Id);

            DataContext = vm;
        }
    }
}