# 🍳 ChefShare - Plataforma de Partilha de Receitas

O **ChefShare** é uma aplicação web robusta desenvolvida para entusiastas da culinária que desejam partilhar as suas criações, gerir as suas próprias receitas e descobrir novos pratos. O projeto foca-se na organização, segurança e escalabilidade.

---

## 🏗️ Arquitetura do Projeto
O projeto foi recentemente reestruturado para seguir uma **Arquitetura em Camadas**, garantindo uma separação clara de responsabilidades:

* **RecipeApp.Web (UI/Razor Pages):** Interface do utilizador e lógica de apresentação.
* **RecipeApp.Services (Business Logic):** Camada intermediária que gere as regras de negócio.
* **RecipeApp.DAL (Data Access Layer):** Comunicação direta com a base de dados via ADO.NET.
* **RecipeApp.Models:** Entidades de dados partilhadas por toda a solução.

---

## 🚀 Funcionalidades Principais

-   **Gestão de Receitas:** Criação, edição e eliminação definitiva de receitas (incluindo limpeza de ingredientes e favoritos).
-   **Sistema de Moderação:** Fluxo de aprovação de receitas para garantir a qualidade do conteúdo.
-   **Gestão de Ingredientes:** Adição dinâmica de ingredientes com reconhecimento de duplicados na base de dados.
-   **Favoritos:** Os utilizadores podem marcar receitas para consulta rápida.
-   **Segurança:** Controlo de sessões e validação de permissões por ID de utilizador.
-   **UI Dinâmica:** Integração com a **Unsplash API** para imagens automáticas e visual moderno com Bootstrap.

---

## 🛠️ Tecnologias Utilizadas

-   **Backend:** ASP.NET Core 8.0 (Razor Pages)
-   **Linguagem:** C#
-   **Base de Dados:** SQL Server (ADO.NET com transações atómicas)
-   **Frontend:** Bootstrap 5, FontAwesome, Animate.css
-   **Versionamento:** Git & GitHub

---

## 📸 Demonstração do Layout

> Atualmente a aplicação conta com uma interface intuitiva onde o utilizador pode acompanhar o estado das suas receitas (Pendente/Aprovada) e gerir o seu perfil de forma simples.

---

## 🔧 Como Executar

1.  Clonar o repositório.
2.  Configurar a `ConnectionString` no ficheiro `appsettings.json` para o seu SQL Server local.
3.  Executar o script SQL de criação de tabelas (fornecido na pasta de documentação).
4.  Fazer Build e Run através do Visual Studio.

---
*Desenvolvido por [Fabio Salgado - https://github.com/Fsalgado-dotnet] como parte do portfólio de desenvolvimento .NET.*