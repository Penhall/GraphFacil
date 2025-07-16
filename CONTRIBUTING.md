# 🤝 CONTRIBUINDO PARA O SISTEMA LOTOFÁCIL

## 🎉 BEM-VINDO!

Obrigado por seu interesse em contribuir para o Sistema Lotofácil! Sua ajuda é muito bem-vinda. Este guia fornece as informações necessárias para que você possa colaborar de forma eficiente e produtiva.

## 🎯 COMO CONTRIBUIR

1.  **Crie um Fork:** Faça um fork do repositório para sua própria conta no GitHub.
2.  **Clone o Repositório:** Clone o fork para sua máquina local.
    ```bash
    git clone https://github.com/SEU_USUARIO/LotoFacil.git
    cd LotoFacil
    ```
3.  **Crie uma Branch:** Crie uma branch para sua contribuição. Use um nome descritivo (ex: `feature/novo-modelo`, `fix/bug-validacao`).
    ```bash
    git checkout -b feature/minha-contribuicao
    ```
4.  **Desenvolva:** Implemente sua contribuição. Siga as diretrizes de código (abaixo) e adicione testes unitários.
5.  **Teste:** Execute todos os testes para garantir que sua contribuição não quebra nada.
    ```bash
    # Comando para executar os testes (exemplo):
    dotnet test
    ```
6.  **Commit:** Faça commits com mensagens claras e concisas.
    ```bash
    git add .
    git commit -m "Adiciona nova funcionalidade X e corrige bug Y"
    ```
7.  **Push:** Envie suas alterações para o seu fork no GitHub.
    ```bash
    git push origin feature/minha-contribuicao
    ```
8.  **Crie um Pull Request (PR):** Abra um PR do seu fork para a branch principal do repositório original. Descreva suas mudanças e quaisquer informações relevantes.

## 📝 DIRETRIZES DE CÓDIGO

*   **Linguagem:** C#
*   **Framework:** .NET 8
*   **Padrões de Design:** Utilize os padrões já existentes no projeto (Strategy, Factory, Observer, Template Method, Repository).
*   **Testes:** Escreva testes unitários para todas as novas funcionalidades e correções de bugs.
*   **Código Limpo:** Siga os princípios de código limpo (SOLID, DRY, KISS).
*   **Documentação:** Adicione comentários relevantes ao seu código e atualize a documentação Markdown (`Docs/`) se necessário.
*   **Estilo de Código:** Mantenha a consistência com o estilo de código existente (nomes de variáveis, formatação, etc.).

## 💡 SUGESTÕES DE CONTRIBUIÇÕES

*   Implementar novos modelos de predição (seguindo a interface `IPredictionModel`).
*   Melhorar algoritmos existentes.
*   Adicionar novas métricas de análise.
*   Otimizar a performance do sistema.
*   Melhorar a interface do usuário.
*   Escrever mais testes unitários.
*   Atualizar ou expandir a documentação.

## 💬 DÚVIDAS?

Se tiver dúvidas, abra uma Issue no GitHub ou entre em contato com os mantenedores do projeto.

## 💖 AGRADECIMENTOS

Agradecemos desde já sua contribuição! Juntos, podemos criar um sistema de predição ainda melhor.