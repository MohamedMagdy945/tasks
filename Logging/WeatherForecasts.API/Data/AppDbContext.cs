using Microsoft.EntityFrameworkCore;
using WeatherForecasts.API.Models;

namespace WeatherForecasts.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
