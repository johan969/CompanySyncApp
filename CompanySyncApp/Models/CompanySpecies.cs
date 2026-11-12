namespace CompanySyncApp.Models;

public sealed class CompanySpecies
{
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public Guid SpeciesId { get; set; }
    public AlgaeSpecies Species { get; set; } = null!;
}