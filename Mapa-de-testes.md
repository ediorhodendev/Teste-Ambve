

---

# JSONs de Teste — Ambev.DeveloperEvaluation

## Convenções usadas nos exemplos

* `id`: GUID em formato padrão.
* Datas em UTC (ISO 8601).
* Campos podem variar levemente conforme DTOs, mas a intenção e os cenários estão alinhados ao projeto.
* Substitua os IDs pelos retornados nos testes reais.

---

## 1) Customers

### 1.1 Criar Customer — Request

```json
{
  "name": "Mercado Central LTDA",
  "email": "financeiro@mercadocentral.com.br",
  "phone": "+55 11 99999-1111",
  "document": "12.345.678/0001-90",
  "isActive": true
}
```

### 1.2 Criar Customer — Response (201 Created – exemplo)

```json
{
  "id": "a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1",
  "name": "Mercado Central LTDA",
  "email": "financeiro@mercadocentral.com.br",
  "phone": "+55 11 99999-1111",
  "document": "12.345.678/0001-90",
  "isActive": true,
  "createdAt": "2026-01-28T21:00:00Z"
}
```

### 1.3 Atualizar Customer — Request

```json
{
  "name": "Mercado Central LTDA (Atualizado)",
  "email": "contato@mercadocentral.com.br",
  "phone": "+55 11 98888-2222",
  "document": "12.345.678/0001-90",
  "isActive": true
}
```

### 1.4 Criar Customer — inválido (email inválido)

```json
{
  "name": "Cliente Email Inválido",
  "email": "email-invalido",
  "phone": "+55 11 99999-1111",
  "document": "12.345.678/0001-90",
  "isActive": true
}
```

---

## 2) Branches

### 2.1 Criar Branch — Request

```json
{
  "name": "Filial São Paulo Paulista",
  "location": "São Paulo - SP",
  "isActive": true
}
```

### 2.2 Criar Branch — Request (Curitiba)

```json
{
  "name": "Filial Curitiba Centro",
  "location": "Curitiba - PR",
  "isActive": true
}
```

### 2.3 Atualizar Branch — Request

```json
{
  "name": "Filial São Paulo Paulista (Atualizada)",
  "location": "São Paulo - SP",
  "isActive": true
}
```

---

## 3) Products

### 3.1 Criar Product — Request (Cerveja)

```json
{
  "name": "Cerveja Pilsen 350ml",
  "price": 4.99,
  "sku": "BEER-350-PILSEN",
  "isActive": true
}
```

### 3.2 Criar Product — Request (Refrigerante)

```json
{
  "name": "Refrigerante Cola 2L",
  "price": 8.50,
  "sku": "SODA-COLA-2L",
  "isActive": true
}
```

### 3.3 Criar Product — inválido (preço negativo)

```json
{
  "name": "Produto Inválido",
  "price": -10.0,
  "sku": "INVALID-NEG",
  "isActive": true
}
```

---

## 4) Sales (Vendas)

### Regras de negócio implementadas

* Quantidade **< 4** → sem desconto
* Quantidade **>= 4 e < 10** → **10%**
* Quantidade **>= 10 e <= 20** → **20%**
* Quantidade **> 20** → **inválido**

> Substitua:

* `customerId` por um customer válido
* `branchId` por uma branch válida
* `productId` por produtos válidos

---

### 4.1 Criar Sale — sem desconto

```json
{
  "saleNumber": "SALE-20260128-0001",
  "saleDate": "2026-01-28T20:00:00Z",
  "customerId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "branchId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
  "items": [
    {
      "productId": "cccccccc-cccc-cccc-cccc-cccccccccccc",
      "quantity": 1,
      "unitPrice": 4.99
    }
  ]
}
```

---

### 4.2 Criar Sale — 10% de desconto (qty = 4)

```json
{
  "saleNumber": "SALE-20260128-0002",
  "saleDate": "2026-01-28T20:10:00Z",
  "customerId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "branchId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
  "items": [
    {
      "productId": "cccccccc-cccc-cccc-cccc-cccccccccccc",
      "quantity": 4,
      "unitPrice": 4.99
    }
  ]
}
```

---

### 4.3 Criar Sale — 20% de desconto (qty = 10)

```json
{
  "saleNumber": "SALE-20260128-0003",
  "saleDate": "2026-01-28T20:20:00Z",
  "customerId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "branchId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
  "items": [
    {
      "productId": "dddddddd-dddd-dddd-dddd-dddddddddddd",
      "quantity": 10,
      "unitPrice": 8.50
    }
  ]
}
```

---

### 4.4 Criar Sale — inválido (qty > 20)

```json
{
  "saleNumber": "SALE-20260128-0004",
  "saleDate": "2026-01-28T20:30:00Z",
  "customerId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "branchId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
  "items": [
    {
      "productId": "cccccccc-cccc-cccc-cccc-cccccccccccc",
      "quantity": 21,
      "unitPrice": 4.99
    }
  ]
}
```

**Resposta esperada (erro de negócio):**

```json
{
  "code": "business_error",
  "message": "Invalid quantity. Maximum allowed is 20.",
  "traceId": "..."
}
```

---

## 5) Cancelamentos

### 5.1 Cancelar venda — Request

```json
{
  "reason": "Cliente desistiu da compra"
}
```

### 5.2 Cancelar item da venda — Request

```json
{
  "itemId": "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee",
  "reason": "Item sem estoque"
}
```

---

## 6) Sequência rápida para o avaliador testar

1. Criar Customer
2. Criar Branch
3. Criar 2 Products
4. Criar Sale (qty 4 → 10%)
5. Criar Sale (qty 10 → 20%)
6. Criar Sale inválida (qty 21)
7. Cancelar item ou venda

---

