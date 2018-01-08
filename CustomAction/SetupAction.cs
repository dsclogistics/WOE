using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.Diagnostics;

namespace CustomAction
{
    [RunInstaller(true)]
    public class SetupAction : Installer
    {
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            if (!EventLog.Exists("WebOrderEntry"))
            {
                EventLog.CreateEventSource("WebOrderEntryApp", "WebOrderEntry");
            }
        }

    }
}
