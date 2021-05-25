using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using remarsemanal.Model;
using remarsemanal.Model.usuario;

namespace remarsemanal.Brokers
{
    public class EmpresaDatabase : DbContext
    {
        private Empresa empresa;
        public DbSet<Quadro> Quadro {get;set;}
        public DbSet<Tarefa> Tarefa {get;set;}
        public DbSet<Tipo> Tipo {get;set;}
        public DbSet<Usuario> Usuario {get;set;}

        public EmpresaDatabase(DbContextOptions<EmpresaDatabase> options, IHttpContextAccessor  httpContextAccessor) : base(options)
        {
            if (httpContextAccessor.HttpContext != null) {
                empresa = (Empresa)httpContextAccessor.HttpContext.Items["EMPRESA"];
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
            optionsBuilder.UseNpgsql(empresa.StringConexao);
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }        
    }

}