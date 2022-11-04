
using System;
using System.Threading.Tasks;

#region KeyVault
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
#endregion

#region AppConfiguration
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Azure.Security.KeyVault.Secrets;
using Azure.Security.KeyVault;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Azure;
#endregion


namespace KeyVault
{
    class Program
    {
        static string tenentId = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";
        static string clientId = "d3ec77b5-8b43-4bdf-a13d-a692253f6fd2";
        static string clientSecret = "ewS8Q~.m0kH6TwX2HjaFI7Keq~rLAoQJHVQ69cQR";
        static string kvUri = "https://ps-sleutelbos.vault.azure.net/";
        
        static async Task Main(string[] args)
        {
           //await ReadKeyVault();
           await ReadAppConfigurationAsync();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
        private static async Task ReadKeyVault()
        {
            ClientSecretCredential cred = new ClientSecretCredential(tenentId, clientId, clientSecret);
            SecretClient kvClient = new SecretClient(new Uri(kvUri), cred);
                
            var result = await kvClient.GetSecretAsync("MyKey");
            Console.WriteLine($"Hello {result.Value?.Value}");
        }

        private static async Task ReadAppConfigurationAsync()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json")
                   .AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();

                      

            //ReadLocal();
            await ReadRemoteAsync();

            void ReadLocal()
            {
                Console.WriteLine(configuration["MySetings:hello"]);
                Console.WriteLine(configuration["ConnectionString"]);
            }

            async Task ReadRemoteAsync()
            {
                //ClientSecretCredential cred = new ClientSecretCredential(tenentId, clientId, clientSecret);
                //var env = Environment.GetEnvironmentVariable("Bla");
               // builder.AddAzureKeyVault(new Uri(kvUri), cred);
                builder.AddAzureAppConfiguration(opts => {
                    opts.ConfigureKeyVault(kvopts =>
                    {
                        kvopts.SetCredential(new ClientSecretCredential(tenentId, clientId, clientSecret));
                    });
                    opts.Connect("Endpoint=https://ps-konf.azconfig.io;Id=QYOE-l9-s0:vkoWX9B8jg5mYqdH+TA9;Secret=8Cark/BY7lm7MgYHpCGJSUxjjiPAAToHQNn9ahO2/9I=")                  
                        .Select(KeyFilter.Any, "Development")
                       // .Select(KeyFilter.Any, "Prog") // When using labels in your configuration, import the appropriate keys for that label
                       .UseFeatureFlags();
                        
                    });
                //builder.AddAzureAppConfiguration(opts => {
                //    opts.ConfigureKeyVault(kvopts =>
                //    {
                //        kvopts.SetCredential(new ClientSecretCredential(tenentId, clientId, clientSecret));
                //    })
                //    .UseFeatureFlags();
                //    opts.Connect(configuration["ConnectionString"]);    
                   
               // });
                IConfiguration conf = builder.Build();

                Console.WriteLine($"{conf["KeyVault:MijnVal"]}");
                Console.WriteLine($"Hello {conf["ThaKey"]}");

                IServiceCollection services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(conf).AddFeatureManagement();

                using (var svcProvider = services.BuildServiceProvider())
                {
                    do
                    {
                        using (var scope = svcProvider.CreateScope())
                        {
                            var featureManager = scope.ServiceProvider.GetRequiredService<IFeatureManager>();

                            if (await featureManager.IsEnabledAsync("mijnfeature"))
                            {
                                Console.WriteLine("We have a new feature");
                            }
                            await Task.Delay(10000);
                        }
                    }
                    while (true);
                }
            }
        }
    }
}
