#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$SCRIPT_DIR"
cd "$ROOT"

# Ajuste se seu repo tiver estrutura diferente
DOMAIN="src/Ambev.DeveloperEvaluation.Domain"
APP="src/Ambev.DeveloperEvaluation.Application"
ORM="src/Ambev.DeveloperEvaluation.ORM"
WEB="src/Ambev.DeveloperEvaluation.WebApi"
IOC="src/Ambev.DeveloperEvaluation.IoC"

echo "==> Root fixado em: $ROOT"
echo "==> Creating folders..."

mkdir -p "$DOMAIN/Entities/Customers"
mkdir -p "$DOMAIN/Repositories/Customers"

mkdir -p "$ORM/Configurations/Customers"
mkdir -p "$ORM/Repositories/Customers"

mkdir -p "$APP/Customers/Dtos"
mkdir -p "$APP/Customers/Commands/CreateCustomer"
mkdir -p "$APP/Customers/Commands/UpdateCustomer"
mkdir -p "$APP/Customers/Commands/DeleteCustomer"
mkdir -p "$APP/Customers/Queries/GetCustomerById"
mkdir -p "$APP/Customers/Queries/GetAllCustomers"

mkdir -p "$WEB/Features/Customers/Dtos"
mkdir -p "$WEB/Features/Customers/Controllers"

mkdir -p "$IOC/Customers"

echo "==> Writing files..."

# ---------------------------
# Domain - Entity
# ---------------------------
cat > "$DOMAIN/Entities/Customers/Customer.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Customers;

public sealed class Customer
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = default!;
    public string Document { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    protected Customer() { }

    public Customer(string name, string document, string email, string phone)
    {
        SetName(name);
        SetDocument(document);
        SetEmail(email);
        SetPhone(phone);
    }

    public void Update(string name, string document, string email, string phone, bool isActive)
    {
        SetName(name);
        SetDocument(document);
        SetEmail(email);
        SetPhone(phone);
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new SalesDomainException("Nome do cliente é obrigatório.");

        Name = name.Trim();
    }

    private void SetDocument(string document)
    {
        if (string.IsNullOrWhiteSpace(document))
            throw new SalesDomainException("Documento do cliente é obrigatório.");

        Document = document.Trim();
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new SalesDomainException("E-mail do cliente é obrigatório.");

        Email = email.Trim();
    }

    private void SetPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new SalesDomainException("Telefone do cliente é obrigatório.");

        Phone = phone.Trim();
    }
}
EOF

# ---------------------------
# Domain - Repository contract
# ---------------------------
cat > "$DOMAIN/Repositories/Customers/ICustomerRepository.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;

namespace Ambev.DeveloperEvaluation.Domain.Repositories.Customers;

public interface ICustomerRepository
{
    // Reads
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Customer>> GetAllAsync(CancellationToken ct);

    // Writes (tracked)
    Task<Customer?> GetByIdTrackedAsync(Guid id, CancellationToken ct);

    // Uniqueness checks
    Task<bool> ExistsByDocumentAsync(string document, Guid? ignoringId, CancellationToken ct);
    Task<bool> ExistsByEmailAsync(string email, Guid? ignoringId, CancellationToken ct);

    // CRUD
    Task AddAsync(Customer customer, CancellationToken ct);
    Task UpdateAsync(Customer customer, CancellationToken ct);
    Task DeleteAsync(Customer customer, CancellationToken ct);
}
EOF

# ---------------------------
# ORM - EF Configuration
# ---------------------------
cat > "$ORM/Configurations/Customers/CustomerConfiguration.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Configurations.Customers;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> b)
    {
        b.ToTable("customers");

        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Document).HasMaxLength(30).IsRequired();
        b.Property(x => x.Email).HasMaxLength(200).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(30).IsRequired();

        b.Property(x => x.IsActive).IsRequired();

        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt);

        b.HasIndex(x => x.Document).IsUnique();
        b.HasIndex(x => x.Email).IsUnique();
        b.HasIndex(x => x.Name);
    }
}
EOF

