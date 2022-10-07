using Core.Common.Exceptions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Core.Common
{
    public static class LoggingExtensionMethods
    {
        public static void LogInformationMethod(this ILogger logger, string scopeName, 
            string methodName, bool completed = false)
        {
            logger.LogInformation("[{ScopeName}] {methodName} {Action}.",
               scopeName, methodName, completed ? "executed" : "executing");
        }

        public static void LogInformationMethod(this ILogger logger, string scopeName, 
            string methodName, string entityName, Guid id, bool completed = false)
        {
            logger.LogInformation("[{ScopeName}] {MethodName} {Action}, {EntityName} Id: {Id}.",
                scopeName, methodName, completed ? "executed" : "executing", entityName, id);
        }

        public static void LogInformationAddedToEntity(this ILogger logger, string scopeName,
            string entity1Name, Guid entity1Id, string entity2Name, Guid entity2Id)
        {
            logger.LogInformation("[{ScopeName}] {Entity1} with Id {Id1} successfully assigned " +
                "to {Entity2} with Id {Id2}", scopeName, entity1Name, entity1Id, entity2Name, entity2Id);
        }

        [DoesNotReturn]
        public static void LogErrorAndThrowExceptionIdMismatch(this ILogger logger, string scopeName, 
            Guid requestId, Guid queryId)
        {
            logger.LogError("[{ScopeName}] Id ({RequestId}) in request data doesn't match Id ({QueryId}) in query", 
                scopeName, requestId, queryId);

            throw new CoreException($"Invalid id in request data.", HttpStatusCode.BadRequest);
        }

        [DoesNotReturn]
        public static void LogErrorAndThrowExceptionPageCount(this ILogger logger, string scopeName, 
            int totalPages, int pageNum)
        {
            var message = $"Total number of pages is {totalPages} and requested page number is {pageNum}";

            logger.LogError("[{ScopeName}] {Message}",
                scopeName, message);

            throw new CoreException(message, HttpStatusCode.BadRequest);
        }

        [DoesNotReturn]
        public static void LogErrorAndThrowExceptionNotFound(this ILogger logger, string scopeName, 
            string entityName, Guid? id = null)
        {
            var entityInfo = id != null ? $"{entityName} with Id {id}" : entityName;

            logger.LogError("[{ScopeName}] {EntityInfo} was not found.",
                scopeName, entityInfo);

            throw new CoreException($"Requested {entityName} was not found.", HttpStatusCode.NotFound);
        }

        [DoesNotReturn]
        public static void LogErrorAndThrowExceptionValueTaken(this ILogger logger, string scopeName,
            string entityName, string propertyName, string propertyValue)
        {
            var message = $"{entityName} with {propertyName} {propertyValue} already exists";

            logger.LogError("[{ScopeName}] {Message}", scopeName, message);

            throw new CoreException(message, HttpStatusCode.BadRequest);
        }

        [DoesNotReturn]
        public static void LogErrorAndThrowExceptionDuplicateEntries(this ILogger logger, string scopeName, 
            string entityName, string propertyName, IEnumerable<Guid> idList)
        {
            var message = $"{entityName} can't have duplicate {propertyName}";

            logger.LogError("[{ScopeName}] {Message}: {IdList}", scopeName, message, String.Join(", ", idList));

            throw new CoreException(message, HttpStatusCode.BadRequest);
        }

        [DoesNotReturn]
        public static void LogErrorAndThrowExceptionNotAllFound(this ILogger logger, string scopeName,
            string entityName, IEnumerable<Guid> idList)
        {
            var message = $"Not all {entityName} were found";

            logger.LogError("[{ScopeName}] {Message}: {IdList}", scopeName, message, String.Join(", ", idList));

            throw new CoreException(message, HttpStatusCode.NotFound);
        }
    }
}
