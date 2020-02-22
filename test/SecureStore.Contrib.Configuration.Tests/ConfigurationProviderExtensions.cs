namespace SecureStore.Contrib.Configuration.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.Configuration;

    [ExcludeFromCodeCoverage]
    public static class ConfigurationProviderExtensions
    {
        public static string Get(this IConfigurationProvider provider, string key)
        {
            string value;

            if (!provider.TryGet(key, out value))
            {
                throw new InvalidOperationException("Key not found");
            }

            return value;
        }
    }
}