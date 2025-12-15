using Microsoft.Extensions.DependencyInjection;

namespace OpenPix.AspNetCore;

public static class OpenPixServiceCollectionExtensions
{
    public static IServiceCollection AddOpenPix(
        this IServiceCollection services,
        Action<OpenPixOptions> configureOptions)
    {
        // 1. Registra as opções
        services.Configure(configureOptions);

        // 2. Registra o serviço como Singleton (pois as configs não mudam por request)
        services.AddSingleton<IPixClient, PixClient>();

        return services;
    }
}