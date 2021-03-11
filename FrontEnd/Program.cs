using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Global.Protos;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FrontEnd
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient()
                { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped(services => 
            {
                var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
                var channel = GrpcChannel.ForAddress("https://localhost:5000", new GrpcChannelOptions
                    { 
                        HttpHandler = httpHandler
                    });

                return new Greeter.GreeterClient(channel);
            });

            builder.Services.AddScoped(services => 
            {
                var baseAddressMessageHandler = services.GetRequiredService<BaseAddressAuthorizationMessageHandler>();
                baseAddressMessageHandler.InnerHandler = new HttpClientHandler();
                var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
                var channel = GrpcChannel.ForAddress("https://localhost:5000", new GrpcChannelOptions
                    { 
                        HttpHandler = httpHandler
                    });

                return new User.UserClient(channel);
            });
            
            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Authentication:Google", options.ProviderOptions);
                options.UserOptions.RoleClaim = "SignedInUser";
            }).AddAccountClaimsPrincipalFactory<CustomUserFactory>();

            builder.Services.AddOptions();
            
            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();

        }
    }
}
