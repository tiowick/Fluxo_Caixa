using FluxoCaixa.Dominio.Entidades.Entrada;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluxoCaixa.Dominio.Interfaces.Entrada
{
    public interface IDinheiroEntradaRepositorio
    {
        /// <summary>Adiciona um lançamento</summary>
        Task<DinheiroEntrada> AdicionarAsync(DinheiroEntrada entrada);

        /// <summary>Retorna todos os lançamentos de uma dada data</summary>
        Task<IEnumerable<DinheiroEntrada>> ObterPorDataAsync(DateTime data);

        /// <summary>Obtém o saldo calculado para uma data</summary>
        Task<decimal> ObterSaldoPorDataAsync(DateTime data);
    }
}
