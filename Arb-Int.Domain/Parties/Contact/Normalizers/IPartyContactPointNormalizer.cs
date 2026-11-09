namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Normaliserar kontaktvärden till det kanoniska format som används för jämförelse och unikhet.
    /// </summary>
    public interface IPartyContactPointNormalizer
    {
        /// <summary>
        /// Normaliserar ett kontaktvärde utifrån dess typ.
        /// <para>Exempel:<br />
        /// e-post till trim + lowercase<br />
        /// telefon till E.164<br />
        /// URL till kanonisk URI</para>
        /// </summary>
        string Normalize(ContactPointKind kind, string value);
    }
}