using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ProcessingService.Data
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
    public DataContext CreateDbContext(string[] args)
    {
        // Determine the project root dynamically
        var projectRoot = Directory.GetCurrentDirectory();


        // If appsettings.Development.json is not found, fallback up three directories
        if (!File.Exists(Path.Combine(projectRoot, "appsettings.Development.json")))
        {
            projectRoot = Path.Combine(projectRoot, "..", "..", "..");
            projectRoot = Path.GetFullPath(projectRoot);
        }

        Console.WriteLine($"Looking for appsettings in: {projectRoot}");

        // Build configuration
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(projectRoot)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"Design-time ConnectionString: {connectionString}");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string 'DefaultConnection' not found in {projectRoot}");
        }

        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseNpgsql(connectionString, opts => opts.EnableRetryOnFailure());

        return new DataContext(optionsBuilder.Options);
        
        }
    }
}
