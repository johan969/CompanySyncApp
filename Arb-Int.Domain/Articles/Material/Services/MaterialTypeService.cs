using Arb_Int.Domain.Articles.Projections;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Läser material type reference-data för uppslag och filtrering.
    /// </summary>
    public sealed class MaterialTypeService : IMaterialTypeService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public MaterialTypeService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar en material type.
        /// </summary>
        public async Task<Result<MaterialTypeResponse>> GetByIdAsync(int materialTypeId, CancellationToken cancellationToken = default)
        {
            var response = await _db.Set<MaterialType>()
                .AsNoTracking()
                .Where(x => x.Id == materialTypeId)
                .Select(MaterialTypeProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                return Result.Fail<MaterialTypeResponse>(
                    new NotFoundError(
                        nameof(MaterialType),
                        materialTypeId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar material types.
        /// </summary>
        public async Task<Result<PagedResult<MaterialTypeResponse>>> QueryAsync(MaterialTypeQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<MaterialTypeResponse>>(
                    validationResult.Errors);
            }

            IQueryable<MaterialType> query = _db.Set<MaterialType>()
                .AsNoTracking();

            if (!request.IncludeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            if (request.ParentMaterialTypeId.HasValue)
            {
                query = query.Where(x => x.ParentMaterialTypeId == request.ParentMaterialTypeId.Value);
            }

            if (request.IsRawMaterial.HasValue)
            {
                query = query.Where(x => x.IsRawMaterial == request.IsRawMaterial.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var pattern = $"%{request.Search.Trim()}%";

                query = query.Where(x =>
                    EF.Functions.ILike(x.Code, pattern)
                    || EF.Functions.ILike(x.Name, pattern)
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
                .Select(MaterialTypeProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok(
                new PagedResult<MaterialTypeResponse>
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
        private static IQueryable<MaterialType> ApplySorting(IQueryable<MaterialType> query, MaterialTypeSortField sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (MaterialTypeSortField.SortOrder, SortDirection.Descending) => query.OrderByDescending(x => x.SortOrder).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (MaterialTypeSortField.Name, SortDirection.Ascending) => query.OrderBy(x => x.Name).ThenBy(x => x.Id),
                (MaterialTypeSortField.Name, SortDirection.Descending) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (MaterialTypeSortField.Code, SortDirection.Ascending) => query.OrderBy(x => x.Code).ThenBy(x => x.Id),
                (MaterialTypeSortField.Code, SortDirection.Descending) => query.OrderByDescending(x => x.Code).ThenByDescending(x => x.Id),
                (MaterialTypeSortField.IsActive, SortDirection.Ascending) => query.OrderBy(x => x.IsActive).ThenBy(x => x.SortOrder).ThenBy(x => x.Id),
                (MaterialTypeSortField.IsActive, SortDirection.Descending) => query.OrderByDescending(x => x.IsActive).ThenByDescending(x => x.SortOrder).ThenByDescending(x => x.Id),
                _ => query.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ThenBy(x => x.Id)
            };
        }
    }
}
