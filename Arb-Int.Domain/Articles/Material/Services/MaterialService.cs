using Arb_Int.Domain.Articles.Projections;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Läser material reference-data för uppslag och filtrering.
    /// </summary>
    public sealed class MaterialService : IMaterialService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public MaterialService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar ett material.
        /// </summary>
        public async Task<Result<MaterialResponse>> GetByIdAsync(Guid materialId, CancellationToken cancellationToken = default)
        {
            var response = await _db.Set<Material>()
                .AsNoTracking()
                .Where(x => x.Id == materialId)
                .Select(MaterialProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                return Result.Fail<MaterialResponse>(
                    new NotFoundError(
                        nameof(Material),
                        materialId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar materials.
        /// </summary>
        public async Task<Result<PagedResult<MaterialResponse>>> QueryAsync(MaterialQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<MaterialResponse>>(
                    validationResult.Errors);
            }

            IQueryable<Material> query = _db.Set<Material>()
                .AsNoTracking();

            if (!request.IncludeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            if (request.MaterialTypeId.HasValue)
            {
                query = query.Where(x => x.MaterialTypeId == request.MaterialTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var pattern = $"%{request.Search.Trim()}%";

                query = query.Where(x =>
                    EF.Functions.ILike(x.Name, pattern)
                    || EF.Functions.ILike(x.Slug, pattern)
                    || EF.Functions.ILike(x.Description, pattern));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(
                query,
                request.SortBy,
                request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(MaterialProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok(
                new PagedResult<MaterialResponse>
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
        private static IQueryable<Material> ApplySorting(IQueryable<Material> query, MaterialSortField sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (MaterialSortField.Name, SortDirection.Descending) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (MaterialSortField.Slug, SortDirection.Ascending) => query.OrderBy(x => x.Slug).ThenBy(x => x.Id),
                (MaterialSortField.Slug, SortDirection.Descending) => query.OrderByDescending(x => x.Slug).ThenByDescending(x => x.Id),
                (MaterialSortField.IsActive, SortDirection.Ascending) => query.OrderBy(x => x.IsActive).ThenBy(x => x.Name).ThenBy(x => x.Id),
                (MaterialSortField.IsActive, SortDirection.Descending) => query.OrderByDescending(x => x.IsActive).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id)
            };
        }
    }
}
