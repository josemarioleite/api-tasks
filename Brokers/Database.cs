using Microsoft.EntityFrameworkCore;
using remarsemanal.Model;

namespace remarsemanal.Brokers
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options) : base(options)
        {
            //this.Database.Migrate();
        }
        public DbSet<Empresa> Empresa {get;set;}
        public DbSet<UsuarioAlfameta> UsuarioAlfameta {get;set;}
    }
}