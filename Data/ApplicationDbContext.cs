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