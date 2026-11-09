using Arb_Int.Domain.Articles.Projections;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Läser process type reference-data för uppslag och filtrering.
    /// </summary>
    public sealed class ProcessTypeService : IProcessTypeService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public ProcessTypeService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar en process type.
        /// </summary>
        public async Task<Result<ProcessTypeResponse>> GetByIdAsync(int processTypeId, CancellationToken cancellationToken = default)
        {
            var response = await _db.Set<ProcessType>()
                .AsNoTracking()
                .Where(x => x.Id == processTypeId)
                .Select(ProcessTypeProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                return Result.Fail<ProcessTypeResponse>(
                    new NotFoundError(
                        nameof(ProcessType),
                        processTypeId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar process types.
        /// </summary>
        public async Task<Result<PagedResult<ProcessTypeResponse>>> QueryAsync(ProcessTypeQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<ProcessTypeResponse>>(
                    validationResult.Errors);
            }

            IQueryable<ProcessType> query = _db.Set<ProcessType>()
                .AsNoTracking();

            if (!request.IncludeInactive)
            {
                query = query.Where(x => x.IsActive);
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
                .Select(ProcessTypeProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok(
                new PagedResult<ProcessTypeResponse>
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
        private static IQueryable<ProcessType> ApplySorting(IQueryable<ProcessType> query, ProcessTypeSortField sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (ProcessTypeSortField.SortOrder, SortDirection.Descending) => query.OrderByDescending(x => x.SortOrder).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (ProcessTypeSortField.Name, SortDirection.Ascending) => query.OrderBy(x => x.Name).ThenBy(x => x.Id),
                (ProcessTypeSortField.Name, SortDirection.Descending) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (ProcessTypeSortField.Code, SortDirection.Ascending) => query.OrderBy(x => x.Code).ThenBy(x => x.Id),
                (ProcessTypeSortField.Code, SortDirection.Descending) => query.OrderByDescending(x => x.Code).ThenByDescending(x => x.Id),
                (ProcessTypeSortField.IsActive, SortDirection.Ascending) => query.OrderBy(x => x.IsActive).ThenBy(x => x.SortOrder).ThenBy(x => x.Id),
                (ProcessTypeSortField.IsActive, SortDirection.Descending) => query.OrderByDescending(x => x.IsActive).ThenByDescending(x => x.SortOrder).ThenByDescending(x => x.Id),
                _ => query.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ThenBy(x => x.Id)
            };
        }
    }
}
