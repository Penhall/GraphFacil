# Documentação do Arquivo MainWindow.xaml

## Descrição Geral

O arquivo [MainWindow.xaml](cci:7://file:///d:/PROJETOS/GraphFacil/Dashboard/MainWindow.xaml:0:0-0:0) define a interface de usuário para a janela principal do aplicativo Dashboard. Ele utiliza um layout em grade para organizar seus componentes e aplica estilos modernos usando a biblioteca Material Design.

## Estrutura da Interface

- **Janela Principal**: 
  - Classe: `Dashboard.MainWindow`
  - Dimensões: Altura de 700 e largura de 1080, centralizada na tela.
  - Estilo: Sem bordas (`WindowStyle="None"`), proporcionando um design limpo e moderno.

- **Layout de Grade**:
  - **Colunas**: 
    - Três colunas, duas com largura fixa de 200 e uma que ocupa o espaço restante.
  - **Linhas**: 
    - Duas linhas, uma com altura fixa de 30 e outra que ocupa o espaço restante.

## Componentes da Interface

- **Grid de Fundo**: 
  - Utiliza um `LinearGradientBrush` para aplicar um fundo gradiente, adicionando profundidade visual à interface.

- **StackPanel**:
  - Contém botões estilizados com ícones, utilizando a biblioteca `MaterialDesignInXaml`.
  - Exemplo de botão: [Dez](cci:1://file:///d:/PROJETOS/GraphFacil/Dashboard/MainWindow.xaml.cs:119:8-123:9), que dispara o evento [Dez_Click](cci:1://file:///d:/PROJETOS/GraphFacil/Dashboard/MainWindow.xaml.cs:119:8-123:9).

- **GridBarraTitulo**:
  - Funciona como uma barra de título personalizada, permitindo que a janela seja arrastada.
  - Inclui botões de ação, como `btnLembrete`, estilizados para se integrarem ao tema geral.

## Funcionalidades

- **Interatividade**: 
  - Botões são associados a eventos de clique, que executam métodos definidos no código-behind.
  
- **Estética e Design**:
  - A interface adota um design moderno e responsivo, utilizando ícones e estilos da biblioteca Material Design.

## Considerações Finais

O arquivo [MainWindow.xaml](cci:7://file:///d:/PROJETOS/GraphFacil/Dashboard/MainWindow.xaml:0:0-0:0) é essencial para a apresentação visual e interatividade do Dashboard, combinando um layout funcional com um design atraente. A integração com o código-behind permite que a interface responda a ações do usuário de forma dinâmica e eficiente.
