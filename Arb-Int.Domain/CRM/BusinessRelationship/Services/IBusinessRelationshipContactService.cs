using FluentResults;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Hanterar kontaktpersoner för specifika affärsrelationer.
    /// </summary>
    public interface IBusinessRelationshipContactService
    {
        /// <summary>
        /// Hämtar samtliga kontakter för en affärsrelation.
        /// </summary>
        Task<Result<IReadOnlyList<BusinessRelationshipContactResponse>>> GetAsync(Guid businessRelationshipId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar en enskild relationskontakt.
        /// </summary>
        Task<Result<BusinessRelationshipContactResponse>> GetByIdAsync(Guid businessRelationshipContactId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lägger till en kontakt på en affärsrelation.
        /// </summary>
        Task<Result<BusinessRelationshipContactResponse>> CreateAsync(Guid businessRelationshipId, CreateBusinessRelationshipContactRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar roll och metadata för en relationskontakt.
        /// </summary>
        Task<Result<BusinessRelationshipContactResponse>> UpdateAsync(Guid businessRelationshipContactId, UpdateBusinessRelationshipContactRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en kontaktkoppling från affärsrelationen.
        /// </summary>
        Task<Result> DeleteAsync(Guid businessRelationshipContactId, CancellationToken cancellationToken = default);
    }
}