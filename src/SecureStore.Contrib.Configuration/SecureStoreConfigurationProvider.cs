namespace SecureStore.Contrib.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using NeoSmart.SecureStore;

    /// <summary>
    ///     A SecureStore file based <see cref="FileConfigurationProvider" />.
    /// </summary>
    public class SecureStoreConfigurationProvider : FileConfigurationProvider
    {
        /// <summary>
        ///     Initializes a new instance with the specified source.
        /// </summary>
        /// <param name="source">The source settings.</param>
        public SecureStoreConfigurationProvider(SecureStoreConfigurationSource source) : base(source)
        {
        }

        /// <summary>
        ///     Loads the SecureStore data from a stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        public override void Load(Stream stream)
        {
            var source = (SecureStoreConfigurationSource) Source;
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var manager = SecretsManager.LoadStore(stream))
            {
                switch (source.KeyType)
                {
                    case KeyType.File:
                        // ref: https://github.com/aspnet/Configuration/blob/master/src/Config.FileExtensions/FileConfigurationProvider.cs#L48
                        var file = source.KeyFileProvider?.GetFileInfo(source.Key);
                        if (file == null || !file.Exists)
                        {
                            var error = new StringBuilder(
                                $"The configuration key file '{source.Key}' was not found and is not optional.");
                            if (!string.IsNullOrEmpty(file?.PhysicalPath))
                            {
                                error.Append($" The physical path is '{file.PhysicalPath}'.");
                            }

                            throw new FileNotFoundException(error.ToString());
                        }

                        using (var keyStream = file.CreateReadStream())
                        {
                            manager.LoadKeyFromStream(keyStream);
                        }
                        break;
                    case KeyType.Password:
                        manager.LoadKeyFromPassword(source.Key);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(source.KeyType));
                }

                foreach (var key in manager.Keys)
                {
                    dictionary.Add(key, manager.Get(key));
                }
            }

            Data = dictionary;
        }
    }
}