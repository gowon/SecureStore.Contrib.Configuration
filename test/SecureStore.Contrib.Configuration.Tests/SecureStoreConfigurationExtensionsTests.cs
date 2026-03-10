namespace SecureStore.Contrib.Configuration.Tests
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Xunit;

    public class SecureStoreConfigurationExtensionsTests
    {
        [Fact]
        public void AddSecureStoreFile_ThrowsIfBuilderIsNull()
        {
            // Arrange
            var path = "does-not-exist.json";
            var key = "password";

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                SecureStoreConfigurationExtensions.AddSecureStoreFile(null, path, key, KeyType.Password).Build());
            Assert.StartsWith("builder", ex.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void AddSecureStoreFile_ThrowsIfFilePathIsNullOrEmpty(string path)
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();
            var key = "password";

            // Act and Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                configurationBuilder.AddSecureStoreFile(path, key, KeyType.Password));
            Assert.Equal("path", ex.ParamName);
            Assert.StartsWith("File path must be a non-empty string.", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void AddSecureStoreFile_ThrowsIfKeyIsNullOrEmpty(string key)
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();
            var path = "does-not-exist.json";

            // Act and Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                configurationBuilder.AddSecureStoreFile(path, key, KeyType.Password));
            Assert.Equal("key", ex.ParamName);
            Assert.StartsWith("File key/path must be a non-empty string.", ex.Message);
        }

        [Fact]
        public void AddSecureStoreFile_ThrowsIfFileDoesNotExistAtPath()
        {
            // Arrange
            var path = "does-not-exist.json";
            var key = "password";

            // Act and Assert
            var ex = Assert.Throws<FileNotFoundException>(() =>
                new ConfigurationBuilder().AddSecureStoreFile(path, key, KeyType.Password).Build());
            Assert.StartsWith($"The configuration file '{path}' was not found and is not optional.", ex.Message);
        }

        [Fact]
        public void AddSecureStoreFile_ThrowsIfFileDoesNotExistAtKey()
        {
            // Arrange
            var path = Path.GetTempFileName();
            TestSecureStoreHelpers.CreateTestStore(path);
            var keyPath = "does-not-exist.json";

            // Act and Assert
            var ex = Assert.Throws<FileNotFoundException>(() =>
                new ConfigurationBuilder().AddSecureStoreFile(path, keyPath, KeyType.File).Build());
            Assert.StartsWith($"The configuration key file '{keyPath}' was not found", ex.Message);

            // Cleanup
            File.Delete(path);
        }
    }
}