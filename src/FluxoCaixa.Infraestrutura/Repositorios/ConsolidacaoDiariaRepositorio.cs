using FluxoCaixa.Dominio.Core.Cache;
using FluxoCaixa.Dominio.Entidades.Consolidacao;
using FluxoCaixa.Dominio.Interfaces.Consolidacao;
using FluxoCaixa.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixa.Infraestrutura.Repositorios
{
    public class ConsolidacaoDiariaRepositorio : IConsolidacaoDiariaRepositorio
    {
        private readonly FluxoCaixaContexto _contexto;
        private readonly ICache _cache;
        private const string CacheKeyPrefix = "consolidado_";

        public ConsolidacaoDiariaRepositorio(FluxoCaixaContexto contexto, ICache cache)
        {
            _contexto = contexto;
            _cache = cache;
        }

        public async Task<ConsolidacaoDiaria> AdicionarAsync(ConsolidacaoDiaria consolidacao)
        {
            await _contexto.Consolidados.AddAsync(consolidacao);
            await _contexto.SaveChangesAsync();
            
            // Atualiza o cache
            await _cache.SetAsync($"{CacheKeyPrefix}{consolidacao.Data:yyyy-MM-dd}", consolidacao, TimeSpan.FromHours(1));
            
            return consolidacao;
        }

        public async Task<ConsolidacaoDiaria?> ObterPorDataAsync(DateTime data)
        {
            var cacheKey = $"{CacheKeyPrefix}{data:yyyy-MM-dd}";
            
            // Tenta obter do cache
            var consolidado = await _cache.GetAsync<ConsolidacaoDiaria>(cacheKey);
            if (consolidado != null)
                return consolidado;

            // Se não estiver no cache, busca no banco
            consolidado = await _contexto.Consolidados
                .FirstOrDefaultAsync(x => x.Data.Date == data.Date);

            if (consolidado != null)
                await _cache.SetAsync(cacheKey, consolidado, TimeSpan.FromHours(1));

            return consolidado;
        }

        public async Task AtualizarAsync(ConsolidacaoDiaria consolidacao)
        {
            _contexto.Consolidados.Update(consolidacao);
            await _contexto.SaveChangesAsync();
            
            // Atualiza o cache
            await _cache.SetAsync($"{CacheKeyPrefix}{consolidacao.Data:yyyy-MM-dd}", consolidacao, TimeSpan.FromHours(1));
        }
    }
}
