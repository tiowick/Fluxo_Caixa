using FluxoCaixa.Dominio.Entidades.Consolidacao;
using FluxoCaixa.Dominio.Entidades.Entrada;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixa.Infraestrutura.Contexto
{
    public class FluxoCaixaContexto : DbContext
    {
        public FluxoCaixaContexto(DbContextOptions<FluxoCaixaContexto> options) : base(options)
        {
             ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
             ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<DinheiroEntrada> Lancamentos { get; set; }
        public DbSet<ConsolidacaoDiaria> Consolidados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DinheiroEntrada>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Data).IsRequired();
                entity.Property(e => e.Quantidade).HasPrecision(18, 2).IsRequired();
                entity.Property(e => e.Tipo).IsRequired();
                entity.Property(e => e.Descricao).HasMaxLength(500).IsRequired();
            });

            modelBuilder.Entity<ConsolidacaoDiaria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Data).IsRequired();
                entity.Property(e => e.SaldoConsolidado).HasPrecision(18, 2).IsRequired();
                
                // Índice para otimizar consultas por data
                entity.HasIndex(e => e.Data).IsUnique();
            });
        }
    }
}
