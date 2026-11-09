using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Arb_Int.Domain.Errors
{
    public abstract class DomainError : Error
    {
        public int StatusCode { get; }

        protected DomainError(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
            base.WithMetadata("status", statusCode);
        }
    }

    public sealed class ValidationError : DomainError
    {
        public IDictionary<string, string[]> Failures { get; private set; }
        public ValidationError(IDictionary<string, string[]> failures)
            : base(string.Join(", ", failures.Keys), StatusCodes.Status400BadRequest)
        {
            Failures = failures;
            Severity = Severity.Error;
            PropertyName = string.Join(", ", failures.Keys);
            ErrorCode = "ValidationError";
        }
        /// <summary>
        /// Skapar ett strukturerat valideringsfel.
        /// </summary>
        public ValidationError(string propertyName, string errorMessage, string errorCode, Severity severity)
            : base(errorMessage, StatusCodes.Status400BadRequest)
        {
            Failures = new Dictionary<string, string[]> { { propertyName, new[] { errorMessage } } };
            PropertyName = propertyName;
            Severity = severity;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Namnet på egenskapen som inte klarade valideringen.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// FluentValidation-felkoden för den misslyckade regeln.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Anger valideringsfelets allvarlighetsgrad.
        /// </summary>
        public Severity Severity { get; }
    }
    public class NotDeletedError : DomainError
    {
        public string EntityName { get; }
        public object Id { get; }
        public NotDeletedError(string entityName, object id)
            : base($"'{entityName}' with Id='{id}' is not deleted.", StatusCodes.Status403Forbidden)
        {
            EntityName = entityName;
            Id = id;
        }
    }
    public class NotFoundError : DomainError
    {
        public string EntityName { get; }
        public object Id { get; }
        public NotFoundType Type { get; }

        public NotFoundError(string entityName, object id, NotFoundType type = NotFoundType.Id)
            : base($"'{entityName}' with {type.ToString().ToLower()} '{id}' not found.", StatusCodes.Status404NotFound)
        {
            EntityName = entityName;
            Id = id;
            Type = type;
        }
    }
    public class EmptyError : DomainError
    {
        public EmptyError()
            : base("[Error.Empty] No values found", StatusCodes.Status404NotFound)
        { }
    }
    public enum AlreadyExistType : int
    {
        Id = 1,
        Name = 2,
        Setup = 3,
        Path = 4
    };
    public enum NotFoundType : int
    {
        /// <summary>
        /// When an Id is not found
        /// </summary>
        Id = 1,
        /// <summary>
        /// When a Name or other string is not found
        /// </summary>
        Name = 2,
        /// <summary>
        /// When a Setup is not found
        /// </summary>
        Setup = 3,
        /// <summary>
        /// When a Path is not found
        /// </summary>
        Path = 4,
        /// <summary>
        /// When no entities matches a filter
        /// </summary>
        Filter = 5,
        /// <summary>
        /// When a Collection is empty
        /// </summary>
        Collection = 6
    };
    public class AlreadyExistsError : DomainError
    {
        public string EntityName { get; }
        public object Id { get; }
        public AlreadyExistType Type { get; }

        public AlreadyExistsError(string entityName, object id, AlreadyExistType type = AlreadyExistType.Id)
            : base($"'{entityName}' with '{type.ToString().ToLower()}' = {id} already exists.", StatusCodes.Status409Conflict)
        {
            EntityName = entityName;
            Id = id;
            Type = type;
        }
    }
    public enum NotAvailableType : int
    {
        Action = 0,
        Notes = 1,
        Tags = 2,
        History = 3,
    };
    public class NotAvailableInEntityError : DomainError
    {
        public string EntityName { get; }
        public NotAvailableType Type { get; } = NotAvailableType.Action;
        public NotAvailableInEntityError(string entityName, NotAvailableType type = NotAvailableType.Action)
            : base(message: $"{type} are not supported by entity '{entityName}'", statusCode: StatusCodes.Status403Forbidden)
        {
            EntityName = entityName;
            Type = type;
        }
    }
    public class ConflictError : DomainError
    {
        public ConflictError() :
            base("[Error.ConflictDetected] Conflict detected", StatusCodes.Status409Conflict)
        { }
        public ConflictError(string message) :
            base(message, StatusCodes.Status409Conflict)
        { }
    }
    /// <summary>
    /// Anger att ett objekt har ändrats sedan klienten senast läste det.
    /// </summary>
    public sealed class ConcurrencyError : DomainError
    {
        /// <summary>
        /// Skapar ett concurrency-fel för ett domänobjekt.
        /// </summary>
        public ConcurrencyError(string entityName, object key)
            : base($"{entityName} with key '{key}' has been modified by another operation.", StatusCodes.Status409Conflict)
        {
            EntityName = entityName;
            Key = key;
        }

        /// <summary>
        /// Typen av objekt som ändrades.
        /// </summary>
        public string EntityName { get; }

        /// <summary>
        /// Nyckeln för objektet som ändrades.
        /// </summary>
        public object Key { get; }
    }

    public class UnauthorizedError : DomainError
    {
        public UnauthorizedError(string? message = null) :
            base(message ?? "[Error.UnauthorizedRequest] Unauthorized request", StatusCodes.Status401Unauthorized)
        { }
    }
    public class ForbiddenError : DomainError
    {
        public ForbiddenError() :
            base("[Error.ForbiddenRequest] Forbidden request", StatusCodes.Status403Forbidden)
        { }
    }
    public class ThrottlingError : DomainError
    {
        public int RetryAfter { get; }
        public ThrottlingError(int retryAfter) :
            base("[Error.TooManyRequests] Too many request", StatusCodes.Status429TooManyRequests)
        {
            RetryAfter = retryAfter;
        }
    }
    public class ProcessError : DomainError
    {
        public ProcessError(string message) :
            base(message, StatusCodes.Status400BadRequest)
        { }
    }
    public class NullReferenceError : DomainError
    {
        public NullReferenceError(string message) :
            base(message, StatusCodes.Status500InternalServerError)
        { }
    }
    public class ServerError : DomainError
    {
        public ServerError() :
            base("[Error.ServerError] Server error", StatusCodes.Status500InternalServerError)
        { }
    }

    public class TokenError : DomainError
    {
        public TokenError(string message) :
            base(message, StatusCodes.Status401Unauthorized)
        { }
    }
    public class LoginError : DomainError
    {
        public LoginError(string? message = null) :
            base(message ?? "[Error.Login] Login failed", StatusCodes.Status401Unauthorized)
        { }
    }
    public class BadRequestError : DomainError
    {
        public BadRequestError(string? message = null) :
            base(message ?? "[Error.BadRequest] Bad Request", StatusCodes.Status400BadRequest)
        { }
    }
}
