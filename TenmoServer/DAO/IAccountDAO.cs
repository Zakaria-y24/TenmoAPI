using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDAO
    {
        AccountUser GetAccount(int UserId);
        AccountUser UpdateAccount(AccountUser incomingAccount);
    }
}
