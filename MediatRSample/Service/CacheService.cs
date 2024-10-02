using Microsoft.Extensions.Caching.Hybrid;

namespace MediatRSample.Service
{
    public class CacheService
    {
        private readonly HybridCache _cache;

        public CacheService(HybridCache cache)
        {
            _cache = cache;
        }
        public async Task<string> GetSomeInfoAsync(string name, int id, CancellationToken token = default)
        {
            return await _cache.GetOrCreateAsync(
                $"{name}-{id}",
                async cancel => await GetDataFromTheSourceAsync(name, id, cancel),
                cancellationToken: token
            );
        }

        private async Task<string> GetDataFromTheSourceAsync(string name, int id, CancellationToken token)
        {
            return $"someinfo-{name}-{id}";
        }

    }
}
