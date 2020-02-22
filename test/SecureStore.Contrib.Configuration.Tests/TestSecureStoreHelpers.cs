namespace SecureStore.Contrib.Configuration.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using NeoSmart.SecureStore;

    [ExcludeFromCodeCoverage]
    public static class TestSecureStoreHelpers
    {
        private static readonly Dictionary<string, string> SecureData = new Dictionary<string, string>
        {
            {"foo1", "bar1"},
            {"foo2", "bar2"},
            {"foo3", "bar3"}
        };

        public static void CreateTestStore(string storePath, string keyPath = null)
        {
            using (var store = SecretsManager.CreateStore())
            {
                store.GenerateKey();
                foreach (var key in SecureData.Keys)
                {
                    store.Set(key, SecureData[key]);
                }

                store.SaveStore(storePath);

                if (!string.IsNullOrEmpty(keyPath))
                {
                    store.ExportKey(keyPath);
                }
            }
        }
    }
}