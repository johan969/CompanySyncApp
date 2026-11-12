namespace CompanySyncApp.Models;

public sealed record ImportedCompany
{
    
    
    public required string Name { get; init; }
    public string? Headquarters { get; init; }
    public string? ValueChain { get; init; }
    public string? Website { get; init; }
    public string? Species { get; init; }
    public string? ProductionProcess { get; init; }
    public string? IndustryApplications { get; init; }
    public string? IndustrySubsectorTags { get; init; }
    public string? Investors { get; init; }
    public string? AssociationFocus { get; init; }
    public int? FoundingYear { get; init; }
    public string? StartupScaleupStatus { get; init; }
    public decimal? DisclosedInvestmentsSince2020 { get; init; }
    public string? DevelopmentStage { get; init; }
    public DateOnly? LastModified { get; init; }
}
