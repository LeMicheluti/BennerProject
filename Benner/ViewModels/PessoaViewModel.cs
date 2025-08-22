using Benner.Models;
using Benner.Resources;
using Benner.Services;
using MvvmHelpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Benner.ViewModels
{
    public class PessoaViewModel : BaseViewModel
    {
        private readonly DataService<Pessoa> _dataService;

        public ObservableCollection<Pessoa> Pessoas { get; set; }

        private Pessoa _pessoaSelecionada;
        public Pessoa PessoaSelecionada
        {
            get => _pessoaSelecionada;
            set { _pessoaSelecionada = value; OnPropertyChanged(); }
        }

        // Propriedades da tela (bindings com TextBox)
        private string _nome;
        public string Nome { get => _nome; set { _nome = value; OnPropertyChanged(); } }

        private string _cpf;
        public string CPF { get => _cpf; set { _cpf = value; OnPropertyChanged(); } }

        private string _endereco;
        public string Endereco { get => _endereco; set { _endereco = value; OnPropertyChanged(); } }

        private string _filtroNome;
        public string FiltroNome { get => _filtroNome; set { _filtroNome = value; OnPropertyChanged(); } }

        private string _filtroCPF;
        public string FiltroCPF { get => _filtroCPF; set { _filtroCPF = value; OnPropertyChanged(); } }

        private Guid? _idEdicao;
        public string TextoBotaoSalvar { get; set; } = "Salvar";

        // Comandos
        public ICommand SalvarCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand FiltrarCommand { get; }
        public ICommand LimparFiltroCommand { get; }
        public ICommand IncluirPedidoCommand { get; }
        public ICommand LimparCamposCommand { get; }

        public PessoaViewModel()
        {
            _dataService = new DataService<Pessoa>("pessoas.json");
            Pessoas = new ObservableCollection<Pessoa>(_dataService.Carregar());

            SalvarCommand = new RelayCommand(SalvarPessoa);
            EditarCommand = new RelayCommand(EditarPessoa, () => PessoaSelecionada != null);
            ExcluirCommand = new RelayCommand(ExcluirPessoa, () => PessoaSelecionada != null);
            FiltrarCommand = new RelayCommand(Filtrar);
            LimparFiltroCommand = new RelayCommand(LimparFiltro);
            IncluirPedidoCommand = new RelayCommand(IncluirPedido, () => PessoaSelecionada != null);
            LimparCamposCommand = new RelayCommand(LimparCampos);
        }

        private void SalvarPessoa()
        {
            if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(CPF))
            {
                MessageBox.Show("Nome e CPF são obrigatórios.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string cpfNumeros = new string(CPF.Where(char.IsDigit).ToArray());
            if (cpfNumeros.Length != 11)
            {
                MessageBox.Show("CPF inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_idEdicao.HasValue)
            {
                // edição
                var pessoa = new Pessoa { Id = _idEdicao.Value, Nome = Nome, CPF = CPF, Endereco = Endereco };
                _dataService.Editar(pessoa);
                RecarregarPessoas();
            }
            else
            {
                // novo
                var pessoa = new Pessoa { Nome = Nome, CPF = CPF, Endereco = Endereco };
                Pessoas.Add(pessoa);
                _dataService.Salvar(Pessoas.ToList());
            }

            LimparCampos();
        }

        private void EditarPessoa()
        {
            if (PessoaSelecionada == null) return;

            Nome = PessoaSelecionada.Nome;
            CPF = PessoaSelecionada.CPF;
            Endereco = PessoaSelecionada.Endereco;
            _idEdicao = PessoaSelecionada.Id;
            TextoBotaoSalvar = "Editar";
            OnPropertyChanged(nameof(TextoBotaoSalvar));
        }

        private void ExcluirPessoa()
        {
            if (PessoaSelecionada == null) return;

            if (MessageBox.Show($"Excluir {PessoaSelecionada.Nome}?", "Confirmação",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Pessoas.Remove(PessoaSelecionada);
                _dataService.Salvar(Pessoas.ToList());
            }
        }

        private void Filtrar()
        {
            var filtrado = _dataService.Carregar()
                .Where(p => (string.IsNullOrEmpty(FiltroNome) || p.Nome.ToLower().Contains(FiltroNome.ToLower())) &&
                            (string.IsNullOrEmpty(FiltroCPF) || p.CPF.Contains(FiltroCPF)))
                .ToList();

            Pessoas = new ObservableCollection<Pessoa>(filtrado);
            OnPropertyChanged(nameof(Pessoas));
        }

        private void LimparFiltro()
        {
            FiltroNome = "";
            FiltroCPF = "";
            _idEdicao = null;
            RecarregarPessoas();
        }

        private void IncluirPedido()
        {
            if (PessoaSelecionada == null) return;

            var pedidoView = new Views.PedidoView(PessoaSelecionada);
            pedidoView.ShowDialog();
        }

        private void LimparCampos()
        {
            Nome = "";
            CPF = "";
            Endereco = "";
            _idEdicao = null;
            TextoBotaoSalvar = "Salvar";
            OnPropertyChanged(nameof(TextoBotaoSalvar));
            RecarregarPessoas();
        }

        private void RecarregarPessoas()
        {
            Pessoas = new ObservableCollection<Pessoa>(_dataService.Carregar());
            OnPropertyChanged(nameof(Pessoas));
        }
    }
}
