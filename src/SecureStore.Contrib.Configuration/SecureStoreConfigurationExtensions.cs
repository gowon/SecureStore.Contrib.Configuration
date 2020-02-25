namespace SecureStore.Contrib.Configuration
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.FileProviders;

    public static class SecureStoreConfigurationExtensions
    {
        public static IConfigurationBuilder AddSecureStoreFile(this IConfigurationBuilder builder, string path,
            string key, KeyType keyType)
        {
            return AddSecureStoreFile(builder, path, key, keyType, false);
        }

        public static IConfigurationBuilder AddSecureStoreFile(this IConfigurationBuilder builder, string path,
            string key, KeyType keyType, bool optional)
        {
            return AddSecureStoreFile(builder, path, key, keyType, optional, false);
        }

        public static IConfigurationBuilder AddSecureStoreFile(this IConfigurationBuilder builder, string path,
            string key, KeyType keyType, bool optional,
            bool reloadOnChange)
        {
            return AddSecureStoreFile(builder, null, path, key, keyType, optional, reloadOnChange);
        }

        public static IConfigurationBuilder AddSecureStoreFile(this IConfigurationBuilder builder,
            IFileProvider provider,
            string path, string key, KeyType keyType, bool optional, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string.", nameof(path));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("File key/path must be a non-empty string.", nameof(key));
            }

            if (provider == null && Path.IsPathRooted(path))
            {
                provider = new PhysicalFileProvider(Path.GetDirectoryName(path));
                path = Path.GetFileName(path);
            }

            var source = new SecureStoreConfigurationSource
            {
                FileProvider = provider,
                Path = path,
                Key = key,
                KeyType = keyType,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            };

            builder.Add(source);
            return builder;
        }
    }
}