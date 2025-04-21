using FluxoCaixa.Aplicacao.DTOs;
using FluxoCaixa.Dominio.Core.Resilience;
using FluxoCaixa.Dominio.Entidades.Entrada;
using FluxoCaixa.Dominio.Enuns;
using FluxoCaixa.Dominio.Interfaces.Entrada;
using FluxoCaixa.Dominio.Servicos;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Aplicacao.Servicos
{
    public class LancamentoServico : ILancamentoServico
    {
        private readonly IDinheiroEntradaRepositorio _repositorio;
        private readonly IRetryPolicy _retryPolicy;
        private readonly ILogger<LancamentoServico> _logger;

        public LancamentoServico(
            IDinheiroEntradaRepositorio repositorio,
            IRetryPolicy retryPolicy,
            ILogger<LancamentoServico> logger)
        {
            _repositorio = repositorio;
            _retryPolicy = retryPolicy;
            _logger = logger;
        }

        public async Task<DinheiroEntrada> RegistrarLancamentoAsync(DateTime data, decimal quantidade, TipoEntrada tipo, string descricao)
        {
            try
            {
                var lancamento = new DinheiroEntrada(data, quantidade, tipo, descricao);
                
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("Registrando lançamento: {Tipo} de {Quantidade:C} em {Data}", 
                        tipo, quantidade, data.ToShortDateString());
                    
                    return await _repositorio.AdicionarAsync(lancamento);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar lançamento: {Mensagem}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<DinheiroEntrada>> ObterLancamentosPorDataAsync(DateTime data)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("Obtendo lançamentos para a data: {Data}", data.ToShortDateString());
                    return await _repositorio.ObterPorDataAsync(data);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter lançamentos por data: {Mensagem}", ex.Message);
                throw;
            }
        }
    }
}
