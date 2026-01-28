
---

# Ambev Developer Evaluation – Backend API

## 📌 Visão Geral

Este projeto foi desenvolvido como parte do **Desafio Técnico – Developer Evaluation**, com o objetivo de demonstrar domínio em:

* Arquitetura de software
* Boas práticas em APIs REST
* Separação de responsabilidades
* Persistência de dados
* Testes automatizados
* Execução reprodutível via Docker

A solução entrega uma **API REST em .NET 8**, utilizando **PostgreSQL**, **MongoDB**, **CQRS com MediatR**, **Entity Framework Core**, **Swagger**, **JWT Authentication** e **testes unitários**.

Todo o ambiente pode ser executado **sem instalação local de banco ou SDK**, apenas com Docker.

---

## 🧱 Arquitetura do Projeto

O projeto segue uma **arquitetura em camadas**, separando claramente responsabilidades:

```
root
├── src
│   ├── Ambev.DeveloperEvaluation.WebApi        # Camada de API (Controllers, Middleware)
│   ├── Ambev.DeveloperEvaluation.Application   # Casos de uso (Commands, Queries, Handlers)
│   ├── Ambev.DeveloperEvaluation.Domain        # Entidades e regras de negócio
│   ├── Ambev.DeveloperEvaluation.ORM           # EF Core, DbContext, Migrations
│   └── Ambev.DeveloperEvaluation.Common        # Cross-cutting (Logging, Validation, Security)
│
├── tests
│   └── Ambev.DeveloperEvaluation.Unit          # Testes unitários (xUnit)
│
├── docker-compose.yml
├── Dockerfile
└── README.md
```

### 🔹 Motivo da Arquitetura

Essa abordagem foi escolhida para:

* Facilitar manutenção e evolução
* Permitir testes isolados de regras de negócio
* Evitar acoplamento entre domínio e infraestrutura
* Demonstrar padrões amplamente utilizados em projetos reais

---

## 🔁 Padrões e Tecnologias Utilizadas

### ✅ .NET 8 + ASP.NET Core

* Framework moderno, performático e amplamente utilizado no mercado.

### ✅ CQRS + MediatR

* Commands e Queries separados
* Handlers isolados e testáveis
* Controllers simples (“thin controllers”)

### ✅ Entity Framework Core + PostgreSQL

* Persistência relacional
* Mapeamentos explícitos com `EntityTypeConfiguration`
* Migrations organizadas na camada ORM

### ✅ MongoDB

* Banco NoSQL utilizado para cenários de documentos/eventos (quando aplicável)
* Demonstra conhecimento em arquitetura poliglota de persistência

### ✅ AutoMapper

* Mapeamento automático entre DTOs e entidades
* Redução de código repetitivo

### ✅ Swagger (OpenAPI)

* Documentação automática da API
* Facilita validação dos endpoints pelo avaliador

### ✅ JWT Authentication (não foi implementado, mas está demonstrado)

* Endpoints protegidos
* Endpoint de autenticação disponível para geração de token

### ✅ Validação Centralizada

* Pipeline Behavior do MediatR
* Middleware global para tratamento de exceções

---

## 🧪 Testes Automatizados

Os testes unitários foram implementados utilizando:

* **xUnit** – framework de testes
* **NSubstitute** – mocking de dependências
* **Bogus** – geração de dados fake realistas

### Regras de Negócio Testadas (Sales)

* Regra de desconto **4%**
* Regra de desconto **10%**
* Regra de desconto **20%**
* Garantia de **limite máximo de 20%**

Esses testes validam que as regras do domínio funcionam corretamente de forma isolada, sem dependência de banco ou infraestrutura.

---

## 🐳 Execução com Docker (Recomendado)

### 🔹 Pré-requisitos

* Docker
* Docker Compose

Nenhuma outra instalação é necessária.

---

### ▶️ Subir toda a aplicação

Na pasta raiz do backend:

```bash
docker compose up -d --build
```

Esse comando irá subir automaticamente:

* PostgreSQL
* MongoDB
* API .NET 8

---

### 📦 Verificar containers

```bash
docker ps
```

---

### 🌐 Acessar a API (Swagger)

Após subir os containers, acesse:

```
http://localhost:8080/swagger/index.html
```

O Swagger exibirá todos os endpoints disponíveis, incluindo:

* Auth
* Sales
* Users

---

### 🧪 Executar os testes

#### Opção 1 – Com .NET SDK instalado localmente

```bash
dotnet test
```

#### Opção 2 – Executar testes via Docker (sem instalar .NET)

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 dotnet test
```

---

### ⏹️ Derrubar os containers

```bash
docker compose down
```

## 📊 Diferenciais da Implementação

✔ Arquitetura limpa e escalável
✔ Separação clara de responsabilidades
✔ CQRS aplicado corretamente
✔ Testes cobrindo regras críticas
✔ Dockerização completa (zero setup para avaliador)
✔ Swagger funcional no ambiente Docker
✔ Código organizado, legível e extensível

---

## 📌 Considerações Finais

Este projeto foi desenvolvido com foco em **qualidade, clareza e aderência a padrões de mercado**, simulando um cenário real de aplicação corporativa.
Foi utilizado um template da ambve

A abordagem adotada facilita:

* Evolução futura
* Testabilidade
* Leitura e avaliação do código

---

