using IVRPOC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IVRPOC
{
    public class SQLDatabaseService
    {
        internal static List<Accountant_Information> getAccountNumbers(CreateAccountOptions? accountType)
        {
            //  bool result = false;

            if (accountType == null)
            {
                return null;
            }
            try
            {
                var acctype = accountType.ToString();
                SqlConnection connection = null;
                string query = null;
                connection = new SqlConnection("Data Source=k8bjlaohq3.database.windows.net;Initial Catalog=ivrbot_db;Integrated Security=False;User ID=datareadserver;Password=Astrani@2016;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                connection.Open();
                MessagesController.accountnumlist = new List<Accountant_Information>();
                Accountant_Information accountantinf;
                string selectquery = "Select AccountNumber from [dbo].[Accountant_Information] where AccountType='"+ acctype+"'";
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = selectquery;
                    cmd.Connection = connection;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            accountantinf = new Accountant_Information();
                            //  accountantinf.AccountNumber = Convert.ToInt64(reader["AccountNumber"]);
                            accountantinf.AccountNumber = reader["AccountNumber"].ToString();
                            MessagesController.accountnumlist.Add(accountantinf);
                        }

                    }
                    //connection.Close();
                }

                return MessagesController.accountnumlist;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        internal static void InsertAccountantInformation(Customer completed, string accno, int accpin)
        {
            try
            {
                SqlConnection connection = null;
                string query = null;
                DateTime datetime = DateTime.Now;
                connection = new SqlConnection("Data Source=k8bjlaohq3.database.windows.net;Initial Catalog=ivrbot_db;Integrated Security=False;User ID=datareadserver;Password=Astrani@2016;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                connection.Open();
                if (completed.AccountType.ToString() == "SavingsAccount")
                {
                    query = "INSERT INTO [dbo].[Accountant_Information](AccountNumber,PinNo,FullName,AccountType,Personal_Information,Correspondence_Address,Permanent_Address,SSN,Nominee_Information,Saving_Balance,Timestamp)" +
                                    "Values ('" + accno + "','" + accpin + "','" + completed.FullName + "','" + completed.AccountType + "','" + completed.PersonalDetails + "','"
                                    + completed.CorrespondenceAddress + "','" + completed.PermanentAddress + "','" + completed.SocialSecurityNumber + "','" + completed.NomineeDetails + "','" + completed.SavingsAmount + "','" + datetime + "')";
                }
                else
                {
                    query = "INSERT INTO [dbo].[Accountant_Information](AccountNumber,PinNo,FullName,AccountType,Personal_Information,Correspondence_Address,Permanent_Address,SSN,Nominee_Information,Current_Balance,Timestamp)" +
                                    "Values ('" + accno + "','" + accpin + "','" + completed.FullName + "','" + completed.AccountType + "','" + completed.PersonalDetails + "','"
                                    + completed.CorrespondenceAddress + "','" + completed.PermanentAddress + "','" + completed.SocialSecurityNumber + "','" + completed.NomineeDetails + "','" + completed.SavingsAmount + "','" + datetime + "')";
                }


                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                    // connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        internal static bool DeleteAccountNumber(string accountNumber)
        {
            bool result = false;
            if (accountNumber == null)
            {
                return false;
            }
            try
            {
                SqlConnection connection = null;
                string query = null;
                connection = new SqlConnection("Data Source=k8bjlaohq3.database.windows.net;Initial Catalog=ivrbot_db;Integrated Security=False;User ID=datareadserver;Password=Astrani@2016;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                connection.Open();

                string deletequery = "Delete from [dbo].[Accountant_Information] where AccountNumber=" + accountNumber;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = deletequery;
                    cmd.Connection = connection;
                    cmd.ExecuteNonQuery();
                    result = true;
                    // connection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        internal static string checkingAccountBalance(string accno, string pIN)
        {
            if (accno == null&&pIN==null)
            {
                return null;
            }
            try
            {
                SqlConnection connection = null;
                string query = null;
                connection = new SqlConnection("Data Source=k8bjlaohq3.database.windows.net;Initial Catalog=ivrbot_db;Integrated Security=False;User ID=datareadserver;Password=Astrani@2016;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                connection.Open();
                MessagesController.accountnumlist = new List<Accountant_Information>();
                Accountant_Information accountantinf = new Accountant_Information();
                string selectquery = "Select Saving_Balance from [dbo].[Accountant_Information] where AccountNumber="+accno+"AND PinNo="+pIN;// where AccountType="+accountType;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = selectquery;
                    cmd.Connection = connection;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                                                      
                            accountantinf.Balance = reader["Saving_Balance"].ToString();
                            
                        }

                    }
                    //connection.Close();
                }

                return accountantinf.Balance;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        internal static string checkingCurrentAccountBalance(string accountNumber, string pIN)
        {
            if (accountNumber == null && pIN == null)
            {
                return null;
            }
            try
            {
                SqlConnection connection = null;
                string query = null;
                connection = new SqlConnection("Data Source=k8bjlaohq3.database.windows.net;Initial Catalog=ivrbot_db;Integrated Security=False;User ID=datareadserver;Password=Astrani@2016;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                connection.Open();
                MessagesController.accountnumlist = new List<Accountant_Information>();
                Accountant_Information accountantinf = new Accountant_Information();
                string selectquery = "Select Current_Balance from [dbo].[Accountant_Information] where AccountNumber=" + accountNumber + "AND PinNo=" + pIN;// where AccountType="+accountType;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = selectquery;
                    cmd.Connection = connection;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            accountantinf.Balance = reader["Current_Balance"].ToString();

                        }

                    }
                    //connection.Close();
                }

                return accountantinf.Balance;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

    }
}