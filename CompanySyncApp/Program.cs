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
        var connectionString = Environment.GetEnvironmentVariable(
            "COMPANYSYNC_DB_CONNECTION");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Sätt COMPANYSYNC_DB_CONNECTION innan du kör importen.");
        }

        //skapar en service collection som innehåller alla services som behövs för att köra programmet
        var services = new ServiceCollection();
        // så vi kan använda logging i programmet, för te
        services.AddLogging();



        //Vi skapr en DBkotext för att kunna kommunicera med databasen vi lägger till kontexten från Arb_IntDbContext
        // och vi lägger till interceptors för normalizering av namn och conectionstring med postgres
        services.AddDbContext<Arb_IntDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.AddInterceptors(
                new NormalizedNameInterceptor(),
                new ActionTrackerInterceptor(new CurrentIdentityService()));
        });

        services.AddIdentityCore<ApplicationUser>()
            .AddEntityFrameworkStores<Arb_IntDbContext>();


            // validator och IValidationService för att validera data som hämtas från csv filen (iValidator har Validator som dependency injection)
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<
            IValidator<CreateOrganizationRequest>,
            CreateOrganizationRequestValidator>();
        services.AddScoped<
            IValidator<UpdateOrganizationRequest>,
            UpdateOrganizationRequestValidator>();
        services.AddScoped<
            IUserIdentitySynchronizationService,
            UserIdentitySynchronizationService>();
        services.AddScoped<IPartyService, PartyService>();
        services.AddScoped<CompanySyncService>();

        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var syncService = scope.ServiceProvider
            .GetRequiredService<CompanySyncService>();


            //skapar path till csv filen
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "data",
            "Companies-All companies.csv");

            //skapar en instans av CsvCompanyDataSource som hämtar företagen från csv filen
        var source = new CsvCompanyDataSource(filePath);
        //hämtar företagen från csv
        var companies = await source.GetCompaniesAsync();

        // om det inte finns några företag i csv filen så skrivs ett meddelande ut och programmet avslutas
        if (companies.Count == 0)
        {
            Console.WriteLine("CSV-filen innehåller inga giltiga företag.");
            return;
        }

        var created = 0;
        var updated = 0;
        var unchanged = 0;
        var failed = 0;


        //loopar igenom alla företag i csv filen
        foreach (var company in companies)
        {
            // hittar matchande företag i databasen
            var matches = await syncService.FindMatchesAsync(company);

            if (matches.Count == 0)
            {
                var createResult = await syncService.CreateAsync(
                    new CreateOrganizationRequest
                    {
                        Name = company.Name,
                        LegalName = company.Name,
                        OrganizationKind = OrganizationKind.Company
                    });

                if (createResult.IsFailed)
                {
                    Console.Error.WriteLine(
                        $"{company.Name}: {string.Join("; ", createResult.Errors.Select(error => error.Message))}");
                    failed++;
                    continue;
                }

                created++;
                continue;
            }

            foreach (var organization in matches)
            {
                bool needsUpdate =
                    !string.Equals(
                        organization.Name,
                        company.Name,
                        StringComparison.Ordinal)
                    || !string.Equals(
                        organization.LegalName,
                        company.Name,
                        StringComparison.Ordinal);

                if (!needsUpdate)
                {
                    unchanged++;
                    continue;
                }

                var updateResult = await syncService.UpdateAsync(
                    organization.Id,
                    new UpdateOrganizationRequest
                    {
                        Name = company.Name,
                        LegalName = company.Name,
                        TradingName = organization.TradingName,
                        OrganizationKind = organization.OrganizationKind,
                        IsInternal = organization.IsInternal,
                        RegistrationCountryId = organization.RegistrationCountryId,
                        ParentOrganizationId = organization.ParentOrganizationId,
                        Description = organization.Description,
                        Notes = organization.Notes,
                        IsActive = organization.IsActive
                    });

                if (updateResult.IsFailed)
                {
                    Console.Error.WriteLine(
                        $"{company.Name} ({organization.Id}): " +
                        $"{string.Join("; ", updateResult.Errors.Select(error => error.Message))}");
                    failed++;
                    continue;
                }

                updated++;
            }
        }

        Console.WriteLine(
            $"Skapade: {created}, uppdaterade: {updated}, oförändrade: {unchanged}, fel: {failed}");
    }
}
