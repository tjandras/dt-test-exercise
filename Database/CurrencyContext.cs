using digitalthinkers.Models;
using Microsoft.EntityFrameworkCore;

namespace digitalthinkers.Database
{
    public class CurrencyContext : DbContext
    {
        public CurrencyContext(DbContextOptions<CurrencyContext> options)
            : base(options)
        {

        }

        public DbSet<Currency> Currencies { get; set; }
    }
}