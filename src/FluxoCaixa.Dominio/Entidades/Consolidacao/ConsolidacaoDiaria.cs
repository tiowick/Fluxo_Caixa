using FluxoCaixa.Dominio.Core.Exceptions;
using System;

namespace FluxoCaixa.Dominio.Entidades.Consolidacao
{
    public class ConsolidacaoDiaria : Entity
    {
        protected ConsolidacaoDiaria() { } // Para o EF Core

        public ConsolidacaoDiaria(DateTime data, decimal saldoConsolidado)
        {
            if (data == default)
                throw new DomainException("Data inválida");

            if (data.Date > DateTime.Today)
                throw new DomainException("Não é permitido consolidar ou consultar datas futuras.");


            Data = data;
            SaldoConsolidado = saldoConsolidado;
        }

        public DateTime Data { get; private set; }
        public decimal SaldoConsolidado { get; private set; }

        public void AtualizarSaldo(decimal novoSaldo)
        {
            SaldoConsolidado = novoSaldo;
        }
    }
}
