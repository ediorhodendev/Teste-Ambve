using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Entities.Branches;
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Ambev.DeveloperEvaluation.Domain.Entities.Products;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.ORM;

public sealed class DefaultContext : DbContext
{
    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options) { }

    
    public DbSet<User> Users { get; set; } = default!;

    
    public DbSet<Sale> Sales { get; set; } = default!;
    public DbSet<SaleItem> SaleItems { get; set; } = default!;

    
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<Customer> Customers { get; set; } = default!;
    public DbSet<Branch> Branches { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
