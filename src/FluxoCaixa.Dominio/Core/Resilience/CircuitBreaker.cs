namespace FluxoCaixa.Dominio.Core.Resilience
{
    public class CircuitBreaker
    {
        private readonly int _failureThreshold;
        private readonly TimeSpan _resetTimeout;
        private int _failureCount;
        private DateTime? _lastFailureTime;
        private bool _isOpen;

        public CircuitBreaker(int failureThreshold = 5, int resetTimeoutSeconds = 60)
        {
            _failureThreshold = failureThreshold;
            _resetTimeout = TimeSpan.FromSeconds(resetTimeoutSeconds);
            _failureCount = 0;
            _lastFailureTime = null;
            _isOpen = false;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            if (_isOpen)
            {
                if (_lastFailureTime.HasValue && DateTime.UtcNow - _lastFailureTime.Value > _resetTimeout)
                {
                    _isOpen = false;
                    _failureCount = 0;
                }
                else
                {
                    throw new InvalidOperationException("Circuit breaker está aberto");
                }
            }

            try
            {
                var result = await action();
                _failureCount = 0;
                return result;
            }
            catch (Exception)
            {
                _failureCount++;
                _lastFailureTime = DateTime.UtcNow;

                if (_failureCount >= _failureThreshold)
                {
                    _isOpen = true;
                }

                throw;
            }
        }
    }
}
