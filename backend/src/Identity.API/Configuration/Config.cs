using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.Extensions.Configuration;

namespace Identity.API.Configuration
{
    public static class Config
    {
        // =========================
        // API RESOURCES
        // =========================
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("orders", "Orders Service"),
                new ApiResource("basket", "Basket Service"),
                new ApiResource("webhooks", "Webhooks Service")
            };
        }

        // =========================
        // API SCOPES
        // =========================
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("orders", "Orders Service"),
                new ApiScope("basket", "Basket Service"),
                new ApiScope("webhooks", "Webhooks Service")
            };
        }

        // =========================
        // IDENTITY RESOURCES
        // =========================
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        // =========================
        // CLIENTS
        // =========================
        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            // 🔒 CHỐNG NULL – FIX CS8604
            var mauiCallback = configuration["MauiCallback"] ?? "http://localhost";
            var webAppClient = configuration["WebAppClient"] ?? "http://localhost";
            var webhooksClient = configuration["WebhooksWebClient"] ?? "http://localhost";
            var basketApiClient = configuration["BasketApiClient"] ?? "http://localhost";
            var orderingApiClient = configuration["OrderingApiClient"] ?? "http://localhost";
            var webhooksApiClient = configuration["WebhooksApiClient"] ?? "http://localhost";

            return new List<Client>
            {
                // =========================
                // MAUI CLIENT
                // =========================
                new Client
                {
                    ClientId = "maui",
                    ClientName = "MAUI Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireConsent = false,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { mauiCallback },
                    PostLogoutRedirectUris = { $"{mauiCallback}/Account/Redirecting" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "orders",
                        "basket",
                        "webhooks"
                    },

                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 7200,
                    IdentityTokenLifetime = 7200
                },

                // =========================
                // WEB APP CLIENT
                // =========================
                new Client
                {
                    ClientId = "webapp",
                    ClientName = "Web App Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = false,
                    RequireConsent = false,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { $"{webAppClient}/signin-oidc" },
                    PostLogoutRedirectUris = { $"{webAppClient}/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "orders",
                        "basket"
                    },

                    AllowOfflineAccess = true
                },

                // =========================
                // SWAGGER UI – BASKET API
                // =========================
                new Client
                {
                    ClientId = "basketswaggerui",
                    ClientName = "Basket Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{basketApiClient}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{basketApiClient}/swagger/" },

                    AllowedScopes = { "basket" }
                },

                // =========================
                // SWAGGER UI – ORDERING API
                // =========================
                new Client
                {
                    ClientId = "orderingswaggerui",
                    ClientName = "Ordering Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{orderingApiClient}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{orderingApiClient}/swagger/" },

                    AllowedScopes = { "orders" }
                },

                // =========================
                // SWAGGER UI – WEBHOOKS API
                // =========================
                new Client
                {
                    ClientId = "webhooksswaggerui",
                    ClientName = "Webhooks Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{webhooksApiClient}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{webhooksApiClient}/swagger/" },

                    AllowedScopes = { "webhooks" }
                }
            };
        }
    }
}
