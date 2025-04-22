using FluxoCaixa.Dominio.Enuns;
using FluxoCaixa.Dominio.Servicos;
using FluxoCaixa.Dominio.Entidades.Entrada;
using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.Api.Modulos.Lancamento.Models;
using FluxoCaixa.Api.Controllers;
using FluxoCaixa.Dominio.Core.Exceptions;

using Microsoft.AspNetCore.Authorization;

namespace FluxoCaixa.Api.Modulos.Lancamento.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class LancamentoController : BasicController
    {
        private readonly ILancamentoServico _lancamentoServico;

        public LancamentoController(ILancamentoServico lancamentoServico)
        {
            _lancamentoServico = lancamentoServico;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarLancamento([FromBody] LancamentoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resultado = await _lancamentoServico.RegistrarLancamentoAsync(
                    request.Data,
                    request.Quantidade,
                    request.Tipo,
                    request.Descricao
                );

                return Success(resultado, "Lançamento registrado com sucesso.");
            }
            catch (DomainException ex) { return Error($"Erro de domínio ao registrar lançamento: {ex.Message}", System.Net.HttpStatusCode.BadRequest);} 
            catch (Exception ex) { return Error($"Erro inesperado ao registrar lançamento: {ex.Message}", System.Net.HttpStatusCode.InternalServerError); }
        }

        [HttpGet]
        public async Task<IActionResult> ObterPorData([FromQuery] DateTime data)
        {
            try
            {
                var lancamentos = await _lancamentoServico.ObterLancamentosPorDataAsync(data);
                return Ok(lancamentos);
            }
            catch (DomainException ex) { return Error($"Erro de domínio ao obter lançamentos: {ex.Message}", System.Net.HttpStatusCode.BadRequest); }
            catch (Exception ex) { return Error($"Erro inesperado ao obter lançamentos: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);}
        }
    }
}
