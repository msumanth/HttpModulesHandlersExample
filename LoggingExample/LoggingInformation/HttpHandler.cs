

namespace LoggingInformation
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Linq;
using System.Text;

    public class HttpHandler : IHttpHandler
    {
        

        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var LogFileFolderUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["LogFileLocation"].ToString();
            var filename = context.Request.QueryString["filename"];
            if (filename != null && !string.IsNullOrEmpty(filename.ToString()))
            {
                LogFileFolderUrl = LogFileFolderUrl + filename;

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
                    HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename);
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
            var sbreturnstring = new StringBuilder();
            var dir = new DirectoryInfo(LogFolderPath);

            FileInfo[] files = dir.GetFiles().OrderBy(d => d.CreationTime).ToArray();
            
            sbreturnstring.Append("<div style='margin:2% 10%'><h1>Log Files Downloader</h1><ul>");

            foreach (var item in files)
            {
                sbreturnstring.Append("<li style='display: inline-block;border:2px solid #eee;background:#ddd;padding:5px;margin:5px;'><div style='display:block;'>")
                    .Append("<span style='display:block;'>").Append(item.FullName.Substring(item.FullName.LastIndexOf("\\") + 1)).Append("</span>")
                    .Append("<span style='display:block;'>").Append("<a href='/showlog.axd?filename=")
                    .Append(item.FullName.Substring(item.FullName.LastIndexOf("\\") + 1)).Append("'>Download</a></span>")
                    .Append("</div></li>");
            }
            sbreturnstring.Append("</ul>");
            return sbreturnstring.ToString();
        }

        #endregion
    }
}
