using System.Globalization;

namespace Arb_Int.Domain.Errors
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException() : base() { }

        public InvalidTokenException(string message) : base(message) { }

        public InvalidTokenException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
