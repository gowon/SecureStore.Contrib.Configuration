namespace SecureStore.Contrib.Configuration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NeoSmart.SecureStore;
    using Xunit;

    public class SecureStoreConfigurationProviderTests
    {
        private static string Password => "P@$$w0rD!";

        private static readonly Dictionary<string, string> SecureData = new Dictionary<string, string>
        {
            {"foo1", "bar1"},
            {"foo2", "bar2"},
            {"foo3", "bar3"}
        };

        private void CreateTestStore(string storePath, string key, KeyType type)
        {
            using (var sman = SecretsManager.CreateStore())
            {
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
        }

        [Fact]
        public void LoadStreamUsingKeyFile()
        {
            var storePath = Path.GetTempFileName();
            var keyPath = Path.GetTempFileName();

            CreateTestStore(storePath, keyPath, KeyType.File);

            var provider = new SecureStoreConfigurationProvider(new SecureStoreConfigurationSource
            {
                KeyType = KeyType.File,
                Key = keyPath,
                Optional = true
            });

            using (var stream = new FileStream(storePath, FileMode.Open, FileAccess.Read))
            {
                provider.Load(stream);
            }

            File.Delete(storePath);
            File.Delete(keyPath);
        }

        [Fact]
        public void LoadStreamUsingPassword()
        {
            var storePath = Path.GetTempFileName();
            CreateTestStore(storePath, Password, KeyType.Password);

            var provider = new SecureStoreConfigurationProvider(new SecureStoreConfigurationSource
            {
                KeyType = KeyType.Password,
                Key = Password,
                Optional = true
            });

            using (var stream = new FileStream(storePath, FileMode.Open, FileAccess.Read))
            {
                provider.Load(stream);
            }

            File.Delete(storePath);
        }

        [Fact]
        public void LoadStreamUsingPassword_ThrowsIfKeyTypeNotInRange()
        {
            var storePath = Path.GetTempFileName();
            CreateTestStore(storePath, Password, KeyType.Password);

            var source = new SecureStoreConfigurationSource
            {
                KeyType = (KeyType) 3,
                Key = Password,
                Optional = true
            };
            var provider = new SecureStoreConfigurationProvider(source);

            using (var stream = new FileStream(storePath, FileMode.Open, FileAccess.Read))
            {
                var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
                    provider.Load(stream));
                Assert.Equal(nameof(source.KeyType), ex.ParamName);
            }

            File.Delete(storePath);
        }
    }
}