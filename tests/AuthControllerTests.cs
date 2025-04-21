using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
// Garante que Program seja encontrado

namespace FluxoCaixa.Tests
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact(DisplayName = "Deve retornar token válido ao logar com credenciais corretas")]
        public async Task Login_DeveRetornarTokenComCredenciaisCorretas()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "admin", password = "senha123" });
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", json);
            Assert.DoesNotContain("null", json);
        }

        [Fact(DisplayName = "Deve negar login com credenciais inválidas")]
        public async Task Login_DeveNegarComCredenciaisInvalidas()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "admin", password = "errado" });
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact(DisplayName = "Deve negar acesso a endpoint protegido sem token")]
        public async Task EndpointProtegido_DeveNegarSemToken()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/lancamento");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("autenticado", content, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "Deve acessar endpoint protegido com token válido")]
        public async Task EndpointProtegido_DevePermitirComTokenValido()
        {
            var client = _factory.CreateClient();
            // Login para pegar token
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { username = "admin", password = "senha123" });
            var loginJson = await loginResponse.Content.ReadAsStringAsync();
            var token = JsonDocument.Parse(loginJson).RootElement.GetProperty("token").GetString();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("/api/lancamento");
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}

