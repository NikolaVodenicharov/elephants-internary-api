using Core.Common.Exceptions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;

namespace Core.Common
{
    public static class Guard
    {
        public static void EnsureNotNull<T>([NotNull] T? value, ILogger logger, string scopeName,
            string entityName, Guid? id = null)
        {
            if (value != null)
            {
                return;
            }

            logger.LogErrorAndThrowExceptionNotFound(scopeName, entityName, id);
        }

        public static void EnsureNotNullPagination([NotNull] int? pageNum, [NotNull] int? pageSize,
            ILogger logger, string scopeName)
        {
            if (pageNum != null && pageSize != null)
            {
                return;
            }

            var message = new StringBuilder();

            if (pageNum == null)
            {
                message.Append("Page Num is required. ");
            }

            if (pageSize == null)
            {
                message.Append("Page Size is required.");
            }

            logger.LogError("[{ScopeName}] {Message}",
                scopeName, message);

            throw new CoreException(message.ToString(), HttpStatusCode.BadRequest);
        }
    }
}
