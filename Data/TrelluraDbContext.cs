using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trellura.Models;

namespace Trellura.API.Data
{
    public class TrelluraDbContext : DbContext
    {
        public TrelluraDbContext(DbContextOptions<TrelluraDbContext> options) : base(options)
        {

        }

        public DbSet<Cartao> Cartoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          modelBuilder.Entity<Cartao>().HasData(
            new Cartao(1,"Tarefa 1"),
            new Cartao(2, "Segunda tarefa"),
            new Cartao(3, "Tarefa 3")
          );

          base.OnModelCreating(modelBuilder);
        }
    }
}
