using Arb_Int.Domain.Errors;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Arb_Int.Domain.Validation
{
    public interface IValidationService
    {
        /// <summary>
        /// Validerar ett objekt med dess registrerade validator.
        /// <para>
        /// Kastar <see cref="InvalidOperationException"/> om ingen validator finns registrerad.
        /// </para>
        /// </summary>
        Task<Result> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Hämtar och kör registrerade FluentValidation-validatorer.
    /// </summary>
    public sealed class ValidationService : IValidationService
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Skapar valideringstjänsten för aktuell dependency injection-scope.
        /// </summary>
        public ValidationService(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Validerar ett objekt med dess registrerade validator.
        /// <para>
        /// Returnerar strukturerade <see cref="ValidationError"/> för varje misslyckad regel.<br />
        /// Saknad validator behandlas som ett konfigurationsfel och kastar <see cref="InvalidOperationException"/>.
        /// </para>
        /// </summary>
        public async Task<Result> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(instance);

            var validator = _provider.GetService<IValidator<T>>();

            if (validator is null)
            {
                throw new InvalidOperationException(
                    $"No validator is registered for type '{typeof(T).FullName}'.");
            }

            var validationResult = await validator.ValidateAsync(instance, cancellationToken);

            if (validationResult.IsValid)
            {
                return Result.Ok();
            }

            var errors = validationResult.Errors
                .Select(failure => new ValidationError(
                    failure.PropertyName,
                    failure.ErrorMessage,
                    failure.ErrorCode,
                    failure.Severity));

            return Result.Fail(errors);
        }
    }
}