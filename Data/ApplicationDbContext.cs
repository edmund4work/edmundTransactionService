using System.Data;
using Microsoft.EntityFrameworkCore;
using edmundTransactionService.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace edmundTransactionService.Data;
public class ApplicationDbContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }

    // Your DbContext configuration goes here

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the entity mappings and relationships
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
    }
}

// Entity configuration for the Transaction entity
public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        // Configure the table name and column mappings
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Amount).HasColumnType("decimal(18,2)");
    }
}
