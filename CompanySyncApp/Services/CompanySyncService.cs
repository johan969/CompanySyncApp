using Arb_Int.Domain;
using Arb_Int.Domain.Parties;
using CompanySyncApp.Models;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CompanySyncApp.Services;

public sealed class CompanySyncService
{
    private readonly Arb_IntDbContext _db;
    private readonly IPartyService _partyService;

    public CompanySyncService(Arb_IntDbContext db, IPartyService partyService)
    {
        _db = db; 
        _partyService = partyService;
    }

    public Task<List<Organization>> FindMatchesAsync(
        ImportedCompany company,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = NameNormalizer.Normalize(company.Name);

        return _db.Organizations
            .AsNoTracking()
            .Where(org => org.NormalizedLegalName == normalizedName)
            .ToListAsync(cancellationToken);
    }

    public Task<Result<PartyDetailsResponse>> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        return _partyService.CreateOrganizationAsync(request, cancellationToken);
    }
    
    public Task<Result<PartyDetailsResponse>> UpdateAsync(
        Guid organizationId,
        UpdateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        return _partyService.UpdateOrganizationAsync(
            organizationId, request, cancellationToken);
    }
    



}
