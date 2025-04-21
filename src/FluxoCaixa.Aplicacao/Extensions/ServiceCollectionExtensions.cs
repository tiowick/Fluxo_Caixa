using FluxoCaixa.Aplicacao.Servicos;
using FluxoCaixa.Dominio.Servicos;
using FluxoCaixa.Dominio.Core.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using MemoryCache = FluxoCaixa.Dominio.Core.Cache.MemoryCache;

namespace FluxoCaixa.Aplicacao.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFluxoCaixaApplication(this IServiceCollection services)
        {
            services.AddScoped<ILancamentoServico, LancamentoServico>();
            services.AddScoped<IConsolidacaoServico, ConsolidacaoServico>();

            // Adiciona o IMemoryCache do .NET
            services.AddMemoryCache();
            // Registra o ICache para ser resolvido como MemoryCache
            services.AddScoped<ICache, MemoryCache>();

            return services;
        }
    }
}
