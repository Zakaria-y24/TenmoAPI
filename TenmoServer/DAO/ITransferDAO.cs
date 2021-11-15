using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        Transfer MakeTransfer(Transfer transfer, int accountId);
        public Transfer GetTransferById(int transferID);
        List<Transfer> ReturnAllTransfers(int accountId);

    }
}
