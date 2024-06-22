using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OSK.UML.Exporters.PlantUML.Internal.Services;
using OSK.UML.Exporters.PlantUML.Options;
using OSK.UML.Ports;
using System;

namespace OSK.UML.Exporters.PlantUML
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlantUml(this IServiceCollection services,
            Action<PlantUmlOptions> configure)
        {
            services.TryAddTransient<IUmlExporter, PlantUmlExporter>();
            services.Configure(configure);

            return services;
        }
    }
}
