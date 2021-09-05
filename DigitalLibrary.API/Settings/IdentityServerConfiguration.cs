using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace DigitalLibrary.API.Settings
{
    public static class IdentityServerConfiguration
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName),
                new ApiScope(DigitalLibraryConstants.Policies.Administration, new []
                {
                    DigitalLibraryConstants.Claims.Role
                }),
                new ApiScope(DigitalLibraryConstants.Policies.Moderation, new []
                {
                    DigitalLibraryConstants.Claims.Role
                })
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "angular.client",
                    ClientName = "Angular Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    RequirePkce = true,
                    RequireConsent = false,

                    RedirectUris = new[] {"https://localhost:5001"},
                    PostLogoutRedirectUris = new[] {"https://localhost:5001"},
                    AllowedCorsOrigins = new[] {"https://localhost:5001"},
                    AllowOfflineAccess = true,


                    AllowedScopes = new[]
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.LocalApi.ScopeName,
                        IdentityServerConstants.StandardScopes.Email,
                        DigitalLibraryConstants.Policies.Administration,
                        DigitalLibraryConstants.Policies.Moderation
                    },
                },
                new Client
                {
                    ClientId = "angular.client.prod",
                    ClientName = "Angular Client Prod",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    RequirePkce = true,
                    RequireConsent = false,

                    RedirectUris = new[] {"https://localhost:5001"},
                    PostLogoutRedirectUris = new[] {"https://localhost:5001"},
                    AllowedCorsOrigins = new[] {"https://localhost:5001"},
                    AllowOfflineAccess = true,


                    AllowedScopes = new[]
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.LocalApi.ScopeName,
                        IdentityServerConstants.StandardScopes.Email,
                        DigitalLibraryConstants.Policies.Administration,
                        DigitalLibraryConstants.Policies.Moderation
                    },
                },
                new Client
                {
                    ClientId = "angular.client_2",
                    ClientName = "Angular Client 2",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    RequirePkce = true,
                    RequireConsent = false,

                    RedirectUris = new[] {"https://localhost:5003", "http://localhost:5002"},
                    PostLogoutRedirectUris = new[] {"https://localhost:5003", "http://localhost:5002"},
                    AllowedCorsOrigins = new[] {"https://localhost:5003", "http://localhost:5002"},
                    AllowOfflineAccess = true,


                    AllowedScopes = new[]
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.LocalApi.ScopeName,
                        IdentityServerConstants.StandardScopes.Email,
                        DigitalLibraryConstants.Policies.Administration,
                        DigitalLibraryConstants.Policies.Moderation
                    },
                },
                new Client
                {
                    ClientId = "angular.client.prod_2",
                    ClientName = "Angular Client Prod 2",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    RequirePkce = true,
                    RequireConsent = false,

                    RedirectUris = new[] {"https://localhost:5003, http://localhost:5002"},
                    PostLogoutRedirectUris = new[] {"https://localhost:5003, http://localhost:5002"},
                    AllowedCorsOrigins = new[] {"https://localhost:5003, http://localhost:5002"},
                    AllowOfflineAccess = true,


                    AllowedScopes = new[]
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.LocalApi.ScopeName,
                        IdentityServerConstants.StandardScopes.Email,
                        DigitalLibraryConstants.Policies.Administration,
                        DigitalLibraryConstants.Policies.Moderation
                    },
                }
            };
    }
}
