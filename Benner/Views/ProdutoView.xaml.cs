using Benner.Models;
using Benner.Services;
using Benner.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace Benner.Views
{
    public partial class ProdutoView : Window
    {
        public ProdutoView()
        {
            InitializeComponent();
            DataContext = new ProdutoViewModel();
        }
    }
}