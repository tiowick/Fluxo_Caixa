using FluxoCaixa.Dominio.Core.Cache;
using FluxoCaixa.Dominio.Core.Exceptions;
using FluxoCaixa.Dominio.Core.Resilience;
using FluxoCaixa.Dominio.Entidades.Consolidacao;
using FluxoCaixa.Dominio.Interfaces.Consolidacao;
using FluxoCaixa.Dominio.Interfaces.Entrada;
using FluxoCaixa.Dominio.Servicos;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Aplicacao.Servicos
{
    public class ConsolidacaoServico : IConsolidacaoServico
    {
        private readonly IConsolidacaoDiariaRepositorio _consolidacaoRepositorio;
        private readonly IDinheiroEntradaRepositorio _lancamentoRepositorio;
        private readonly IRetryPolicy _retryPolicy;
        private readonly CircuitBreaker _circuitBreaker;
        private readonly ILogger<ConsolidacaoServico> _logger;
        private readonly ICache _cache;

        // Padrão de chave para o cache
        private static string GetCacheKey(DateTime data) => $"consolidado_diario_{data:yyyyMMdd}";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

        public ConsolidacaoServico(
            IConsolidacaoDiariaRepositorio consolidacaoRepositorio, IDinheiroEntradaRepositorio lancamentoRepositorio,
            IRetryPolicy retryPolicy, ILogger<ConsolidacaoServico> logger, ICache cache)
        {
            _consolidacaoRepositorio = consolidacaoRepositorio;
            _lancamentoRepositorio = lancamentoRepositorio;
            _retryPolicy = retryPolicy;
            _circuitBreaker = new CircuitBreaker(5, 60); // 5 falhas em 60 segundos
            _logger = logger;
            _cache = cache;
        }

        public async Task<ConsolidacaoDiaria> ObterConsolidadoDiarioAsync(DateTime data)
        {
          
            try
            {
                string cacheKey = GetCacheKey(data);
                var consolidadoCache = await _cache.GetAsync<ConsolidacaoDiaria>(cacheKey);
                if (consolidadoCache != null)
                {
                    _logger.LogInformation("Consolidado diário encontrado no cache para {Data}", data.ToShortDateString());
                    return consolidadoCache;
                }

                return await _circuitBreaker.ExecuteAsync(async () =>
                {
                    var consolidado = await _consolidacaoRepositorio.ObterPorDataAsync(data);
                    if (consolidado == null)
                    {
                        _logger.LogInformation("Consolidado não encontrado para {Data}, gerando novo...", 
                            data.ToShortDateString());
                        await AtualizarConsolidadoDiarioAsync(data);
                        consolidado = await _consolidacaoRepositorio.ObterPorDataAsync(data);
                    }
                    // Atualiza o cache
                    await _cache.SetAsync(cacheKey, consolidado, CacheExpiration);
                    return consolidado!;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter consolidado diário: {Mensagem}", ex.Message);
                throw;
            }
        }

        public async Task AtualizarConsolidadoDiarioAsync(DateTime data)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("Atualizando consolidado para a data: {Data}", 
                        data.ToShortDateString());

                    var saldo = await _lancamentoRepositorio.ObterSaldoPorDataAsync(data);
                    var consolidado = await _consolidacaoRepositorio.ObterPorDataAsync(data);

                    if (consolidado == null)
                    {
                        consolidado = new ConsolidacaoDiaria(data, saldo);
                        await _consolidacaoRepositorio.AdicionarAsync(consolidado);
                    }
                    else
                    {
                        consolidado.AtualizarSaldo(saldo);
                        await _consolidacaoRepositorio.AtualizarAsync(consolidado);
                    }

                    // Atualiza o cache após alteração
                    string cacheKey = GetCacheKey(data);
                    await _cache.SetAsync(cacheKey, consolidado, CacheExpiration);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar consolidado diário: {Mensagem}", ex.Message);
                throw;
            }
        }
    }
}
