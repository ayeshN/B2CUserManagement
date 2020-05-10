using B2CUserManagement.Interfaces;
using B2CUserManagement.Models;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2CUserManagement
{
    public class UserManager : IUserManager
    {
        private readonly GraphServiceClient graphClient;
        private readonly B2CUserSettings userSettings;


        //public UserManagement() { }
        public UserManager(IOptions<B2CUserSettings> userSettings)
        {
            // The client_id, client_secret, and tenant are pulled in from the appsettings.json from coach API
            this.userSettings = userSettings.Value;

            // Initialize the client credential auth provider
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(this.userSettings.clientId)
                .WithTenantId(this.userSettings.tenant)
                .WithClientSecret(this.userSettings.clientSecret)
                .Build();

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

            // Set up the Microsoft Graph service client with client credentials
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);
            this.graphClient = graphClient;
        }

        public async Task CreateUser(B2CUser user)
        {
            try
            {
                // Create user
                var result = await this.graphClient.Users
                .Request()
                .AddAsync(new User
                {
                    GivenName = user.FirstName,
                    Surname = user.LastName,
                    DisplayName = user.FirstName + " " + user.LastName,
                    Identities = new List<ObjectIdentity>
                    {
                        new ObjectIdentity()
                        {
                            SignInType = "emailAddress",
                            Issuer = this.userSettings.tenant,
                            IssuerAssignedId = user.Email
                        }
                    },
                    PasswordProfile = new PasswordProfile()
                    {
                        Password = Helpers.PasswordHelper.GenerateNewPassword(4, 8, 4)
                    },
                    PasswordPolicies = "DisablePasswordExpiration",
                });
            }
            catch (ServiceException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {

                }
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<IGraphServiceUsersCollectionPage> GetUserByEmail(string email)
        {
            try
            {
                // Get user by sign-in name
                var result = await this.graphClient.Users
                    .Request()
                    .Filter($"identities/any(c:c/issuerAssignedId eq '{email}' and c/issuer eq '{this.userSettings.tenant}')")
                    .Select(e => new
                    {
                        e.DisplayName,
                        e.Id,
                        e.Identities
                    })
                    .GetAsync();

                if (result != null)
                {
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async void DeleteUser(string email)
        {
            var user = await this.GetUserByEmail(email);
            var userId = user.CurrentPage[0].Id;

            try
            {
                // Delete user by object ID
                await this.graphClient.Users[userId]
                   .Request()
                   .DeleteAsync();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
