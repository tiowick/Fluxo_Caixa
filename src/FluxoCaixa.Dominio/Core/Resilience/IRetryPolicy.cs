namespace FluxoCaixa.Dominio.Core.Resilience
{
    public interface IRetryPolicy
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
        Task ExecuteAsync(Func<Task> action);
    }
}
