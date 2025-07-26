using BookBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace BookBooks.Data
{
    public class BancoContext : DbContext
    {
        public BancoContext(DbContextOptions<BancoContext> options) : base (options)
        {

        }

        public DbSet<LivroModel> Livros { get; set; }
    }
}
