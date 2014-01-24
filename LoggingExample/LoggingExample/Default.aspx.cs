using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LoggingExample
{
    public partial class _Default : Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            var ex = new Exception();
            log.Error(ex);
            log.Debug("Debug logging");
            log.Info("Info logging");
            log.Warn("Warn logging");
            log.Error("Error logging");
            log.Fatal("Fatal logging");
        }
    }
}