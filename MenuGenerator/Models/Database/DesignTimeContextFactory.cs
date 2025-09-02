using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MenuGenerator.Models.Database;

public class DesignTimeContextFactory : IDesignTimeDbContextFactory<MenuGeneratorContext>
{
    public MenuGeneratorContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MenuGeneratorContext>();
        optionsBuilder.UseSqlite("Data Source=Models/Database/DevDbFiles/MenuGenerator.db"); // ../../../Models/Database/DevDbFiles/
        
        return new MenuGeneratorContext(optionsBuilder.Options);
    }
}