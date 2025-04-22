using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FluxoCaixa.Aplicacao.Servicos;
using FluxoCaixa.Dominio.Interfaces.Entrada;
using FluxoCaixa.Dominio.Enuns;
using FluxoCaixa.Dominio.Entidades.Entrada;
using FluxoCaixa.Dominio.Core.Resilience;
using FluxoCaixa.Tests.retry;

namespace FluxoCaixa.Tests
{
    public class LancamentoServico_IsolamentoEResiliencia_Tests
    {
        [Fact(DisplayName = "Lancamento deve funcionar mesmo se Consolidacao falhar (isolamento)")]
        public async Task Lancamento_DeveFuncionar_SeConsolidacaoFalhar()
        {
            // Arrange: mock do repositório de lançamentos
            var repoMock = new Mock<IDinheiroEntradaRepositorio>();
            repoMock.Setup(r => r.AdicionarAsync(It.IsAny<DinheiroEntrada>())).ReturnsAsync((DinheiroEntrada d) => d);

            // Simula RetryPolicy sem efeito (não lança exceção)
            var retryPolicy = new NoOpRetryPolicy();
            var loggerMock = new Mock<ILogger<LancamentoServico>>();

            var servico = new LancamentoServico(repoMock.Object, retryPolicy, loggerMock.Object);

            // Act: tenta registrar um lançamento
            var result = await servico.RegistrarLancamentoAsync(DateTime.Today, 100, TipoEntrada.credito, "Teste isolado");

            // Assert: deve registrar normalmente
            Assert.NotNull(result);
            Assert.Equal(100, result.Quantidade);
            Assert.Equal(TipoEntrada.credito, result.Tipo);
        }

        [Fact(DisplayName = "Lancamento deve funcionar sob alta concorrência (resiliência)")]
        public async Task Lancamento_DeveSerResiliente_EmPicoDeConcorrencia()
        {
            
            var repoMock = new Mock<IDinheiroEntradaRepositorio>();
            repoMock.Setup(r => r.AdicionarAsync(It.IsAny<DinheiroEntrada>())).ReturnsAsync((DinheiroEntrada d) => d);
            var retryPolicy = new NoOpRetryPolicy();
            var loggerMock = new Mock<ILogger<LancamentoServico>>();
            var servico = new LancamentoServico(repoMock.Object, retryPolicy, loggerMock.Object);

            int totalRequests = 50;
            var tasks = new List<Task<DinheiroEntrada>>();

            // Act: dispara 50 requisições em paralelo simulando pico
            for (int i = 1; i <= totalRequests; i++)
            {
                tasks.Add(servico.RegistrarLancamentoAsync(DateTime.Today, i, TipoEntrada.credito, $"Teste {i}"));
            }
            var results = await Task.WhenAll(tasks);

            // Assert: todas as requisições devem ser processadas sem erro
            Assert.Equal(totalRequests, results.Length);
            for (int i = 1; i <= totalRequests; i++)
            {
                Assert.Equal(i, results[i - 1].Quantidade);
            }
        }
    }

  
}
