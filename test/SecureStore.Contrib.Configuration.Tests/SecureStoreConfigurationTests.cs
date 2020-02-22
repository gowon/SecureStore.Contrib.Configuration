namespace SecureStore.Contrib.Configuration.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using NeoSmart.SecureStore;
    using Xunit;

    public class SecureStoreConfigurationTest
    {
        private static readonly Dictionary<string, string> SecureData = new Dictionary<string, string>
        {
            {"foo1", "bar1"},
            {"foo2", "bar2"},
            {"foo3", "bar3"}
        };

        public void CreateStore()
        {
            var storePath = Path.GetTempFileName();

            using (var sman = SecretsManager.CreateStore())
            {
                sman.GenerateKey();
                sman.SaveStore(storePath);
            }

            Assert.True(File.Exists(storePath));
            Assert.NotEqual(0, new FileInfo(storePath).Length);
        }

        public void ExportNewKey()
        {
            var keyPath = Path.GetTempFileName();

            using (var sman = SecretsManager.CreateStore())
            {
                sman.GenerateKey();
                sman.ExportKey(keyPath);
            }

            Assert.True(File.Exists(keyPath));
            Assert.NotEqual(0, new FileInfo(keyPath).Length);
        }

        private void CreateTestStore(string storePath, string keyPath)
        {
            using (var sman = SecretsManager.CreateStore())
            {
                sman.GenerateKey();
                foreach (var key in SecureData.Keys)
                {
                    sman.Set(key, SecureData[key]);
                }

                sman.SaveStore(storePath);
                sman.ExportKey(keyPath);
            }
        }

        public void StoreAndLoad()
        {
            var storePath = Path.GetTempFileName();
            var keyPath = Path.GetTempFileName();

            CreateTestStore(storePath, keyPath);

            using (var sman = SecretsManager.LoadStore(storePath))
            {
                sman.LoadKeyFromFile(keyPath);
                foreach (var key in SecureData.Keys)
                {
                    Assert.Equal(SecureData[key], sman.Get(key));
                }
            }
        }

        public void StoreAndLoadStream()
        {
            var storePath = Path.GetTempFileName();
            var keyPath = Path.GetTempFileName();

            CreateTestStore(storePath, keyPath);

            using (var stream = new FileStream(storePath, FileMode.Open, FileAccess.Read))
            {
                using (var sman = SecretsManager.LoadStore(stream))
                {
                    sman.LoadKeyFromFile(keyPath);
                    foreach (var key in SecureData.Keys)
                    {
                        Assert.Equal(SecureData[key], sman.Get(key));
                    }
                }
            }
        }

        private SecureStoreConfigurationProvider LoadProvider(string Yaml)
        {
            var p = new SecureStoreConfigurationProvider(new SecureStoreConfigurationSource() { Optional = true });
            p.Load(TestStreamHelpers.StringToStream(Yaml));
            return p;
        }
    }
}