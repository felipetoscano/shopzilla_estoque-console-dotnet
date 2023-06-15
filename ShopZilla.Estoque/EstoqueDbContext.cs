using Microsoft.EntityFrameworkCore;
using ShopZilla.Estoque.Entities;

namespace ShopZilla.Estoque
{
    public class EstoqueDbContext : DbContext
    {
        public DbSet<EstoqueEntity> Estoque { get; set; }

        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }
    }
}
