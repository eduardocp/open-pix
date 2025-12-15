using Microsoft.Extensions.DependencyInjection;

namespace OpenPix.AspNetCore;

public static class OpenPixServiceCollectionExtensions
{
    public static IServiceCollection AddOpenPix(
        this IServiceCollection services,
        Action<OpenPixOptions> configureOptions)
    {
        // 1. Register options
        services.Configure(configureOptions);

        // 2. Register the service as Singleton (since configs don't change per request)
        services.AddSingleton<IPixClient, PixClient>();

        return services;
    }
}