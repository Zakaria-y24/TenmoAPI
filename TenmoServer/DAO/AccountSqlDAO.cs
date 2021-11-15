using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO: IAccountDAO
    {
        private readonly string connectionString;
        const string SQLSelect = "SELECT account_id AS accountid, accounts.user_id AS userid, balance FROM accounts INNER JOIN users ON accounts.user_id = users.user_id WHERE accounts.user_id = @user_id";
        const string SQLUpdate = "UPDATE accounts SET balance = @balance WHERE account_id = @account_id;";
        public AccountSqlDAO(string dbconnectionString)
        {
            connectionString = dbconnectionString;
        }
        public AccountUser GetAccount(int UserId)
        {   
            AccountUser returnAccount = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(SQLSelect, conn);
                cmd.Parameters.AddWithValue("@user_id", UserId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    returnAccount = GetAccountFromReader(reader);
                }
            }

            return returnAccount;
        }
        private AccountUser GetAccountFromReader(SqlDataReader reader)
        {
            return new AccountUser()
            {
                UserId = Convert.ToInt32(reader["userid"]),
                AccountId = Convert.ToInt32(reader["accountid"]),
                Balance = Convert.ToDecimal(reader["balance"])

            };
        }
        public AccountUser UpdateAccount(AccountUser incomingAccount)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(SQLUpdate, conn);
                cmd.Parameters.AddWithValue("@account_id", incomingAccount.AccountId);
                cmd.Parameters.AddWithValue("@balance", incomingAccount.Balance);
                cmd.ExecuteNonQuery();
                return incomingAccount;
            }
        }

    }
}
