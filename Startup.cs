using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using edmundTransactionService.Data;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Other code in the Startup class...

    public void ConfigureServices(IServiceCollection services)
    {
        // Configure the connection string
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        // Use the connection string in your code as needed (e.g., when configuring the database context)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySQL(connectionString));

        // Other service configurations...
    }

    // Other code in the Startup class...
}
