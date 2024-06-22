using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OSK.UML.Internal.Services;
using OSK.UML.Ports;

namespace OSK.UML
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUmlDiagrams(this IServiceCollection services)
        {
            services.TryAddTransient<IUmlParserFactory, UmlParserFactory>();
            services.TryAddTransient<IUmlGenerator, DefaultUmlGenerator>();

            return services;
        }
    }
}
