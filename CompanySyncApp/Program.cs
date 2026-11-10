using Arb_Int.Domain;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Parties.Validation;
using Arb_Int.Domain.User;
using Arb_Int.Domain.Validation;
using CompanySyncApp.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CompanySyncApp;

internal static class Program
{
    private static async Task Main()
    {
        //     var connectionString = Environment.GetEnvironmentVariable(
        //         "COMPANYSYNC_DB_CONNECTION");
        //
        //     if (string.IsNullOrWhiteSpace(connectionString))
        //     {
        //         throw new InvalidOperationException(
        //             "Sätt COMPANYSYNC_DB_CONNECTION först.");
        //     }
        //
        //     var services = new ServiceCollection();
        //     services.AddLogging();
        //
        //     services.AddDbContext<Arb_IntDbContext>(options =>
        //     {
        //         options.UseNpgsql(connectionString);
        //         options.AddInterceptors(
        //             new NormalizedNameInterceptor(),
        //             new ActionTrackerInterceptor(new CurrentIdentityService()));
        //     });
        //
        //     services.AddIdentityCore<ApplicationUser>()
        //         .AddEntityFrameworkStores<Arb_IntDbContext>();
        //
        //     services.AddScoped<IValidationService, ValidationService>();
        //     services.AddScoped<
        //         IValidator<CreateOrganizationRequest>,
        //         CreateOrganizationRequestValidator>();
        //     services.AddScoped<
        //         IValidator<UpdateOrganizationRequest>,
        //         UpdateOrganizationRequestValidator>();
        //     services.AddScoped<
        //         IUserIdentitySynchronizationService,
        //         UserIdentitySynchronizationService>();
        //     services.AddScoped<IPartyService, PartyService>();
        //     services.AddScoped<CompanySyncService>();
        //
        //     await using var provider = services.BuildServiceProvider();
        //     await using var scope = provider.CreateAsyncScope();
        //
        //     var syncService = scope.ServiceProvider
        //         .GetRequiredService<CompanySyncService>();
        //
        //     var filePath = Path.Combine(
        //         AppContext.BaseDirectory,
        //         "data",
        //         "Companies-All companies.csv");
        //
        //     var source = new CsvCompanyDataSource(filePath);
        //     var companies = await source.GetCompaniesAsync();
        //
        //     if (companies.Count == 0)
        //     {
        //         Console.WriteLine("CSV-filen innehåller inga giltiga företag.");
        //         return;
        //     }
        //
        //     var created = 0;
        //     var skipped = 0;
        //     var failed = 0;
        //
        //     foreach (var company in companies)
        //     {
        //         var matches = await syncService.FindMatchesAsync(company);
        //
        //         if (matches.Count > 0)
        //         {
        //             Console.WriteLine($"{company.Name}: redan matchad, hoppas över.");
        //             skipped++;
        //             continue;
        //         }
        //
        //         var result = await syncService.CreateAsync(new CreateOrganizationRequest
        //         {
        //             Name = company.Name,
        //             LegalName = company.Name,
        //             OrganizationKind = OrganizationKind.Company
        //         });
        //
        //         if (result.IsFailed)
        //         {
        //             Console.Error.WriteLine(
        //                 $"{company.Name}: {string.Join("; ", result.Errors.Select(e => e.Message))}");
        //             failed++;
        //             continue;
        //         }
        //
        //         created++;
        //     }
        //
        //     Console.WriteLine($"Skapade: {created}, överhoppade: {skipped}, fel: {failed}");
        //
        //
        // }
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "data",
            "Companies-All companies.csv");

        var source = new CsvCompanyDataSource(filePath);
        var companies = await source.GetCompaniesAsync();

        var headquartersValues = companies
            .Select(company => company.Headquarters?.Trim())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value);

        foreach (var value in headquartersValues)
        {
            Console.WriteLine(value);
        }
    }
}
