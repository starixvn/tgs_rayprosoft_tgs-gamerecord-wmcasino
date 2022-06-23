using Tgs.GameRecord.WMCasinoService;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameRecordWMCasinoService(
            this IServiceCollection services,
            Action<RedisOptions, IServiceProvider> redisDBconfigure)
            => services
                .AddOptions<RedisOptions>()
                .Configure(redisDBconfigure)
                .Services
                .AddSingleton<RedisConnectionManager>();
    }
}
