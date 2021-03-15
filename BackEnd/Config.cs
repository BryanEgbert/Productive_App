// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace BackEnd
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile(),
                        new IdentityResources.Email(),
                        new IdentityResource("role", "role", new List<string>() { "role" })
                   };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("scope1"),
                new ApiScope("scope2"),
            };
        
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("todo-api", "To-do API", new List<string>() { JwtClaimTypes.Role, JwtClaimTypes.Email })
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "499675830263-ldcg4fm7kcbjlt48tpaffqdbfnskmi8v.apps.googleusercontent.com",
                    ClientName = "blazor",
                    RequireClientSecret = false,
                    RequirePkce = true,
                    RequireConsent = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedCorsOrigins = { "https://localhost:5001" },

                    RedirectUris = { "https://localhost:5001/authentication/login-callback" },
                    PostLogoutRedirectUris = { "https://localhost:5001/" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "email", "role" }
                },
            };
    }
}