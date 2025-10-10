using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyNuget.Azure.AppConfiguration;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class AppConfigurationExtensions
{
    public static IHostApplicationBuilder AddMyAppConfiguration(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptionsWithValidateOnStart<MyAppConfigurationOptions>()
            .BindConfiguration(MyAppConfigurationOptions.SectionName);

        var myAppConfigurationOptions = new MyAppConfigurationOptions();
        builder.Configuration.Bind(MyAppConfigurationOptions.SectionName, myAppConfigurationOptions);

        if (myAppConfigurationOptions.Endpoint?.IsAbsoluteUri ?? false)
        {
            builder.Services.AddAzureAppConfiguration();

            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                var azureCredential = new DefaultAzureCredential();
                options
                    .Connect(myAppConfigurationOptions.Endpoint, azureCredential)
                    .ConfigureKeyVault(kv => kv.SetCredential(azureCredential));

                if (myAppConfigurationOptions.SharedPrefix is string sharedPrefix)
                {
                    options.Select($"{sharedPrefix}-*");
                    options.TrimKeyPrefix($"{sharedPrefix}-*");
                }

                if (myAppConfigurationOptions.ApplicationPrefix is string applicationPrefix)
                {
                    options.Select($"{applicationPrefix}-*");
                    options.TrimKeyPrefix($"{applicationPrefix}-*");
                }

                options.ConfigureRefresh(refreshOptions =>
                {
                    refreshOptions
                        .RegisterAll()
                        .SetRefreshInterval(myAppConfigurationOptions.RefreshInterval);
                })
                .UseFeatureFlags(featureFlagOptions =>
                {
                    featureFlagOptions.SetRefreshInterval(myAppConfigurationOptions.RefreshInterval);
                });
            });
        }

        return builder;
    }
}
