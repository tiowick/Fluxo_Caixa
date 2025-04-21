using FluxoCaixa.Dominio.Core.Resilience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluxoCaixa.Tests.retry
{
    // Implementação de RetryPolicy para testes
    public class NoOpRetryPolicy : IRetryPolicy
    {
        public Task ExecuteAsync(Func<Task> action) => action();
        public Task<T> ExecuteAsync<T>(Func<Task<T>> action) => action();
    }
}
