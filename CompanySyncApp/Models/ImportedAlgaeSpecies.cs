using CsvHelper;

namespace CompanySyncApp.Models;


public sealed record ImportedAlgaeSpecies
{
    [CsvColumn("Algae species")]
    public string SpeciesName { get; set; } = string.Empty;
 
    [CsvColumn("Number of co's")]
    public int NumberOfCompanies { get; set; }
 
    [CsvColumn("Producers")]
    public string Producers { get; set; } = string.Empty;
 
    [CsvColumn("Genus")]
    public string Genus { get; set; } = string.Empty;
 
    [CsvColumn("Color")]
    public string Color { get; set; } = string.Empty;
 
    [CsvColumn("Common name")]
    public string CommonName { get; set; } = string.Empty;
}