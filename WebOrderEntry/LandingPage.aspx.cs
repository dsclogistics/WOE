using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Diagnostics;


namespace WebOrderEntry
{
    public partial class LandingPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String sUserAcct = Request.Params["username"];

            if (Request.Params["username"] != null)
            {
                Session["AccountId"] = sUserAcct;
                lblUserId.Text = Session["AccountId"].ToString();
            }
            if (Request.Params["LOGOFF_URL"] != null)
            {
                Session["LogoffUrl"] = Request.Params["LOGOFF_URL"];
                // Change Made by FD on 04/30/2012 To disable the Logoff URL and let the menu close the page instead
                //Menu masterPageMenu = (Menu)this.Master.FindControl("Menu1");
                //MenuItem logoffMenuItem = masterPageMenu.Items[2];
                //logoffMenuItem.NavigateUrl = (string)Session["LogoffUrl"];
            }

            //if (!dbGetAccountNumber((string)Session["AccountId"]))
            //{

            //}

            //Determine if the Logged on Customer needs to be redirected to the "Enter Order" page or if it will be given the 
            //options in the Landing Page
            
            switch (sUserAcct)
            {
                case "DRCOMFORT":
                case "PLANETINC":
                case "SKOLNIKINC":
                    // Do Nothing. We want the user to stay in this page to be able to select
                    //whether to enter orders manually or to Upload a file
                    break;
                default:
                 // Redirect user to the "Enter Order" Screen
                    Response.Redirect("EnterOrder.aspx");
                    break;
            }

        }

        //private bool dbGetAccountNumber(string sAccountId)
        //{
        //    bool isValid = true;

        //    string connString =
        //    ConfigurationManager.ConnectionStrings["WebOrderEntryConnectionString"].ConnectionString;
        //    try
        //    {
        //        using (SqlConnection myConnection = new SqlConnection(connString.ToString()))
        //        {
        //            myConnection.Open();

        //            SqlDataReader dr = null;

        //            SqlCommand myCommand = new SqlCommand();
        //            myCommand.Connection = myConnection;
        //            myCommand.CommandText = "sp_GetAccountNumber";// sItemSelect;
        //            myCommand.CommandType = System.Data.CommandType.StoredProcedure;
        //            myCommand.Parameters.Add("@account_id", SqlDbType.VarChar, 30);

        //            myCommand.Parameters["@account_id"].Value = sAccountId;

        //            dr = myCommand.ExecuteReader();
        //            if (!dr.HasRows)
        //            {
        //                isValid = false;
        //            }
        //            else 
        //            {                        
        //                //set the session variable
        //                dr.Read();
        //                int iAccountNumber = (int)dr[0];
        //                string sAccountNumber = Convert.ToString(iAccountNumber);
        //                Session["AccountNumber"] = sAccountNumber;
        //            }
        //            dr.Close();
        //        }
        //    }
        //    catch (SqlException e)
        //    {
        //        isValid = false;
        //        EventLog woeLog = new EventLog("WebOrderEntry");
        //        woeLog.Source = "WebOrderEntryApp";
        //        string errorMessage = "Message\r\n" +
        //            e.Message.ToString() + "\r\n\r\n";
        //        errorMessage += "Source\r\n" +
        //            e.Source + "\r\n\r\n";
        //        errorMessage += "Target site\r\n" +
        //            e.TargetSite.ToString() + "\r\n\r\n";
        //        errorMessage += "Stack trace\r\n" +
        //            e.StackTrace + "\r\n\r\n";
        //        errorMessage += "ToString()\r\n\r\n" +
        //            e.ToString();
        //        woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 6);
        //    }
        //    return isValid;
        //}
    }

}
