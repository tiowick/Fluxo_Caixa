using FluxoCaixa.Dominio.Entidades.Entrada;
using FluxoCaixa.Dominio.Interfaces.Entrada;
using FluxoCaixa.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixa.Infraestrutura.Repositorios
{
    public class DinheiroEntradaRepositorio : IDinheiroEntradaRepositorio
    {
        private readonly FluxoCaixaContexto _contexto;

        public DinheiroEntradaRepositorio(FluxoCaixaContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<DinheiroEntrada> AdicionarAsync(DinheiroEntrada entrada)
        {
            await _contexto.Lancamentos.AddAsync(entrada);
            await _contexto.SaveChangesAsync();
            return entrada;
        }

        public async Task<IEnumerable<DinheiroEntrada>> ObterPorDataAsync(DateTime data)
        {
            return await _contexto.Lancamentos
                .Where(x => x.Data.Date == data.Date)
                .OrderBy(x => x.Data)
                .ToListAsync();
        }

        public async Task<decimal> ObterSaldoPorDataAsync(DateTime data)
        {
            var lancamentos = await ObterPorDataAsync(data);
            return lancamentos.Sum(x => x.ObterValorCalculado());
        }
    }
}
