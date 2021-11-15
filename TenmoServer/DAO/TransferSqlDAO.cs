using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO: ITransferDAO
    {
        private readonly string connectionString;
        const string SQLInsert = "INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (@transfer_type_id, @transfer_status_id, @account_from, @account_to, @amount);  SELECT @@IDENTITY;";
        const string SQLSelect = "SELECT users.username AS Name,transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount FROM transfers INNER JOIN accounts ON transfers.account_from = @account_from OR transfers.account_to = @account_from INNER JOIN users ON accounts.user_id = users.user_id WHERE accounts.account_id != @account_from";
        const string SQLSelectByTransferId = "SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount FROM transfers WHERE transfer_id = @transfer_id;";

        public TransferSqlDAO(string dbconnectionString)
        {
            connectionString = dbconnectionString;
        }
        public Transfer MakeTransfer(Transfer transfer, int accountId)
        {

            transfer.AccountFrom = accountId;
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(SQLInsert, conn);
                command.Parameters.AddWithValue("transfer_type_id", transfer.TransferTypeId);
                command.Parameters.AddWithValue("@transfer_status_id", transfer.TransferStatusId);
                command.Parameters.AddWithValue("@account_from", accountId);
                command.Parameters.AddWithValue("@account_to", transfer.AccountTo);
                command.Parameters.AddWithValue("@amount", transfer.Amount);
                transfer.TransferId = Convert.ToInt32(command.ExecuteScalar());

            }
            return transfer;
        }
        public List<Transfer> ReturnAllTransfers(int accountId)
        {
            List<Transfer> transferList = new List<Transfer>();

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(SQLSelect, conn);
                command.Parameters.AddWithValue("@account_from", accountId);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Transfer transfer = new Transfer();
                        transfer.AccountToOrFromName = Convert.ToString(reader["Name"]);
                        transfer.AccountFrom = Convert.ToInt32(reader["account_from"]);
                        transfer.AccountTo = Convert.ToInt32(reader["account_to"]);
                        transfer.Amount = Convert.ToDecimal(reader["amount"]);
                        transfer.TransferId = Convert.ToInt32(reader["transfer_id"]);
                        transfer.TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]);

                        transferList.Add(transfer);
                    }
                }

            }
            return transferList;
        }
        public Transfer GetTransferById(int transferID)
        {


            Transfer transfer = new Transfer();
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(SQLSelectByTransferId, conn);
                command.Parameters.AddWithValue("@transfer_id", transferID);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        
                        transfer.AccountFrom = Convert.ToInt32(reader["account_from"]);
                        transfer.AccountTo = Convert.ToInt32(reader["account_to"]);
                        transfer.Amount = Convert.ToDecimal(reader["amount"]);
                        transfer.TransferId = Convert.ToInt32(reader["transfer_id"]);
                        transfer.TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]);
                        transfer.TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]);

                    }
                }
                return transfer;
            }
        }
    }
}
