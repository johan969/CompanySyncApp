namespace CompanySyncApp.Models;

public sealed class AlgaeSpecies
{
    public Guid Id { get; set; }
    public string SpeciesName { get; set; } = string.Empty;
    public string Genus { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string CommonName { get; set; } = string.Empty;
    public ICollection<CompanySpecies> CompanyLinks { get; set; }
        = new List<CompanySpecies>();
}