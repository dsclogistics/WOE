using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebOrderEntry
{
    public partial class XmlDisplay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string formatForDisplay = (string)Session["xmlForProduction"];
            string formatForDisplay1 = formatForDisplay.Replace("<", "&lt");
            string formatForDisplay2 = formatForDisplay1.Replace(">", "&gt");
            Label1.Text = formatForDisplay2;
        }
    }
}
