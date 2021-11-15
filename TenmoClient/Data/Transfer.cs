using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class Transfer
    {
        public Transfer()
        {

        }
        public Transfer(int accountIdTo, int accountIdFrom, decimal amount)
        {
            this.Amount = amount;
            this.AccountTo = accountIdTo;
            this.AccountFrom = accountIdFrom;
           
        }
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; } = 1001;
        public int TransferStatusId { get; set; } = 2001;
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public string AccountToOrFromName { get; set; }
        public decimal Amount { get; set; }
        public string TransferStatus
        {
            get
            {
                if (TransferStatusId == 2001)
                {
                    return "Approved";
                }
                else if (TransferStatusId == 2002)
                {
                    return "Rejected";
                }
                else
                {
                    return "Pending";
                }
            }
        }
        public string TransferType
        {
            get
            {
                if (TransferTypeId == 1000)
                {
                    return "Request";
                }
                else
                {
                    return "Send";
                }
            }
        }
    }
}
