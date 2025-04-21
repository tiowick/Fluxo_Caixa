namespace FluxoCaixa.Aplicacao.DTOs
{
    public class ConsolidadoDiarioDTO
    {
        public Guid Id { get; set; }
        public DateTime Data { get; set; }
        public decimal SaldoConsolidado { get; set; }
    }
}