# ---------------------------
# ORM - Repository implementation
# ---------------------------
cat > "$ORM/Repositories/Customers/CustomerRepository.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Customers;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly DefaultContext _ctx;

    public CustomerRepository(DefaultContext ctx) => _ctx = ctx;

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct)
        => _ctx.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<List<Customer>> GetAllAsync(CancellationToken ct)
        => _ctx.Customers.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<Customer?> GetByIdTrackedAsync(Guid id, CancellationToken ct)
        => _ctx.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> ExistsByDocumentAsync(string document, Guid? ignoringId, CancellationToken ct)
        => _ctx.Customers.AnyAsync(x => x.Document == document && (!ignoringId.HasValue || x.Id != ignoringId), ct);

    public Task<bool> ExistsByEmailAsync(string email, Guid? ignoringId, CancellationToken ct)
        => _ctx.Customers.AnyAsync(x => x.Email == email && (!ignoringId.HasValue || x.Id != ignoringId), ct);

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        _ctx.Customers.Add(customer);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken ct)
    {
        _ctx.Customers.Update(customer);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken ct)
    {
        _ctx.Customers.Remove(customer);
        await _ctx.SaveChangesAsync(ct);
    }
}
EOF

# ---------------------------
# Application - DTO
# ---------------------------
cat > "$APP/Customers/Dtos/CustomerDto.cs" <<'"'"'EOF'"'"'
namespace Ambev.DeveloperEvaluation.Application.Customers.Dtos;

public sealed class CustomerDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = default!;
    public string Document { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Phone { get; init; } = default!;

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
EOF

# ---------------------------
# Application - Create
# ---------------------------
cat > "$APP/Customers/Commands/CreateCustomer/CreateCustomerCommand.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name,
    string Document,
    string Email,
    string Phone
) : IRequest<CustomerDto>;
EOF

cat > "$APP/Customers/Commands/CreateCustomer/CreateCustomerCommandHandler.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _repo;

    public CreateCustomerCommandHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        if (await _repo.ExistsByDocumentAsync(request.Document, null, ct))
            throw new SalesDomainException("Já existe cliente com este documento.");

        if (await _repo.ExistsByEmailAsync(request.Email, null, ct))
            throw new SalesDomainException("Já existe cliente com este e-mail.");

        var customer = new Customer(request.Name, request.Document, request.Email, request.Phone);
        await _repo.AddAsync(customer, ct);

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Document = customer.Document,
            Email = customer.Email,
            Phone = customer.Phone,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
EOF

# ---------------------------
# Application - Update
# ---------------------------
cat > "$APP/Customers/Commands/UpdateCustomer/UpdateCustomerCommand.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid Id,
    string Name,
    string Document,
    string Email,
    string Phone,
    bool IsActive
) : IRequest<CustomerDto>;
EOF

cat > "$APP/Customers/Commands/UpdateCustomer/UpdateCustomerCommandHandler.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _repo;

    public UpdateCustomerCommandHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<CustomerDto> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var customer = await _repo.GetByIdTrackedAsync(request.Id, ct);
        if (customer is null)
            throw new SalesDomainException("Cliente não encontrado.");

        if (await _repo.ExistsByDocumentAsync(request.Document, request.Id, ct))
            throw new SalesDomainException("Já existe outro cliente com este documento.");

        if (await _repo.ExistsByEmailAsync(request.Email, request.Id, ct))
            throw new SalesDomainException("Já existe outro cliente com este e-mail.");

        customer.Update(request.Name, request.Document, request.Email, request.Phone, request.IsActive);
        await _repo.UpdateAsync(customer, ct);

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Document = customer.Document,
            Email = customer.Email,
            Phone = customer.Phone,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
EOF

