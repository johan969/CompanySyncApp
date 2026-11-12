namespace CompanySyncApp.Models;

public sealed class Company
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Headquarters { get; set; }
    public string? ValueChain { get; set; }
    public string? Website { get; set; }
    public string? ProductionProcess { get; set; }
    public string? IndustryApplications { get; set; }
    public string? IndustrySubsectorTags { get; set; }
    public string? Investors { get; set; }
    public string? AssociationFocus { get; set; }
    public int? FoundingYear { get; set; }
    public string? StartupScaleupStatus { get; set; }
    public decimal? DisclosedInvestmentsSince2020 { get; set; }
    public string? DevelopmentStage { get; set; }
    public ICollection<CompanySpecies> SpeciesLinks { get; set; }
        = new List<CompanySpecies>();
    public DateOnly? LastModified { get; set; }
}