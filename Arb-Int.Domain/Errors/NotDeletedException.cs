using System.Globalization;

namespace Arb_Int.Domain.Errors
{
    public class NotDeletedException : Exception
    {
        public NotDeletedException() : base() { }

        public NotDeletedException(string message) : base(message) { }

        public NotDeletedException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }

}
