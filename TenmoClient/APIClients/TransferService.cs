using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient.APIClients
{
    public class TransferService
    {
        private const string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();


        private string apiToken = null;
        public Transfer MakeATransfer(Transfer transfer)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "account/transfer");
            request.AddJsonBody(transfer);
            apiToken = UserService.Token;
            client.Authenticator = new JwtAuthenticator(apiToken);
            IRestResponse<Transfer> response = client.Post<Transfer>(request);
            if (!response.IsSuccessful)
            {
                Console.WriteLine("Could not make Transfer. Error status code " + (int)response.StatusCode);
                return null;
            }
            return response.Data;

        }
        public List<Transfer> ReturnPastTransfers(int accountId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"account/transfers/{accountId}");
            apiToken = UserService.Token;
            client.Authenticator = new JwtAuthenticator(apiToken);
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            if (!response.IsSuccessful)
            {
                Console.WriteLine("Could not get Transfers. Error status code " + (int)response.StatusCode);
                return new List<Transfer>();
            }

            return response.Data;
        }
        public Transfer GetTransferByID(int transferId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"{transferId}/transfers");
            apiToken = UserService.Token;
            client.Authenticator = new JwtAuthenticator(apiToken);
            IRestResponse<Transfer> response = client.Get<Transfer>(request);
            Transfer transfer = response.Data;
            if (!response.IsSuccessful)
            {
                Console.WriteLine("Could not get Transfer. Error status code " + (int)response.StatusCode);
                return new Transfer();
            }

            return transfer;
        }
    }
}
