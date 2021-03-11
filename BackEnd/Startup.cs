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

            var cert = new X509Certificate2(Path.Combine(".", "IdsvCertificate.pfx"), "YouShallNotPass123");

            _clientId = Configuration["OAuth:ClientId"];
            _clientSecret = Configuration["OAuth:ClientSecret"];

            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IProfileService, ProfileService>();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddProfileService<ProfileService>()
                .AddAspNetIdentity<ApplicationUser>();

            builder.AddSigningCredential(cert);
            // builder.AddDeveloperSigningCredential();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication()
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.Authority = "https://accounts.google.com";
                    options.RequireHttpsMetadata = true;
                    options.ResponseType = "code";
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("openid");

                    options.ClientId = _clientId;
                    options.ClientSecret = _clientSecret;
                    options.SaveTokens = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role",
                        ValidateIssuer = true
                    };
                });

                services.AddAuthorization();

                services.AddGrpc();
        }

        public void Configure(IApplicationBuilder app)
        {
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
    }
}