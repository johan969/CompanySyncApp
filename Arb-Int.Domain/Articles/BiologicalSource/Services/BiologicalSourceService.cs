using Arb_Int.Domain.Articles.Projections;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Läser reference-data för biologiska källor för uppslag och filtrering.
    /// </summary>
    public sealed class BiologicalSourceService : IBiologicalSourceService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public BiologicalSourceService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar en biologisk källa.
        /// </summary>
        public async Task<Result<BiologicalSourceResponse>> GetByIdAsync(Guid biologicalSourceId, CancellationToken cancellationToken = default)
        {
            var response = await _db.Set<BiologicalSource>()
                .AsNoTracking()
                .Where(x => x.Id == biologicalSourceId)
                .Select(BiologicalSourceProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                return Result.Fail<BiologicalSourceResponse>(
                    new NotFoundError(
                        nameof(BiologicalSource),
                        biologicalSourceId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar biologiska källor.
        /// </summary>
        public async Task<Result<PagedResult<BiologicalSourceResponse>>> QueryAsync(BiologicalSourceQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<BiologicalSourceResponse>>(
                    validationResult.Errors);
            }

            IQueryable<BiologicalSource> query = _db.Set<BiologicalSource>()
                .AsNoTracking();

            if (request.RecordKind.HasValue)
            {
                query = query.Where(x => x.RecordKind == request.RecordKind.Value);
            }

            if (request.DeclaredRank.HasValue)
            {
                query = query.Where(x => x.DeclaredRank == request.DeclaredRank.Value);
            }

            if (request.ParentBiologicalSourceId.HasValue)
            {
                query = query.Where(x => x.ParentBiologicalSourceId == request.ParentBiologicalSourceId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var pattern = $"%{request.Search.Trim()}%";

                query = query.Where(x =>
                    EF.Functions.ILike(x.Name, pattern)
                    || EF.Functions.ILike(x.Slug, pattern)
                    || EF.Functions.ILike(x.Description, pattern)
                    || x.AlternativeNames.Any(alternativeName =>
                        EF.Functions.ILike(alternativeName.Name, pattern)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(
                query,
                request.SortBy,
                request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(BiologicalSourceProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok(
                new PagedResult<BiologicalSourceResponse>
                {
                    Items = items,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                });
        }

        /// <summary>
        /// Sorterar frågan och håller ordningen stabil via en explicit tiebreaker.
        /// </summary>
        private static IQueryable<BiologicalSource> ApplySorting(IQueryable<BiologicalSource> query, BiologicalSourceSortField sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (BiologicalSourceSortField.Name, SortDirection.Descending) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (BiologicalSourceSortField.Slug, SortDirection.Ascending) => query.OrderBy(x => x.Slug).ThenBy(x => x.Id),
                (BiologicalSourceSortField.Slug, SortDirection.Descending) => query.OrderByDescending(x => x.Slug).ThenByDescending(x => x.Id),
                (BiologicalSourceSortField.RecordKind, SortDirection.Ascending) => query.OrderBy(x => x.RecordKind).ThenBy(x => x.Name).ThenBy(x => x.Id),
                (BiologicalSourceSortField.RecordKind, SortDirection.Descending) => query.OrderByDescending(x => x.RecordKind).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id)
            };
        }
    }
}
