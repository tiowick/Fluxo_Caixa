using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FluxoCaixa.Aplicacao.Servicos;
using FluxoCaixa.Dominio.Interfaces.Consolidacao;
using FluxoCaixa.Dominio.Interfaces.Entrada;
using FluxoCaixa.Dominio.Entidades.Consolidacao;
using FluxoCaixa.Dominio.Core.Resilience;
using FluxoCaixa.Dominio.Core.Cache;
using FluxoCaixa.Tests.retry;
using FluxoCaixa.Dominio.Entidades.Entrada;
using FluxoCaixa.Dominio.Enuns;

namespace FluxoCaixa.Tests
{
    public class ConsolidacaoServicoTests
    {
        private readonly Mock<IConsolidacaoDiariaRepositorio> _consolidacaoRepoMock;
        private readonly Mock<IDinheiroEntradaRepositorio> _lancamentoRepoMock;
        private readonly Mock<ILogger<ConsolidacaoServico>> _loggerMock;
        private readonly IRetryPolicy _retryPolicy;
        private readonly Mock<ICache> _cacheMock;
        private readonly ConsolidacaoServico _servico;

        public ConsolidacaoServicoTests()
        {
            _consolidacaoRepoMock = new Mock<IConsolidacaoDiariaRepositorio>();
            _lancamentoRepoMock = new Mock<IDinheiroEntradaRepositorio>();
            _loggerMock = new Mock<ILogger<ConsolidacaoServico>>();
            _retryPolicy = new NoOpRetryPolicy(); // Implementação simples para testes
            _cacheMock = new Mock<ICache>();
            _servico = new ConsolidacaoServico(
                _consolidacaoRepoMock.Object,
                _lancamentoRepoMock.Object,
                _retryPolicy,
                _loggerMock.Object,
                _cacheMock.Object
            );
        }

        [Fact]
        public async Task ObterConsolidadoDiarioAsync_DeveRetornarConsolidadoExistente()
        {
            // Arrange
            var data = DateTime.Today;
            var consolidado = new ConsolidacaoDiaria(data, 100);
            _consolidacaoRepoMock.Setup(r => r.ObterPorDataAsync(data)).ReturnsAsync(consolidado);

            // Act
            var resultado = await _servico.ObterConsolidadoDiarioAsync(data);

            // Assert
            Assert.Same(consolidado, resultado);
            _consolidacaoRepoMock.Verify(r => r.ObterPorDataAsync(data), Times.Once);
        }

        [Fact]
        public async Task ObterConsolidadoDiarioAsync_SeNaoExistir_DeveAtualizarEObter()
        {
            // Arrange
            var data = DateTime.Today;
            var consolidado = new ConsolidacaoDiaria(data, 200);
            int callCount = 0;
            _consolidacaoRepoMock.Setup(r => r.ObterPorDataAsync(data))
                .ReturnsAsync(() =>
                {
                    callCount++;
                    if (callCount <= 2)
                        return null;
                    return consolidado;
                });
            _lancamentoRepoMock.Setup(r => r.ObterSaldoPorDataAsync(data)).ReturnsAsync(200);
            _consolidacaoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<ConsolidacaoDiaria>())).ReturnsAsync(consolidado);

            // Act
            var resultado = await _servico.ObterConsolidadoDiarioAsync(data);

