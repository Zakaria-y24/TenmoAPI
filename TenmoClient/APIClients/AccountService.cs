using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient.APIClients
{
    public class AccountService
    {
        private const string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();


        private string apiToken = null;

        public AccountUser LoggedInAccount()
        {

            RestRequest request = new RestRequest(API_BASE_URL + $"account/user");
            apiToken = UserService.Token;
            client.Authenticator = new JwtAuthenticator(apiToken);

            IRestResponse<AccountUser> response = client.Get<AccountUser>(request);
            AccountUser account = response.Data;
            return account;
        }
        public AccountUser GetAccountByUserID(int userId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"account/{userId}");
            apiToken = UserService.Token;
            client.Authenticator = new JwtAuthenticator(apiToken);
            IRestResponse<AccountUser> response = client.Get<AccountUser>(request);
            AccountUser account = response.Data;
            if (!response.IsSuccessful)
            {
                Console.WriteLine("Could not get Account. Error status code " + (int)response.StatusCode);
                return new AccountUser();
            }
            return account;
        }
        public List<API_User> GetAllUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "account");
            apiToken = UserService.Token;
            client.Authenticator = new JwtAuthenticator(apiToken);
            IRestResponse<List<API_User>> response = client.Get<List<API_User>>(request);
            if (!response.IsSuccessful)
            {
                Console.WriteLine("Could not get Users. Error status code " + (int)response.StatusCode);
                return new List<API_User>();
            }
            return response.Data;
        }
        public AccountUser UpdateAccount(AccountUser account)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"account/balance/{account.AccountId}");

            request.AddJsonBody(account);

            apiToken = UserService.Token;
            client.Authenticator = new JwtAuthenticator(apiToken);
            IRestResponse<AccountUser> response = client.Put<AccountUser>(request);
            return response.Data;
        }
    }
}
