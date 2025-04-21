using FluxoCaixa.Dominio.Entidades.Entrada;
using FluxoCaixa.Dominio.Enuns;

namespace FluxoCaixa.Dominio.Servicos
{
    public interface ILancamentoServico
    {
        Task<DinheiroEntrada> RegistrarLancamentoAsync(DateTime data, decimal quantidade, TipoEntrada tipo, string descricao);
        Task<IEnumerable<DinheiroEntrada>> ObterLancamentosPorDataAsync(DateTime data);
    }
}
