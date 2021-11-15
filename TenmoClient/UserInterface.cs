using System;
using System.Collections.Generic;
using TenmoClient.APIClients;
using TenmoClient.Data;


namespace TenmoClient
{
    public class UserInterface
    {
        private readonly ConsoleService consoleService = new ConsoleService();
        private readonly AuthService authService = new AuthService();
        private readonly AccountService accountService = new AccountService();
        private readonly TransferService transferService = new TransferService();


        private bool quitRequested = false;


        public void Start()
        {
            while (!quitRequested)
            {
                while (!UserService.IsLoggedIn)
                {
                    ShowLogInMenu();
                }

                // If we got here, then the user is logged in. Go ahead and show the main menu
                ShowMainMenu();
            }
        }

        private void ShowLogInMenu()
        {
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.Write("Please choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int loginRegister))
            {
                Console.WriteLine("Invalid input. Please enter only a number.");
            }
            else if (loginRegister == 1)
            {
                HandleUserLogin();
            }
            else if (loginRegister == 2)
            {
                HandleUserRegister();
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        private void ShowMainMenu()
        {
            int menuSelection;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: Send TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else
                {
                    AccountUser account = accountService.LoggedInAccount();

                    switch (menuSelection)
                    {
                        case 1: // View Balance
                            Console.WriteLine("Your current balance is: " + account.Balance);
                            break;

                        case 2: // View Past Transfers
                            ReturnPastTransfers(account);
                            break;

                        case 3:
                            SendTEBucks();
                            break;

                        case 6: // Log in as someone else
                            Console.WriteLine();
                            UserService.ClearLoggedInUser(); //wipe out previous login info
                            return; // Leaves the menu and should return as someone else

                        case 0: // Quit
                            Console.WriteLine("Goodbye!");
                            quitRequested = true;
                            return;

                        default:
                            Console.WriteLine("That doesn't seem like a valid choice.");
                            break;
                    }
                }
            } while (menuSelection != 0);
        }

        private void SendTEBucks()
        {
            ListAllUsers();
            AccountUser accountFrom = accountService.LoggedInAccount();

            int userIdTo = CLIHelper.GetInteger("Enter ID of user you are sending to (0 to cancel):");
            AccountUser accountTo = accountService.GetAccountByUserID(userIdTo);
            decimal transferAmount = CLIHelper.GetInteger("Enter amount:");
            Transfer transfer = new Transfer(accountTo.AccountId, accountFrom.AccountId, transferAmount);
            if (transfer.AccountFrom != transfer.AccountTo)
            {
                transfer = transferService.MakeATransfer(transfer);
                AddToAccount(accountTo, transfer, accountFrom);
            }
            else
            {
                Console.WriteLine("User selection is Invalid");
            }
        }
        public void AddToAccount(AccountUser accountTo, Transfer transfer, AccountUser accountFrom)
        {
            if (transfer.Amount < accountFrom.Balance)
            {
                accountTo.Balance += transfer.Amount;
                accountFrom.Balance -= transfer.Amount;
                accountService.UpdateAccount(accountTo);
                accountService.UpdateAccount(accountFrom);
            }
            else
            {
                Console.WriteLine("You do not have enough money!");
            }

        }
        private void ListAllUsers()
        {
            List<API_User> users = accountService.GetAllUsers();
            if (users.Count <= 0)
            {
                Console.WriteLine("No Users Exist");
            }
            else
            {
                Console.WriteLine("Users");
                Console.WriteLine("ID     Name");
                foreach (API_User user in users)
                {
                    Console.WriteLine(user.UserId + "     " + user.Username);
                }
            }
        }

        private void HandleUserRegister()
        {
            bool isRegistered = false;

            while (!isRegistered) //will keep looping until user is registered
            {
                LoginUser registerUser = consoleService.PromptForLogin();
                isRegistered = authService.Register(registerUser);
            }

            Console.WriteLine("");
            Console.WriteLine("Registration successful. You can now log in.");
        }

        private void HandleUserLogin()
        {
            while (!UserService.IsLoggedIn) //will keep looping until user is logged in
            {
                LoginUser loginUser = consoleService.PromptForLogin();
                authService.Login(loginUser);
            }
        }
        private void ReturnPastTransfers(AccountUser account)
        {
            bool shouldContinue = true;
            while (shouldContinue)
            {

                List<Transfer> transferList = transferService.ReturnPastTransfers(account.AccountId);
                if (transferList.Count <= 0)
                {
                    Console.WriteLine("No transfers Exist");
                }
                else
                {
                    Console.WriteLine("Transfers");
                    Console.WriteLine("ID     From/To     Amount");
                    foreach (Transfer transfer in transferList)
                    {
                        Console.WriteLine();
                        Console.Write(transfer.TransferId + "     ");
                        if (transfer.AccountFrom == account.AccountId)
                        {
                            Console.Write("To: ");
                        }
                        else
                        {
                            Console.Write("From: ");
                        }
                        Console.Write(transfer.AccountToOrFromName + "     " + transfer.Amount);
                        Console.WriteLine();
                    }
                    int transferID = CLIHelper.GetInteger("Please Select Transfer ID you would like to see details of:");
                    DisplaySpecificTransfer(transferList, account, transferID);
                    string choice = CLIHelper.GetString("Would You Like to try again y/n: ");
                    if (choice.ToLower() == "n")
                    {
                        shouldContinue = false;
                    }
                }
            }
       }

        private void DisplaySpecificTransfer(List<Transfer> transferList,AccountUser account, int transferID)
        {
            foreach (Transfer transfersSelection in transferList)
            {

                if (transfersSelection.TransferId == transferID)
                {
                    Console.WriteLine("Transfer Details");
                    Console.WriteLine(transfersSelection.TransferId);
                    if (transfersSelection.AccountFrom == account.AccountId)
                    {
                        Console.WriteLine("To: " + transfersSelection.AccountToOrFromName);
                        Console.WriteLine("From: Me");
                    }
                    if (transfersSelection.AccountTo == account.AccountId)
                    {
                        Console.WriteLine("To: Me");
                        Console.WriteLine("From: " + transfersSelection.AccountToOrFromName);
                    }
                    Console.WriteLine(transfersSelection.TransferType);
                    Console.WriteLine(transfersSelection.TransferStatus);
                    Console.WriteLine(transfersSelection.Amount.ToString("C"));
                }

            }
        }
    }
}
