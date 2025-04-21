using FluxoCaixa.Dominio.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace FluxoCaixa.Api.Controllers
{
    /// <summary>
    /// Controller base para respostas customizadas de sucesso e erro.
    /// </summary>
    public class BasicController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string? message = null)
        {
            return Ok(new {
                success = true,
                message = message ?? "Operação realizada com sucesso.",
                data
            });
        }

        protected IActionResult Success(string? message = null)
        {
            return Ok(new {
                success = true,
                message = message ?? "Operação realizada com sucesso."
            });
        }

        protected IActionResult Error(string message, HttpStatusCode status = HttpStatusCode.BadRequest)
        {
            return StatusCode((int)status, new {
                success = false,
                message
            });
        }


    }
}