# ---------------------------
# Application - Delete
# ---------------------------
cat > "$APP/Customers/Commands/DeleteCustomer/DeleteCustomerCommand.cs" <<'"'"'EOF'"'"'
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.DeleteCustomer;

public sealed record DeleteCustomerCommand(Guid Id) : IRequest;
EOF

cat > "$APP/Customers/Commands/DeleteCustomer/DeleteCustomerCommandHandler.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _repo;

    public DeleteCustomerCommandHandler(ICustomerRepository repo) => _repo = repo;

    public async Task Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var customer = await _repo.GetByIdTrackedAsync(request.Id, ct);
        if (customer is null)
            throw new SalesDomainException("Cliente não encontrado.");

        await _repo.DeleteAsync(customer, ct);
    }
}
EOF

# ---------------------------
# Application - GetById
# ---------------------------
cat > "$APP/Customers/Queries/GetCustomerById/GetCustomerByIdQuery.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto>;
EOF

cat > "$APP/Customers/Queries/GetCustomerById/GetCustomerByIdQueryHandler.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Queries.GetCustomerById;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto>
{
    private readonly ICustomerRepository _repo;

    public GetCustomerByIdQueryHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var customer = await _repo.GetByIdAsync(request.Id, ct);
        if (customer is null)
            throw new SalesDomainException("Cliente não encontrado.");

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Document = customer.Document,
            Email = customer.Email,
            Phone = customer.Phone,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
EOF

# ---------------------------
# Application - GetAll
# ---------------------------
cat > "$APP/Customers/Queries/GetAllCustomers/GetAllCustomersQuery.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Queries.GetAllCustomers;

public sealed record GetAllCustomersQuery() : IRequest<List<CustomerDto>>;
EOF

cat > "$APP/Customers/Queries/GetAllCustomers/GetAllCustomersQueryHandler.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Queries.GetAllCustomers;

public sealed class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, List<CustomerDto>>
{
    private readonly ICustomerRepository _repo;

    public GetAllCustomersQueryHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<List<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken ct)
    {
        var list = await _repo.GetAllAsync(ct);

        return list.Select(customer => new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Document = customer.Document,
            Email = customer.Email,
            Phone = customer.Phone,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        }).ToList();
    }
}
EOF

# ---------------------------
# WebApi - Request DTOs
# ---------------------------
cat > "$WEB/Features/Customers/Dtos/CreateCustomerRequestDto.cs" <<'"'"'EOF'"'"'
namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.Dtos;

public sealed class CreateCustomerRequestDto
{
    public string Name { get; set; } = default!;
    public string Document { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
}
EOF

cat > "$WEB/Features/Customers/Dtos/UpdateCustomerRequestDto.cs" <<'"'"'EOF'"'"'
namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.Dtos;

public sealed class UpdateCustomerRequestDto
{
    public string Name { get; set; } = default!;
    public string Document { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}
EOF

# ---------------------------
# WebApi - Controller
# ---------------------------
cat > "$WEB/Features/Customers/Controllers/CustomersController.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Application.Customers.Commands.CreateCustomer;
using Ambev.DeveloperEvaluation.Application.Customers.Commands.DeleteCustomer;
using Ambev.DeveloperEvaluation.Application.Customers.Commands.UpdateCustomer;
using Ambev.DeveloperEvaluation.Application.Customers.Queries.GetAllCustomers;
using Ambev.DeveloperEvaluation.Application.Customers.Queries.GetCustomerById;
using Ambev.DeveloperEvaluation.WebApi.Features.Customers.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllCustomersQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetCustomerByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequestDto dto, CancellationToken ct)
    {
        var created = await _mediator.Send(new CreateCustomerCommand(dto.Name, dto.Document, dto.Email, dto.Phone), ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequestDto dto, CancellationToken ct)
        => Ok(await _mediator.Send(new UpdateCustomerCommand(id, dto.Name, dto.Document, dto.Email, dto.Phone, dto.IsActive), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCustomerCommand(id), ct);
        return NoContent();
    }
}
EOF

# ---------------------------
# IoC - Module
# ---------------------------
cat > "$IOC/Customers/CustomersDependencyInjection.cs" <<'"'"'EOF'"'"'
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using Ambev.DeveloperEvaluation.ORM.Repositories.Customers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.Customers;

public static class CustomersDependencyInjection
{
    public static IServiceCollection AddCustomersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        return services;
    }
}
EOF