            // Assert
            Assert.Same(consolidado, resultado);
            _consolidacaoRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<ConsolidacaoDiaria>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarConsolidadoDiarioAsync_DeveAdicionarSeNaoExistir()
        {
            // Arrange
            var data = DateTime.Today;
            _consolidacaoRepoMock.Setup(r => r.ObterPorDataAsync(data)).ReturnsAsync((ConsolidacaoDiaria)null);
            _lancamentoRepoMock.Setup(r => r.ObterSaldoPorDataAsync(data)).ReturnsAsync(300);
            _consolidacaoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<ConsolidacaoDiaria>())).ReturnsAsync((ConsolidacaoDiaria c) => c);

            // Act
            await _servico.AtualizarConsolidadoDiarioAsync(data);

            // Assert
            _consolidacaoRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<ConsolidacaoDiaria>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarConsolidadoDiarioAsync_DeveAtualizarSeExistir()
        {
            // Arrange
            var data = DateTime.Today;
            var consolidado = new ConsolidacaoDiaria(data, 400);
            _consolidacaoRepoMock.Setup(r => r.ObterPorDataAsync(data)).ReturnsAsync(consolidado);
            _lancamentoRepoMock.Setup(r => r.ObterSaldoPorDataAsync(data)).ReturnsAsync(500);
            _consolidacaoRepoMock.Setup(r => r.AtualizarAsync(It.IsAny<ConsolidacaoDiaria>())).Returns(Task.CompletedTask);

            // Act
            await _servico.AtualizarConsolidadoDiarioAsync(data);

            // Assert
            _consolidacaoRepoMock.Verify(r => r.AtualizarAsync(It.Is<ConsolidacaoDiaria>(c => c.SaldoConsolidado == 500)), Times.Once);
        }
        
    

        [Fact]
        public async Task ObterConsolidadoDiarioAsync_DeveRetornarDoCache()
        {
            var data = DateTime.Today;
            var consolidado = new ConsolidacaoDiaria(data, 100);
            _cacheMock.Setup(c => c.GetAsync<ConsolidacaoDiaria>(It.IsAny<string>())).ReturnsAsync(consolidado);

            var resultado = await _servico.ObterConsolidadoDiarioAsync(data);

            Assert.Same(consolidado, resultado);
            _consolidacaoRepoMock.Verify(r => r.ObterPorDataAsync(It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task ObterConsolidadoDiarioAsync_DeveRetornarDoRepositorioQuandoCacheVazio()
        {
            var data = DateTime.Today;
            var consolidado = new ConsolidacaoDiaria(data, 200);
            _cacheMock.Setup(c => c.GetAsync<ConsolidacaoDiaria>(It.IsAny<string>())).ReturnsAsync((ConsolidacaoDiaria)null!);
            _consolidacaoRepoMock.Setup(r => r.ObterPorDataAsync(data)).ReturnsAsync(consolidado);

            var resultado = await _servico.ObterConsolidadoDiarioAsync(data);

            Assert.Same(consolidado, resultado);
        }

        [Fact]
        public async Task ObterConsolidadoDiarioAsync_DeveAtualizarSeNaoExistir()
        {
            var data = DateTime.Today;
            _cacheMock.Setup(c => c.GetAsync<ConsolidacaoDiaria>(It.IsAny<string>())).ReturnsAsync((ConsolidacaoDiaria)null!);
            _consolidacaoRepoMock.SetupSequence(r => r.ObterPorDataAsync(data))
                .ReturnsAsync((ConsolidacaoDiaria)null!)
                .ReturnsAsync((ConsolidacaoDiaria)null!)
                .ReturnsAsync(new ConsolidacaoDiaria(data, 300));
            _lancamentoRepoMock.Setup(r => r.ObterSaldoPorDataAsync(data)).ReturnsAsync(300);
            _consolidacaoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<ConsolidacaoDiaria>())).ReturnsAsync(new ConsolidacaoDiaria(data, 300));

            var resultado = await _servico.ObterConsolidadoDiarioAsync(data);

            Assert.NotNull(resultado);
            Assert.Equal(300, resultado.SaldoConsolidado);
        }

        [Fact]
        public async Task AtualizarConsolidadoDiarioAsync_DeveAtualizarCache()
        {
            var data = DateTime.Today;
            var consolidado = new ConsolidacaoDiaria(data, 400);
            _consolidacaoRepoMock.Setup(r => r.ObterPorDataAsync(data)).ReturnsAsync(consolidado);
            _lancamentoRepoMock.Setup(r => r.ObterSaldoPorDataAsync(data)).ReturnsAsync(500);
            _consolidacaoRepoMock.Setup(r => r.AtualizarAsync(It.IsAny<ConsolidacaoDiaria>())).Returns(Task.CompletedTask);

            await _servico.AtualizarConsolidadoDiarioAsync(data);

            _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<ConsolidacaoDiaria>(), It.IsAny<TimeSpan?>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ObterConsolidadoDiarioAsync_DevePropagarExcecaoDoRepositorio()
        {
            var data = DateTime.Today;
            _cacheMock.Setup(c => c.GetAsync<ConsolidacaoDiaria>(It.IsAny<string>())).ReturnsAsync((ConsolidacaoDiaria)null!);
            _consolidacaoRepoMock.Setup(r => r.ObterPorDataAsync(data)).ThrowsAsync(new Exception("Erro no repositório"));

            await Assert.ThrowsAsync<Exception>(async () =>
                await _servico.ObterConsolidadoDiarioAsync(data));
        }
    }
}

