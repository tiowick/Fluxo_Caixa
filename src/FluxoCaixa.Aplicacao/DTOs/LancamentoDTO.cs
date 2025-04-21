using FluxoCaixa.Dominio.Enuns;

namespace FluxoCaixa.Aplicacao.DTOs
{
    public class LancamentoDTO
    {
        public Guid Id { get; set; }
        public DateTime Data { get; set; }
        public decimal Quantidade { get; set; }
        public TipoEntrada Tipo { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorCalculado { get; set; }
    }
}
