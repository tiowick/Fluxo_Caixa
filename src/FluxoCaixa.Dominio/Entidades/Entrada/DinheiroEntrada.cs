using FluxoCaixa.Dominio.Core.Exceptions;
using FluxoCaixa.Dominio.Enuns;
using System;

namespace FluxoCaixa.Dominio.Entidades.Entrada
{
    public class DinheiroEntrada : Entity
    {
        protected DinheiroEntrada() { } // Para o EF Core

        public DinheiroEntrada(DateTime data, decimal quantidade, TipoEntrada tipo, string descricao)
        {
            ValidarEntrada(data, quantidade, descricao);
            
            Data = data;
            Quantidade = quantidade;
            Tipo = tipo;
            Descricao = descricao;
        }

        public DateTime Data { get; private set; }
        public decimal Quantidade { get; private set; }
        public TipoEntrada Tipo { get; private set; }
        public string Descricao { get; private set; } = string.Empty;

        private void ValidarEntrada(DateTime data, decimal quantidade, string descricao)
        {
            if (data == default)
                throw new DomainException("Data inválida");

            if (quantidade <= 0)
                throw new DomainException("Quantidade deve ser maior que zero");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new DomainException("Descrição é obrigatória");

            if (data.Date > DateTime.Today)
                throw new DomainException("Não é permitido registrar lançamentos para datas futuras.");
        }

        public decimal ObterValorCalculado()
        {
            return Tipo == TipoEntrada.credito ? Quantidade : -Quantidade;
        }
    }
}
