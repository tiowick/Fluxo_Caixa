using FluxoCaixa.Dominio.Entidades.Consolidacao;

namespace FluxoCaixa.Dominio.Servicos
{
    public interface IConsolidacaoServico
    {
        Task<ConsolidacaoDiaria> ObterConsolidadoDiarioAsync(DateTime data);
        Task AtualizarConsolidadoDiarioAsync(DateTime data);
    }
}
