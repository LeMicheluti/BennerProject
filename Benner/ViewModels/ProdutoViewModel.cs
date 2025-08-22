using Benner.Models;
using Benner.Resources;
using Benner.Services;
using MvvmHelpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Benner.ViewModels
{
    public class ProdutoViewModel : BaseViewModel
    {
        private DataService<Produto> _dataService;

        public ObservableCollection<Produto> Produtos { get; set; } = new ObservableCollection<Produto>();
        private Produto _produtoSelecionado;
        private Guid? _idEdicao;

        // Campos de formulário
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public string Valor { get; set; }

        // Campos de filtro
        public string FiltroNome { get; set; }
        public string FiltroCodigo { get; set; }
        public string FiltroValorMin { get; set; }
        public string FiltroValorMax { get; set; }

        // Texto do botão Salvar
        public string TextoBotaoSalvar { get; set; } = "Salvar";

        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set { _produtoSelecionado = value; OnPropertyChanged(nameof(ProdutoSelecionado)); }
        }

        // Comandos
        public RelayCommand SalvarCommand { get; }
        public RelayCommand LimparCommand { get; }
        public RelayCommand FiltrarCommand { get; }
        public RelayCommand LimparFiltroCommand { get; }
        public RelayCommand<Produto> EditarCommand { get; }
        public RelayCommand<Produto> ExcluirCommand { get; }

        public ProdutoViewModel()
        {
            _dataService = new DataService<Produto>("produtos.json");
            CarregarProdutos();

            SalvarCommand = new RelayCommand(SalvarProduto);
            LimparCommand = new RelayCommand(LimparFormulario);
            FiltrarCommand = new RelayCommand(FiltrarProdutos);
            LimparFiltroCommand = new RelayCommand(LimparFiltros);
            EditarCommand = new RelayCommand<Produto>(EditarProduto);
            ExcluirCommand = new RelayCommand<Produto>(ExcluirProduto);
        }

        private void CarregarProdutos()
        {
            var lista = _dataService.Carregar();
            Produtos = new ObservableCollection<Produto>(lista);
            OnPropertyChanged(nameof(Produtos));
        }

        private void SalvarProduto(object _)
        {
            if (string.IsNullOrEmpty(Nome) || string.IsNullOrEmpty(Codigo) || string.IsNullOrEmpty(Valor))
            {
                MessageBox.Show("Nome, Código e Valor são obrigatórios.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(Valor, out decimal valorDec) || valorDec <= 0)
            {
                MessageBox.Show("Valor inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var produto = new Produto
            {
                Nome = Nome,
                Codigo = Codigo,
                Valor = valorDec
            };

            if (_idEdicao.HasValue)
            {
                produto.Id = _idEdicao.Value;
                _dataService.Editar(produto);
            }
            else
            {
                Produtos.Add(produto);
                _dataService.Salvar(Produtos.ToList());
            }

            LimparFormulario(null);
            CarregarProdutos();
        }

        private void LimparFormulario(object _)
        {
            Nome = Codigo = Valor = string.Empty;
            _idEdicao = null;
            TextoBotaoSalvar = "Salvar";
            OnPropertyChanged(nameof(Nome));
            OnPropertyChanged(nameof(Codigo));
            OnPropertyChanged(nameof(Valor));
            OnPropertyChanged(nameof(TextoBotaoSalvar));
        }

        private void FiltrarProdutos(object _)
        {
            decimal.TryParse(FiltroValorMin, out decimal valorMin);
            decimal.TryParse(FiltroValorMax, out decimal valorMax);

            var filtrado = _dataService.Carregar()
                .Where(p =>
                    (string.IsNullOrEmpty(FiltroNome) || p.Nome.ToLower().Contains(FiltroNome.ToLower())) &&
                    (string.IsNullOrEmpty(FiltroCodigo) || p.Codigo.Contains(FiltroCodigo)) &&
                    (string.IsNullOrEmpty(FiltroValorMin) || p.Valor >= valorMin) &&
                    (string.IsNullOrEmpty(FiltroValorMax) || p.Valor <= valorMax))
                .ToList();

            Produtos = new ObservableCollection<Produto>(filtrado);
            OnPropertyChanged(nameof(Produtos));
        }

        private void LimparFiltros(object _)
        {
            FiltroNome = FiltroCodigo = FiltroValorMin = FiltroValorMax = string.Empty;
            OnPropertyChanged(nameof(FiltroNome));
            OnPropertyChanged(nameof(FiltroCodigo));
            OnPropertyChanged(nameof(FiltroValorMin));
            OnPropertyChanged(nameof(FiltroValorMax));
            CarregarProdutos();
        }

        private void EditarProduto(Produto produto)
        {
            if (produto == null) return;

            Nome = produto.Nome;
            Codigo = produto.Codigo;
            Valor = produto.Valor.ToString();
            _idEdicao = produto.Id;
            TextoBotaoSalvar = "Editar";

            OnPropertyChanged(nameof(Nome));
            OnPropertyChanged(nameof(Codigo));
            OnPropertyChanged(nameof(Valor));
            OnPropertyChanged(nameof(TextoBotaoSalvar));
        }

        private void ExcluirProduto(Produto produto)
        {
            if (produto == null) return;

            if (MessageBox.Show($"Excluir {produto.Nome}?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Produtos.Remove(produto);
                _dataService.Salvar(Produtos.ToList());
            }
        }
    }
}
