using System.Globalization;
using CompanySyncApp.Models;
using Microsoft.VisualBasic.FileIO;

namespace CompanySyncApp.Services;

public sealed class CsvCompanyDataSource
{
    private readonly string _filePath;

    public CsvCompanyDataSource(string filePath)
    {
        _filePath = filePath;
    }

    public Task<IReadOnlyList<ImportedCompany>> GetCompaniesAsync(
        CancellationToken cancellationToken = default)
    {
        using var parser = new TextFieldParser(_filePath);
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        var headers = parser.ReadFields()
            ?? throw new FormatException("CSV-filen saknar kolumnrubriker.");

        var columns = headers
            .Select((header, index) => (Name: header.Trim(), Index: index))
            .ToDictionary(column => column.Name, column => column.Index,
                StringComparer.OrdinalIgnoreCase);

        var companies = new List<ImportedCompany>();

        while (!parser.EndOfData)
        {
            cancellationToken.ThrowIfCancellationRequested();

            long lineNumber = parser.LineNumber;
            var fields = parser.ReadFields();

            if (fields is null || fields.Length != headers.Length)
            {
                throw new FormatException(
                    $"Rad {lineNumber}: antalet värden matchar inte rubrikerna.");
            }

            string? Read(string column)
            {
                if (!columns.TryGetValue(column, out int index))
                {
                    throw new FormatException($"CSV-filen saknar kolumnen '{column}'.");
                }

                string value = fields[index].Trim();
                return value.Length == 0 ? null : value;
            }

            var name = Read("Name");
            if (name is null)
            {
                Console.Error.WriteLine($"Rad {lineNumber} hoppades över: Name saknas.");
                continue;
            }
            companies.Add(new ImportedCompany
            {
                Name = Read("Name")
                    ?? throw new FormatException($"Rad {lineNumber}: Name får inte vara tomt."),
                Headquarters = Read("Headquarters"),
                ValueChain = Read("Value chain"),
                Website = Read("Website"),
                Species = Read("Species"),
                ProductionProcess = Read("Production process"),
                IndustryApplications = Read("Industry (Applications)"),
                IndustrySubsectorTags = Read("Industry subsector tags"),
                Investors = Read("Investors"),
                AssociationFocus = Read("Association focus"),
                FoundingYear = ParseOptional("Founding year",
                    value => int.Parse(value, CultureInfo.InvariantCulture)),
                StartupScaleupStatus = Read("Start-up/scale-up?"),
                DisclosedInvestmentsSince2020 = ParseOptional(
                    "Total disclosed investments since 2020",
                    value => decimal.Parse(value, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"))),
                DevelopmentStage = Read("Development stage"),
                LastModified = ParseOptional("Last Modified",
                    value => DateTimeOffset.Parse(
                        value, CultureInfo.GetCultureInfo("sv-SE"), DateTimeStyles.AssumeUniversal))
            });

            T? ParseOptional<T>(string column, Func<string, T> parse) where T : struct
            {
                string? value = Read(column);

                if (value is null)
                {
                    return null;
                }

                try
                {
                    return parse(value);
                }
                catch (Exception ex) when (ex is FormatException or OverflowException)
                {
                    throw new FormatException(
                        $"Rad {lineNumber}: ogiltigt värde i '{column}': '{value}'.", ex);
                }
            }
        }

        return Task.FromResult<IReadOnlyList<ImportedCompany>>(companies);
    }
}