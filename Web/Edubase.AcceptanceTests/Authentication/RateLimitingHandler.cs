using System.Diagnostics;

namespace Edubase.AcceptanceTests.Authentication
{
    public class RateLimitingHandler : DelegatingHandler
    {
        private readonly TimeSpan _minInterval;
        private DateTime _lastRequestTime = DateTime.MinValue;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public RateLimitingHandler(TimeSpan minInterval)
        {
            _minInterval = minInterval;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var now = DateTime.UtcNow;
                var timeSinceLastRequest = now - _lastRequestTime;

                if (timeSinceLastRequest < _minInterval)
                {
                    var delay = _minInterval - timeSinceLastRequest;
                    await Task.Delay(delay, cancellationToken);
                }

                _lastRequestTime = DateTime.UtcNow;

                Debug.WriteLine($"[RateLimitingHandler] Handling request to {request.RequestUri}");

                return await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
