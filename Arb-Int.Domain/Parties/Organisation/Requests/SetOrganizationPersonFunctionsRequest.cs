namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller den kompletta uppsättningen funktioner för en organisationsrelation.
    /// </summary>
    public sealed class SetOrganizationPersonFunctionsRequest
    {
        /// <summary>
        /// Identifierare för de funktionstyper som relationen ska ha.
        /// <para>
        /// En tom lista tar bort samtliga funktioner.
        /// </para>
        /// </summary>
        public IReadOnlyCollection<int> ContactFunctionTypeIds { get; init; } = [];
    }
}