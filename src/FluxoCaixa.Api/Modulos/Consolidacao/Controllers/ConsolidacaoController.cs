using FluxoCaixa.Dominio.Servicos;
using FluxoCaixa.Dominio.Entidades.Consolidacao;
using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.Api.Controllers;
using FluxoCaixa.Dominio.Core.Exceptions;

using Microsoft.AspNetCore.Authorization;

namespace FluxoCaixa.Api.Modulos.Consolidacao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConsolidacaoController : BasicController
    {
        private readonly IConsolidacaoServico _consolidacaoServico;

        public ConsolidacaoController(IConsolidacaoServico consolidacaoServico)
        {
            _consolidacaoServico = consolidacaoServico;
        }

        [HttpPost("atualizar")]
        public async Task<IActionResult> AtualizarConsolidado([FromQuery] DateTime data)
        {
            try
            {
                await _consolidacaoServico.AtualizarConsolidadoDiarioAsync(data);
                return Success("Consolidação atualizada com sucesso.");
            }
            catch (DomainException ex) { return Error($"Erro ao atualizar consolidação: {ex.Message}", System.Net.HttpStatusCode.BadRequest); }
            catch (Exception ex) { return Error($"Erro ao atualizar consolidação: {ex.Message}", System.Net.HttpStatusCode.InternalServerError); }
        }

        [HttpGet]
        public async Task<IActionResult> ObterPorData([FromQuery] DateTime data)
        {
            try
            {
                var consolidado = await _consolidacaoServico.ObterConsolidadoDiarioAsync(data);
                if (consolidado == null)
                    return Error("Consolidação não encontrada.", System.Net.HttpStatusCode.NotFound);
                return Success(consolidado, "Consolidação obtida com sucesso.");
            }
            catch (DomainException ex) { return Error($"Erro inesperado ao obter consolidação: {ex.Message}", System.Net.HttpStatusCode.BadRequest);}
            catch (Exception ex){ return Error($"Erro inesperado ao obter consolidação: {ex.Message}", System.Net.HttpStatusCode.InternalServerError); }
        }
    }
}
