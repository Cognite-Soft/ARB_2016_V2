using System;
using System.IO;
using System.Text;
using System.Web.Http.Filters;
using Cognite.Arb.Server.WebApi.ExceptionHandling;

namespace Cognite.Arb.Server.WebApi
{
    public class FileExceptionLogger : IExceptionLogger
    {
        private readonly string _path;

        public FileExceptionLogger(string path)
        {
            _path = path;
        }

        public void Log(HttpActionExecutedContext context)
        {
            var logEntry = CreateLogEntry(context);
            File.AppendAllText(_path, logEntry);
        }

        private string CreateLogEntry(HttpActionExecutedContext context)
        {
            var result = new StringBuilder();

            var time = DateTime.Now.ToString();
            result.AppendLine(time);
            result.AppendFormat("Controller = {0}", context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName).AppendLine();
            result.AppendFormat("Action = {0}", context.ActionContext.ActionDescriptor.ActionName).AppendLine();
            result.AppendFormat("Request URI = {0}", context.Request.RequestUri).AppendLine();
            WriteException(result, context.Exception);
            result.AppendFormat("---------- ---------- ---------- ---------- {0}", time).AppendLine();

            return result.ToString();
        }

        private void WriteException(StringBuilder result, Exception exception)
        {
            result.AppendFormat("Exception = {0}", exception.GetType().FullName).AppendLine();
            result.AppendFormat("Message = {0}", exception.Message).AppendLine();
            result.AppendFormat("StackTrace = {0}", exception.StackTrace).AppendLine();

            var innerException = exception.InnerException;
            while (innerException != null)
            {
                result.AppendFormat("InnerException = {0}", innerException.GetType().FullName).AppendLine();
                result.AppendFormat("Message = {0}", innerException.Message).AppendLine();
                innerException = innerException.InnerException;
            }
        }
    }
}