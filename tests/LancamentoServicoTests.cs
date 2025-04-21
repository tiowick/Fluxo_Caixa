using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FluxoCaixa.Dominio.Core.Resilience;
using FluxoCaixa.Aplicacao.Servicos;
using FluxoCaixa.Dominio.Interfaces.Entrada;
using FluxoCaixa.Dominio.Entidades.Entrada;
using FluxoCaixa.Dominio.Enuns;

namespace FluxoCaixa.Tests
{
    public class LancamentoServicoTests
    {
        private readonly Mock<IDinheiroEntradaRepositorio> _repoMock;
        private readonly Mock<IRetryPolicy> _retryPolicyMock;
        private readonly Mock<ILogger<LancamentoServico>> _loggerMock;
        private readonly LancamentoServico _servico;

        public LancamentoServicoTests()
        {
            _repoMock = new Mock<IDinheiroEntradaRepositorio>();
            _retryPolicyMock = new Mock<IRetryPolicy>();
            _loggerMock = new Mock<ILogger<LancamentoServico>>();
            _servico = new LancamentoServico(_repoMock.Object, _retryPolicyMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task RegistrarLancamentoAsync_DeveAdicionarComSucesso()
        {
            // Arrange
            var data = DateTime.Today;
            var quantidade = 100m;
            var tipo = TipoEntrada.credito;
            var descricao = "Teste";
            var lancamento = new DinheiroEntrada(data, quantidade, tipo, descricao);
            _retryPolicyMock.Setup(r => r.ExecuteAsync(It.IsAny<Func<Task<DinheiroEntrada>>>()))
                .Returns<Func<Task<DinheiroEntrada>>>(f => f());
            _repoMock.Setup(r => r.AdicionarAsync(It.IsAny<DinheiroEntrada>())).ReturnsAsync(lancamento);

            // Act
            var resultado = await _servico.RegistrarLancamentoAsync(data, quantidade, tipo, descricao);

            // Assert
            Assert.Equal(lancamento, resultado);
            _repoMock.Verify(r => r.AdicionarAsync(It.IsAny<DinheiroEntrada>()), Times.Once);
        }

        [Fact]
        public async Task ObterLancamentosPorDataAsync_DeveRetornarLista()
        {
            // Arrange
            var data = DateTime.Today;
            var lista = new List<DinheiroEntrada> { new DinheiroEntrada(data, 50, TipoEntrada.debito, "Desc") };
            _retryPolicyMock.Setup(r => r.ExecuteAsync(It.IsAny<Func<Task<IEnumerable<DinheiroEntrada>>>>()))
                .Returns<Func<Task<IEnumerable<DinheiroEntrada>>>>(f => f());
            _repoMock.Setup(r => r.ObterPorDataAsync(data)).ReturnsAsync(lista);

            // Act
            var resultado = await _servico.ObterLancamentosPorDataAsync(data);

            // Assert
            Assert.Equal(lista, resultado);
            _repoMock.Verify(r => r.ObterPorDataAsync(data), Times.Once);
        }

        [Fact]
        public async Task LancamentoServico_DeveFuncionarMesmoSeConsolidadoEstiverIndisponivel()
        {
            // Arrange
            var data = DateTime.Today;
            var quantidade = 100m;
            var tipo = TipoEntrada.credito;
            var descricao = "Teste";

            // Simula o repositório de lançamentos funcionando normalmente
            var lancamento = new DinheiroEntrada(data, quantidade, tipo, descricao);
            _retryPolicyMock.Setup(r => r.ExecuteAsync(It.IsAny<Func<Task<DinheiroEntrada>>>()))
                .Returns<Func<Task<DinheiroEntrada>>>(f => f());
            _repoMock.Setup(r => r.AdicionarAsync(It.IsAny<DinheiroEntrada>())).ReturnsAsync(lancamento);

            // Act
            var resultado = await _servico.RegistrarLancamentoAsync(data, quantidade, tipo, descricao);

            // Assert
            Assert.Equal(lancamento, resultado);
            _repoMock.Verify(r => r.AdicionarAsync(It.IsAny<DinheiroEntrada>()), Times.Once);
        }
    }
}
