// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Idp
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResources.Address(),
                new IdentityResources.Email(),
                new IdentityResource("roles","角色",new List<string>{ JwtClaimTypes.Role}),
                new IdentityResource("locations","地点",new List<string>{"location"}),
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api1", "My API #1",new List<string> { "location" })
                {
                    ApiSecrets={new Secret("api1 secret".Sha256()) }
                }
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "console client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1"}
                },

                // wpf client passward grand
                new Client
                {
                    ClientId = "wpf client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { 
                        new Secret("wpf client".Sha256())
                    },

                    AllowedScopes = { "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone
                    }
                },

                //dotnet core mvc client,authrization code
                new Client
                {
                    ClientId = "mvc client",
                    ClientName = "Asp.net Core Mvc Client",

                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    ClientSecrets = {
                        new Secret("mvc secret".Sha256())
                    },
                    RedirectUris={ "http://localhost:5003/signin-oidc"},
                    FrontChannelLogoutUri="http://localhost:5003/signout-oidc",
                    PostLogoutRedirectUris={ "http://localhost:5003/signout-callback-oidc"},
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AccessTokenLifetime=60,
                    AllowedScopes = { "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone
                    }
                },

                 // react, implicit flow
                new Client
                {
                    ClientId = "react-client",
                    ClientName =  "React SPA 客户端",
                    ClientUri = "http://localhost:8084",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = true,
                    AccessTokenLifetime = 60 * 5,

                    RedirectUris =
                    {
                        "http://localhost:8084/#/callback",
                        "http://localhost:8084/silentRenew.html"
                    },

                    PostLogoutRedirectUris =
                    {
                        "http://localhost:8084"
                    },

                    AllowedCorsOrigins =
                    {
                        "http://localhost:8084"
                    },

                    AllowedScopes = {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        //IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },

               
                // mvc, hybrid flow
                new Client
                {
                    ClientId = "hybrid client",
                    ClientName =  "Asp.net Core Hybrid 客户端",
                    ClientUri = "http://localhost:8084",

                    ClientSecrets={ new Secret("hybrid secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,


                    RedirectUris =
                    {
                        "http://localhost:7000/signin-oidc"
                    },

                    PostLogoutRedirectUris =
                    {
                        "http://localhost:7000/signout-callback-oidc"
                    },

                   AllowOfflineAccess=true,
                   AccessTokenType=AccessTokenType.Reference,
                   AlwaysIncludeUserClaimsInIdToken=true,
                    AllowedScopes = {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "locations"
                    }
                },
            };
    }
}