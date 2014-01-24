

namespace LoggingInformation
{
    using System;
    using System.IO;
    using System.Web;

    public class HttpHandler : IHttpHandler
    {

        private string LogFileFolderUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["LogFileLocation"].ToString();
        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {

            
            var date = context.Request.QueryString["date"];

            if (date != null && !string.IsNullOrEmpty(date.ToString()))
            {


                LogFileFolderUrl = LogFileFolderUrl + date + ".log";

                if (string.IsNullOrEmpty(LogFileFolderUrl))
                {
                    return;
                }

                string destPath = context.Server.MapPath(LogFileFolderUrl);

                FileInfo fi = new FileInfo(destPath);

                if (fi.Exists)
                {
                    HttpContext.Current.Response.ClearHeaders();
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.AppendHeader("Content-Length", fi.Length.ToString());
                    HttpContext.Current.Response.ContentType = "text/plain";
                    HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=LogFile" + date + ".log");
                    HttpContext.Current.Response.BinaryWrite(ReadByteArryFromFile(destPath));
                    HttpContext.Current.Response.End();
                }
            }
            else
            {
                string destPath = context.Server.MapPath(LogFileFolderUrl);
                var strHtml = ShowLogFiles(destPath.Substring(0, destPath.LastIndexOf('\\') + 1));
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.AppendHeader("Content-Length", strHtml.Length.ToString());
                HttpContext.Current.Response.ContentType = "text/html";
                //HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=LogFile" + date + ".log");
                HttpContext.Current.Response.Write(strHtml);
                HttpContext.Current.Response.End();
            }

        }

        private byte[] ReadByteArryFromFile(string destPath)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(destPath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(destPath).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }


        private string ShowLogFiles(string LogFolderPath)
        {
            var files = Directory.GetFiles(LogFolderPath, "*.log");
            var rtnstr = string.Empty;
            foreach (var item in files)
            {
                rtnstr += "<div style='padding:10px;margin:10px;background:#ddd;border:1px solid #eee;'>"+item+"</div>";
            }

            return rtnstr;
        }

        #endregion
    }
}
