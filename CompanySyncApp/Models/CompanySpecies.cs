using CompanySyncApp.Models.Enums;

namespace CompanySyncApp.Models;

public sealed class CompanySpecies
{
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public Guid SpeciesId { get; set; }
    public AlgaeSpecies Species { get; set; } = null!;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.ObservedInPublicSource;
}
