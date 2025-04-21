namespace FluxoCaixa.Api.Models.Auth
{
    public class AuthSettings
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string JwtSecret { get; set; } = default!;
    }
}
