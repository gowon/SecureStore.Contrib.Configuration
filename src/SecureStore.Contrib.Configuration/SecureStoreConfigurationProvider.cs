namespace SecureStore.Contrib.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using NeoSmart.SecureStore;

    public class SecureStoreConfigurationProvider : FileConfigurationProvider
    {
        public SecureStoreConfigurationProvider(SecureStoreConfigurationSource source) : base(source)
        {
        }

        public override void Load(Stream stream)
        {
            var source = (SecureStoreConfigurationSource)Source;
            var dictionary = new Dictionary<string, string>();
            using (var manager = SecretsManager.LoadStore(stream))
            {
                switch (source.KeyType)
                {
                    case KeyType.File:
                        manager.LoadKeyFromFile(source.Key);
                        break;
                    case KeyType.Password:
                        manager.LoadKeyFromPassword(source.Key);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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