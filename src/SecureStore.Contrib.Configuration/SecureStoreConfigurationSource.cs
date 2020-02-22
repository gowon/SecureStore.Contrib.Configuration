namespace SecureStore.Contrib.Configuration
{
    using Microsoft.Extensions.Configuration;

    public class SecureStoreConfigurationSource : FileConfigurationSource
    {
        public string Key { get; set; }
        public KeyType KeyType { get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            FileProvider ??= builder.GetFileProvider();
            return new SecureStoreConfigurationProvider(this);
        }
    }
}