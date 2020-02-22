# SecureStore.Contrib.Configuration

A [SecureStore](https://github.com/neosmart/SecureStore) configuration provider to use with .NET Core's `Microsoft.Extensions.Configuration`.

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/SecureStore.Contrib.Configuration?color=blue)](https://www.nuget.org/packages/SecureStore.Contrib.Configuration)
![build](https://github.com/gowon/SecureStore.Contrib.Configuration/workflows/build/badge.svg)

## Installing via NuGet

`Install-Package SecureStore.Contrib.Configuration`

or `dotnet add package SecureStore.Contrib.Configuration`

## Usage 

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
