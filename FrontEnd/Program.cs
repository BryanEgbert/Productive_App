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

            // Connect server to client
            builder.Services.AddScoped(services => 
            {
                var baseAddressMessageHandler = services.GetRequiredService<AuthorizationMessageHandler>()
                    .ConfigureHandler(
                        authorizedUrls: new[] { "https://localhost:5001" }, 
                        scopes: new[] { "todoApi" }
                    );
                baseAddressMessageHandler.InnerHandler = new HttpClientHandler();
                var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
                var channel = GrpcChannel.ForAddress("https://localhost:5000", new GrpcChannelOptions
                    { 
                        HttpHandler = httpHandler
                    });

                return new User.UserClient(channel);
            });
            
            // Add Open-ID Connect authentication
            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Authentication:Google", options.ProviderOptions);
                options.ProviderOptions.DefaultScopes.Add("role");
                options.UserOptions.RoleClaim = "role";  // Important to get role claim
            }).AddAccountClaimsPrincipalFactory<CustomUserFactory>();

            builder.Services.AddOptions();
            
            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();

        }
    }
}
