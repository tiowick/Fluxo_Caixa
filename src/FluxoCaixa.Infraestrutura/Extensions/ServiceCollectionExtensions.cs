using FluxoCaixa.Dominio.Core.Cache;
using FluxoCaixa.Dominio.Core.Resilience;
using FluxoCaixa.Dominio.Interfaces.Consolidacao;
using FluxoCaixa.Dominio.Interfaces.Entrada;
using FluxoCaixa.Dominio.Servicos;
using FluxoCaixa.Infraestrutura.Contexto;
using FluxoCaixa.Infraestrutura.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FluxoCaixa.Infraestrutura.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFluxoCaixaInfrastructure(this IServiceCollection services)
        {
            // DbContext
            services.AddDbContext<FluxoCaixaContexto>(options =>
                options.UseInMemoryDatabase("FluxoCaixaDb"));

            // Cache
            services.AddMemoryCache();
            services.AddSingleton<ICache, MemoryCache>();

            // Resilience
            services.AddSingleton<IRetryPolicy>(new RetryPolicy(3, 100));

            // Repositories
            services.AddScoped<IDinheiroEntradaRepositorio, DinheiroEntradaRepositorio>();
            services.AddScoped<IConsolidacaoDiariaRepositorio, ConsolidacaoDiariaRepositorio>();

            return services;
        }
    }
}
