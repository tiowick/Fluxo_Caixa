using FluxoCaixa.Dominio.Enuns;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluxoCaixa.Api.Modulos.Lancamento.Models
{
    public class LancamentoRequest
    {
        [Required]
        public DateTime Data { get; set; }
        [Required]
        public decimal Quantidade { get; set; }
        [Required]
        public TipoEntrada Tipo { get; set; }
        [Required]
        public string Descricao { get; set; } = string.Empty;
    }
}
