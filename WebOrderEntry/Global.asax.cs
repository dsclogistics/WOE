using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Diagnostics;

namespace WebOrderEntry
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            //System.Diagnostics.EventLog.Delete("WebOrderEntry");

            //if (!EventLog.Exists("WebOrderEntry"))
            //{
            //    EventLog.CreateEventSource("WebOrderEntryApp", "WebOrderEntry");
            //}
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //// Log error to the Event Log
            Exception myError = null;
            if (HttpContext.Current.Server.GetLastError() != null)
            {
                string eventLog = "WebOrderEntry";
                string eventSource = "WebOrderEntryApp";
                string myErrorMessage = "";

                myError = Server.GetLastError();
                while (myError.InnerException != null)
                {
                    myErrorMessage += "Message\r\n" +
                        myError.Message.ToString() + "\r\n\r\n";
                    myErrorMessage += "Source\r\n" +
                        myError.Source + "\r\n\r\n";
                    myErrorMessage += "Target site\r\n" +
                        myError.TargetSite.ToString() + "\r\n\r\n";
                    myErrorMessage += "Stack trace\r\n" +
                        myError.StackTrace + "\r\n\r\n";
                    myErrorMessage += "ToString()\r\n\r\n" +
                        myError.ToString();

                    // Assign the next InnerException
                    // to catch the details of that exception as well
                    myError = myError.InnerException;
                }

                // Make sure the Eventlog Exists
                if (EventLog.SourceExists(eventSource))
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog(eventLog);
                    myLog.Source = eventSource;

                    // Write the error entry to the event log.    
                    myLog.WriteEntry("An error occurred in the Web application " + eventSource + "\r\n\r\n" + myErrorMessage,
                        EventLogEntryType.Error, 19);
                }
            }

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}