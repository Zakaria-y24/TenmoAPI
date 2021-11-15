using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly IUserDAO userDAO;
        private readonly ITransferDAO transferDAO;
        public AccountController(IAccountDAO _accountDAO, IUserDAO _userDAO, ITransferDAO _transferDAO)
        {
            accountDAO = _accountDAO;
            userDAO = _userDAO;
            transferDAO = _transferDAO;

        }
        [HttpGet("user")]
        [Authorize]
        public IActionResult GetAccount()
        {
            int userId = int.Parse(this.User.FindFirst("sub").Value);
            AccountUser account = accountDAO.GetAccount(userId);
            return Ok(account);
        }
        [HttpGet("{userId}")]
        [Authorize]
        public IActionResult GetAccountById(int userId)
        {
            AccountUser account = accountDAO.GetAccount(userId);
            return Ok(account);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAllUsers()
        {
           List<User> users = userDAO.GetUsers();
           return Ok(users);
        }
        [HttpPost("transfer")]
        [Authorize]
        public IActionResult MakeATransfer(Transfer incomingTransfer)
        {
            int userId = int.Parse(this.User.FindFirst("sub").Value);
            AccountUser account = accountDAO.GetAccount(userId);

            if (incomingTransfer.AccountFrom == account.AccountId && incomingTransfer.AccountTo != account.AccountId && account.Balance > 0)
            {
                Transfer transfer = transferDAO.MakeTransfer(incomingTransfer,account.AccountId);
                return Created($"transfer/{transfer.TransferId}", transfer);
            }
            else
            {
                return Forbid();
            }
        }
        [HttpPut("balance/{account.AccountId}")]
        [Authorize]
        public IActionResult UpdateBalance(AccountUser incomingAccount)
        {
            int userId = int.Parse(this.User.FindFirst("sub").Value);
            AccountUser account = accountDAO.UpdateAccount(incomingAccount);
            if (incomingAccount.UserId == userId)
            {
                return Ok(account.Balance);
            }
            else
            {
                return Forbid();
            }
        }
        [HttpGet("transfers/{accountId}")]
        [Authorize]
        public IActionResult GetAllTransactions(int accountId)
        {
            List<Transfer> transferList = transferDAO.ReturnAllTransfers(accountId);
            return Ok(transferList);
        }
        [HttpGet("{transferId}/transfers")]
        [Authorize]
        public IActionResult GetTransferById(int transferId)
        {
            Transfer transfer = transferDAO.GetTransferById(transferId);
            return Ok(transfer);
        }

    }
}
