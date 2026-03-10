using Microsoft.Extensions.FileProviders;

namespace SecureStore.Contrib.Configuration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NeoSmart.SecureStore;
    using Xunit;

    public class SecureStoreConfigurationProviderTests : IDisposable
    {
        private static readonly string EmbeddedKeyName = "embedded.key";
        private static readonly string Password = "P@$$w0rD!";
        private readonly string _storePath;

        private static readonly Dictionary<string, string> SecureData = new Dictionary<string, string>
        {
            {"foo1", "bar1"},
            {"foo2", "bar2"},
            {"foo3", "bar3"}
        };

        public SecureStoreConfigurationProviderTests()
        {
            _storePath = Path.GetTempFileName();
        }

        public void Dispose()
        {
            File.Delete(_storePath);
        }

        [Fact]
        public void LoadStreamUsingKeyFile()
        {
            var keyPath = Path.GetTempFileName();
            CreateTestStore(_storePath, keyPath, KeyType.File);
            var configurationSource = new SecureStoreConfigurationSource
            {
                KeyType = KeyType.File,
                Key = keyPath,
                Optional = true
            };
            configurationSource.ResolveKeyFileProvider();
            var provider = new SecureStoreConfigurationProvider(configurationSource);

            using (var stream = new FileStream(_storePath, FileMode.Open, FileAccess.Read))
            {
                provider.Load(stream);
            }

            Assert.All(SecureData, item => Assert.Equal(provider.Get(item.Key), item.Value));
            File.Delete(keyPath);
        }

        [Fact]
        public void LoadStreamUsingEmbeddedKeyFile()
        {
            var assembly = typeof(SecureStoreConfigurationProviderTests).Assembly;
            var names = assembly.GetManifestResourceNames();
            using (var key = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{EmbeddedKeyName}")!)
            {
                CreateTestStore(_storePath, key);
            }
            var provider = new SecureStoreConfigurationProvider(new SecureStoreConfigurationSource
            {
                KeyFileProvider = new ManifestEmbeddedFileProvider(assembly),
                KeyType = KeyType.File,
                Key = EmbeddedKeyName,
                Optional = true
            });

            using (var stream = new FileStream(_storePath, FileMode.Open, FileAccess.Read))
            {
                provider.Load(stream);
            }

            Assert.All(SecureData, item => Assert.Equal(provider.Get(item.Key), item.Value));
        }

        [Fact]
        public void LoadStreamUsingPassword()
        {
            CreateTestStore(_storePath, Password, KeyType.Password);

            var provider = new SecureStoreConfigurationProvider(new SecureStoreConfigurationSource
            {
                KeyType = KeyType.Password,
                Key = Password,
                Optional = true
            });

            using (var stream = new FileStream(_storePath, FileMode.Open, FileAccess.Read))
            {
                provider.Load(stream);
            }

            Assert.All(SecureData, item => Assert.Equal(provider.Get(item.Key), item.Value));
        }

        [Fact]
        public void LoadStreamUsingPassword_ThrowsIfKeyTypeNotInRange()
        {
            CreateTestStore(_storePath, Password, KeyType.Password);

            var source = new SecureStoreConfigurationSource
            {
                KeyType = (KeyType)3,
                Key = Password,
                Optional = true
            };
            var provider = new SecureStoreConfigurationProvider(source);

            using (var stream = new FileStream(_storePath, FileMode.Open, FileAccess.Read))
            {
                var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
                    provider.Load(stream));
                Assert.Equal(nameof(source.KeyType), ex.ParamName);
            }
        }

        private void CreateTestStore(string storePath, string key, KeyType type)
        {
            using var sman = SecretsManager.CreateStore();
            if (type == KeyType.Password)
            {
                sman.LoadKeyFromPassword(key);
            }
            else
            {
                sman.GenerateKey();
            }

            foreach (var secretKey in SecureData.Keys)
            {
                sman.Set(secretKey, SecureData[secretKey]);
            }

            sman.SaveStore(storePath);
            sman.ExportKey(key);
        }

        private void CreateTestStore(string storePath, Stream key)
        {
            using var sman = SecretsManager.CreateStore();
            sman.LoadKeyFromStream(key);

            foreach (var secretKey in SecureData.Keys)
            {
                sman.Set(secretKey, SecureData[secretKey]);
            }

            sman.SaveStore(storePath);
        }
    }
}