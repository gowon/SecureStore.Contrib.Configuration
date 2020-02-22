# SecureStore.Contrib.Configuration

A [SecureStore](https://github.com/neosmart/SecureStore) configuration provider to use with .NET Core's `Microsoft.Extensions.Configuration`.

### Installing 

Install using the [SecureStore.Contrib.Configuration package](https://www.nuget.org/packages/SecureStore.Contrib.Configuration):

`Install-Package SecureStore.Contrib.Configuration`

or `dotnet add package SecureStore.Contrib.Configuration`

### Usage 

When you install the package, it should be added to your _csproj_ file. Alternatively, you can add it directly by adding:

```xml
<PackageReference Include="SecureStore.Contrib.Configuration" Version="1.0.0" />
```

To load a SecureStore file as part of your config, just load it as part of your normal `ConfigurationBuilder` setup in the `Program` class of your .NET Core app. 

The simplest possible usage that loads a single SecureStore file called `secrets.json` with key file `secrets.key` would be:

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => 
            {
                builder.AddSecureStoreFile("secrets.json", "secrets.key", KeyType.File, optional: false);
            })
            .UseStartup<Startup>()
            .Build();
}
```

A more complete `Program` class that loads multiple files (overwriting config values) might look more like the following: 

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
            {
                var env = context.HostingEnvironment;
                builder
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddSecureStoreFile("secrets.json", "secrets.key", KeyType.File, optional: false);
            })
            .UseStartup<Startup>()
            .Build();
}
```

## License

MIT