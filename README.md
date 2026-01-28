
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

# 📦 Customers (Clientes)

## ➕ Criar Cliente

**POST** `/api/customers`

```json
{
  "name": "João da Silva",
  "document": "12345678901",
  "email": "joao.silva@email.com",
  "phone": "11999990001"
}
```

### Regras

* Documento e e-mail devem ser únicos
* E-mail validado
* Cliente inicia como ativo

---

## 📄 Listar Clientes

**GET** `/api/customers`

Retorna lista paginada de clientes.

---

## 🔍 Obter Cliente por ID

**GET** `/api/customers/{id}`

---

## ✏️ Atualizar Cliente

**PUT** `/api/customers/{id}`

```json
{
  "name": "João da Silva Atualizado",
  "email": "joao.novo@email.com",
  "phone": "11999990002",
  "isActive": true
}
```

---

## ❌ Desativar Cliente

**DELETE** `/api/customers/{id}`

> O cliente não é removido fisicamente (soft delete).

---

# 🏬 Branches (Filiais)

## ➕ Criar Filial

**POST** `/api/branches`

```json
{
  "name": "Filial São Paulo",
  "location": "São Paulo - SP"
}
```

---

## 📄 Listar Filiais

**GET** `/api/branches`

---

## 🔍 Obter Filial por ID

**GET** `/api/branches/{id}`

---

## ✏️ Atualizar Filial

**PUT** `/api/branches/{id}`

---

## ❌ Desativar Filial

**DELETE** `/api/branches/{id}`

---

# 🍺 Products (Produtos)

## ➕ Criar Produto

**POST** `/api/products`

```json
{
  "externalId": "SKU-001",
  "name": "Cerveja Pilsen 350ml",
  "description": "Cerveja Pilsen lata 350ml",
  "price": 3.50
}
```

### Regras

* `externalId` único
* Preço maior que zero

---

## 📄 Listar Produtos

**GET** `/api/products`

---

## 🔍 Obter Produto por ID

**GET** `/api/products/{id}`

---

## ✏️ Atualizar Produto

**PUT** `/api/products/{id}`

---

## ❌ Desativar Produto

**DELETE** `/api/products/{id}`

---

# 🧾 Sales (Vendas)

## ➕ Criar Venda

**POST** `/api/sales`

```json
{
  "saleNumber": "SALE-2026-0001",
  "saleDate": "2026-01-28T01:30:00Z",
  "customerId": "feda6cac-ff92-4e3a-809b-9650f978b267",
  "branchId": "a3f5ff94-9b34-4456-8b30-3a319dee5bc0",
  "items": [
    {
      "productId": "b202a1d9-937f-4c59-8ef7-384b93dc8a95",
      "quantity": 4
    }
  ]
}
```

### Exemplos de testes no documento de mapa de teste.
* Mapa-de-testes.md

### Regras de Negócio

* `saleNumber` é único
* Cliente, filial e produtos devem existir e estar ativos
* Quantidade > 0
* Totais são calculados automaticamente
* Snapshot de nomes e preços é salvo

---

## 📄 Listar Vendas

**GET** `/api/sales`

### Filtros disponíveis

* `saleNumber`
* `customerName`
* `branchName`
* `initialDate`
* `finalDate`
* Paginação

---

## 🔍 Obter Venda por ID

**GET** `/api/sales/{id}`

---

# 📜 Auditoria (MongoDB)

## 📌 Conceito

Toda ação relevante gera um **evento de auditoria** persistido no MongoDB, sem impactar a transação principal.

### Collection

```
sale_events
```

### Exemplo de documento

```json
{
  "saleId": "5407d2a8-1a77-43ce-9b05-f44d22f6fa8f",
  "eventType": "SaleCreated",
  "occurredAt": "2026-01-28T01:30:01Z",
  "payload": {
    "saleNumber": "SALE-2026-0001",
    "totalAmount": 14.00,
    "items": [
      {
        "productName": "Cerveja Pilsen 350ml",
        "quantity": 4,
        "unitPrice": 3.50,
        "totalAmount": 14.00
      }
    ]
  }
}
```

---




# ✅ Conclusão

Este projeto demonstra:

* Organização de código profissional
* Boas práticas modernas de backend
* Clareza arquitetural
* Facilidade de extensão e manutenção

---





## 📌 Considerações Finais

Este projeto foi desenvolvido com foco em **qualidade, clareza e aderência a padrões de mercado**, simulando um cenário real de aplicação corporativa.
Foi utilizado um template da ambve

A abordagem adotada facilita:

* Evolução futura
* Testabilidade
* Leitura e avaliação do código

---