# ---------------------------
# Readme with manual steps
# ---------------------------
cat > "CUSTOMERS_ADD_THIS.md" <<'"'"'EOF'"'"'
# Customers - passos finais (cola e pronto)

Este script cria os arquivos do módulo Customers (Domain + Application + ORM + WebApi + IoC).
Alguns pontos são específicos do seu template e exigem ajustes manuais simples.

---

## 1) DefaultContext (EF Core)

Arquivo:
- `src/Ambev.DeveloperEvaluation.ORM/DefaultContext.cs`

### 1.1) DbSet

Adicione (no topo do arquivo, junto com outros `using`):
```csharp
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
```

E adicione o DbSet na classe do contexto:
```csharp
public DbSet<Customer> Customers => Set<Customer>();
```

### 1.2) ApplyConfiguration

No `OnModelCreating(ModelBuilder modelBuilder)` (ou equivalente), registre a configuração:

```csharp
using Ambev.DeveloperEvaluation.ORM.Configurations.Customers;

// ...

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfiguration(new CustomerConfiguration());

    // outras configurações...
}
```

> Observação: se seu projeto usa `ApplyConfigurationsFromAssembly`, então **não precisa** do ApplyConfiguration manual.
> Basta garantir que o assembly do ORM está sendo varrido, por exemplo:
> `modelBuilder.ApplyConfigurationsFromAssembly(typeof(DefaultContext).Assembly);`

---

## 2) IoC/Program.cs (registrar módulo Customers)

Verifique onde você registra dependências (geralmente em `src/Ambev.DeveloperEvaluation.IoC` e chamado no WebApi).

### 2.1) Adicione o using
No arquivo que compõe os módulos (ex.: `DependencyInjection.cs`, `NativeInjector.cs` ou direto no `Program.cs`), adicione:

```csharp
using Ambev.DeveloperEvaluation.IoC.Customers;
```

### 2.2) Chame o módulo
Onde estiver registrando módulos, inclua:

```csharp
services.AddCustomersModule(configuration);
```

---

## 3) MediatR (registrar handlers)

Se o template já faz scan por assembly, provavelmente já está ok. Se precisar, garanta que o assembly
da Application está sendo varrido, por exemplo:

```csharp
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Ambev.DeveloperEvaluation.Application.Customers.Dtos.CustomerDto).Assembly));
```

---

## 4) Referências de projetos (.csproj)

Verifique se o WebApi já referencia a Application; se não, adicione referências conforme seu template.

Geralmente:
- WebApi -> Application
- Application -> Domain
- ORM -> Domain
- IoC -> Domain + ORM (e às vezes Application, dependendo do padrão do template)

---

## 5) Migration (SQLite)

A partir da raiz do repo, rode (ajuste o caminho do projeto Startup/DbContext se necessário):

```bash
dotnet ef migrations add add_customers --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

---

## 6) Teste rápido (Postman / curl)

Base URL (exemplo):
- `https://localhost:5001`

### Criar
```bash
curl -X POST "https://localhost:5001/api/customers" \
  -H "Content-Type: application/json" \
  -d '{"name":"João da Silva","document":"12345678901","email":"joao@teste.com","phone":"+55 11 99999-9999"}'
```

### Listar
```bash
curl "https://localhost:5001/api/customers"
```

EOF

echo "==> Done!"
echo "Files generated + CUSTOMERS_ADD_THIS.md created."
