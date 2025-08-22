using System.Windows;
using Benner.ViewModels;

namespace Benner.Views
{
    public partial class PessoaView : Window
    {
        public PessoaView()
        {
            InitializeComponent();
            DataContext = new PessoaViewModel();
        }
    }
}
