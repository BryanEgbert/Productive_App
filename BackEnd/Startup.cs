// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Npgsql;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BackEnd
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }
        private string _clientId = null;
        private string _clientSecret = null;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Initialize certificate
            var cert = new X509Certificate2(Path.Combine(".", "IdsvCertificate.pfx"), "YouShallNotPass123");

            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            // The connection strings is in user secret
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            _clientId = Configuration["OAuth:ClientId"];
            _clientSecret = Configuration["OAuth:ClientSecret"];

            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddClaimsPrincipalFactory<ClaimsFactory>()
                .AddDefaultTokenProviders();


            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
                options.UserInteraction = new UserInteractionOptions() 
                { 
                    LoginUrl = "/Account/Login", 
                    LogoutUrl = "/Account/Logout" 
                };
            })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddProfileService<ProfileService>()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(options => 
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(connectionString, 
                        sql => sql.MigrationsAssembly(migrationAssembly));
                })
                .AddOperationalStore(options => 
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(connectionString, 
                        sql => sql.MigrationsAssembly(migrationAssembly));
                });

            // Add signed certificate to identity server
            builder.AddSigningCredential(cert);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddScoped<IProfileService, ProfileService>();
            // services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = _clientId;
                    options.ClientSecret = _clientSecret;
                    options.SaveTokens = true;
                    options.ClaimActions.MapJsonKey("role", "role");
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://localhost:5000";
                    options.Audience = "todoApi";
                });

                services.AddAuthorization();

                services.AddGrpc(options => 
                {
                    options.EnableDetailedErrors = true;
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app);

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<UserService>();
                endpoints.MapDefaultControllerRoute().RequireAuthorization();
            });
        }
        
        // Based on IdentityServer4 document
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}