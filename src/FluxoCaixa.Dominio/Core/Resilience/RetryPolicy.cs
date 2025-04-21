namespace FluxoCaixa.Dominio.Core.Resilience
{
    public class RetryPolicy : IRetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _delay;

        public RetryPolicy(int maxRetries = 3, int delayMilliseconds = 100)
        {
            _maxRetries = maxRetries;
            _delay = TimeSpan.FromMilliseconds(delayMilliseconds);
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            for (int i = 0; i < _maxRetries; i++)
            {
                try
                {
                    return await action();
                }
                catch (Exception) when (i < _maxRetries - 1)
                {
                    await Task.Delay(_delay);
                }
            }

            return await action(); // Última tentativa
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            for (int i = 0; i < _maxRetries; i++)
            {
                try
                {
                    await action();
                    return;
                }
                catch (Exception) when (i < _maxRetries - 1)
                {
                    await Task.Delay(_delay);
                }
            }

            await action(); // Última tentativa
        }
    }
}
