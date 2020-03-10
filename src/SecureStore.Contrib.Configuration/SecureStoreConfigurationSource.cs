namespace SecureStore.Contrib.Configuration
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Represents a SecureStore file as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class SecureStoreConfigurationSource : FileConfigurationSource
    {
        /// <summary>
        /// The key to decrypt the SecureStore file.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Determines if the key is a password or a key file.
        /// </summary>
        public KeyType KeyType { get; set; }

        /// <summary>
        /// Builds the <see cref="SecureStoreConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="SecureStoreConfigurationProvider"/></returns>
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new SecureStoreConfigurationProvider(this);
        }
    }
}