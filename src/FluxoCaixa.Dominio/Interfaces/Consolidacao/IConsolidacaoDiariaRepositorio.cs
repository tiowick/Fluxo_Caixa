using FluxoCaixa.Dominio.Entidades.Consolidacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluxoCaixa.Dominio.Interfaces.Consolidacao
{
    public interface IConsolidacaoDiariaRepositorio
    {
        /// <summary>Obtém o consolidado de uma data</summary>
        Task<ConsolidacaoDiaria?> ObterPorDataAsync(DateTime data);

        /// <summary>Adiciona novo registro de consolidação</summary>
        Task<ConsolidacaoDiaria> AdicionarAsync(ConsolidacaoDiaria consolidacao);

        /// <summary>Atualiza um registro de consolidação existente</summary>
        Task AtualizarAsync(ConsolidacaoDiaria consolidacao);
    }
}
