using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using OSK.Functions.Outputs.Abstractions;
using OSK.Functions.Outputs.Logging;
using OSK.Functions.Outputs.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;
using OSK.UML;
using OSK.UML.CommandLine.Commands;
using OSK.UML.Exporters.PlantUML;

using var cts = new CancellationTokenSource();
int exitCode;
var finished = false;

AppDomain.CurrentDomain.ProcessExit += (_, _) =>
{
    if (!finished)
    {
        Cancel();
    }
};
Console.CancelKeyPress += (_, e) =>
{
    Cancel();
    e.Cancel = true;
};

try
{
    var generationResult = await Parser.Default.ParseArguments<GenerateUmlOptions>(args)
        .MapResult(
           (GenerateUmlOptions options) =>
           {
               var provider = GetServiceProvider(verboseLogging: options.EnableVerboseLogging, jarFilePath: options.PlantUmlPathOverride);
               return ActivatorUtilities.CreateInstance<GenerateUmlCommand>(provider, options)
                .ExecuteAsync(cts.Token);
           },
           errs =>
           {
               var provider = GetServiceProvider(verboseLogging: false, jarFilePath: string.Empty);
               var outputFactory = provider.GetRequiredService<IOutputFactory<Program>>();
               var errors = string.Join(",", errs.Select(e => e.Tag));
               return Task.FromResult(outputFactory.Error(HttpStatusCode.InternalServerError, $"There was an issue parsing the execution command. Errors: {errors}"));
           });
    if (!generationResult.IsSuccessful)
    {
        Console.WriteLine($"There was an error with parsing the input arguments. Error: {generationResult.GetErrorString()}");
    }

    exitCode = generationResult.IsSuccessful
        ? 0
        : 1;

    finished = true;
}
catch (OperationCanceledException)
{
    exitCode = 2;
}

return exitCode;

void Cancel()
{
    if (!cts.IsCancellationRequested)
    {
        Console.WriteLine("Cancelling...");
        cts.Cancel();
    }
}

IServiceProvider GetServiceProvider(bool verboseLogging, string? jarFilePath)
{
    var serviceCollection = new ServiceCollection();
    serviceCollection
        .AddUmlDiagrams()
        .AddPlantUml(o =>
        {
            o.JarFilePath = jarFilePath ?? Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "plantuml-mit-1.2024.5.jar")
             ?? throw new InvalidOperationException("No valid jar file path could be found.");

            var jarPathGiven = string.IsNullOrWhiteSpace(jarFilePath)
             ? "No jar path was provided, using the default. "
             : string.Empty;
            Console.WriteLine($"{jarPathGiven}Using jar path: {o.JarFilePath}");
        })
        .AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(verboseLogging ? LogLevel.Debug : LogLevel.Information);
        })
        .AddLoggingFunctionOutputs();

    return serviceCollection.BuildServiceProvider();
}