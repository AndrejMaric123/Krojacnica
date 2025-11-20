using Krojacnica.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

public static class DbContextFactory
{
    public static AppDbContext Create()
    {
        // Load appsettings.json
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var config = builder.Build();
        string connString = config.GetConnectionString("DefaultConnection");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connString, ServerVersion.AutoDetect(connString))
            .Options;

        return new AppDbContext(options);
    }
}
