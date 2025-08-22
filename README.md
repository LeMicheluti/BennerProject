# Sistema de Cadastro WPF (.NET Framework)

Este projeto é um sistema de cadastro simples desenvolvido em **WPF** utilizando o padrão **MVVM**. Ele permite gerenciar Pessoas, Produtos e Pedidos, com funcionalidades de CRUD, filtros e gestão de pedidos.

---

## Tecnologias Utilizadas

- **C#**  
- **WPF (.NET Framework 4.6.2)**  
- **MVVM Pattern** (com MvvmHelpers para `BaseViewModel` e `RelayCommand`)  
- **JSON** para persistência local dos dados (`pessoas.json`, `produtos.json`, `pedidos.json`)  
- **Visual Studio 2022** (recomendado)  

---

## Estrutura do Projeto

- **Helpers**: Contém os helpers, como `FormaPagamentoHelper`.
- **Models**: Contém as classes de modelo, como `Pessoa`, `Produto`, `Pedido`, `PedidoProduto`, `FormaPagamento` e `StatusPedido`.  
- **ViewModels**: Contém os `ViewModels` de cada tela (`PessoaViewModel`, `ProdutoViewModel`, `PedidoViewModel`, `MainWindowViewModel`), responsáveis por lógica, comandos e bindings.  
- **Views**: Contém as telas em XAML (`PessoaView`, `ProdutoView`, `PedidoView`, `MainWindow`).  
- **Services**: Contém a classe `DataService<T>` responsável pela leitura e escrita de arquivos JSON.  
- **Resources**: Conté o RealyCommand.

---

## Como Executar

1. Não há requisitos específicos para executar
