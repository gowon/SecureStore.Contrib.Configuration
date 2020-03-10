namespace SecureStore.Contrib.Configuration
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Extension methods for adding <see cref="SecureStoreConfigurationProvider"/>.
    /// </summary>
    public static class SecureStoreConfigurationExtensions
    {
        /// <summary>
        /// Adds the SecureStore configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="key">The SecureStore key.</param>
        /// <param name="keyType">The key type.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddSecureStoreFile(this IConfigurationBuilder builder, string path,
            string key, KeyType keyType)
        {
            return AddSecureStoreFile(builder, path, key, keyType, false);
        }

        /// <summary>
        /// Adds the SecureStore configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="key">The SecureStore key.</param>
        /// <param name="keyType">The key type.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddSecureStoreFile(this IConfigurationBuilder builder, string path,
            string key, KeyType keyType, bool optional)
        {
            return AddSecureStoreFile(builder, path, key, keyType, optional, false);
        }

        /// <summary>
        /// Adds the SecureStore configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="key">The SecureStore key.</param>
        /// <param name="keyType">The key type.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddSecureStoreFile(this IConfigurationBuilder builder, string path,
            string key, KeyType keyType, bool optional,
            bool reloadOnChange)
        {
            return AddSecureStoreFile(builder, null, path, key, keyType, optional, reloadOnChange);
        }

        /// <summary>
        /// Adds a SecureStore configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the file.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="key">The SecureStore key.</param>
        /// <param name="keyType">The key type.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
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

            return builder.AddSecureStoreFile(source =>
            {
                source.FileProvider = provider;
                source.Path = path;
                source.Key = key;
                source.KeyType = keyType;
                source.Optional = optional;
                source.ReloadOnChange = reloadOnChange;
            });
        }

        /// <summary>
        /// Adds a SecureStore configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddSecureStoreFile(this IConfigurationBuilder builder, Action<SecureStoreConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}