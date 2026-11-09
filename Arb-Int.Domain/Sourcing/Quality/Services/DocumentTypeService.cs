using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar document type reference-data med immutable code och optimistic concurrency.
    /// </summary>
    public sealed class DocumentTypeService : IDocumentTypeService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public DocumentTypeService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar en document type.
        /// </summary>
        public async Task<Result<DocumentTypeResponse>> GetByIdAsync(int documentTypeId, CancellationToken cancellationToken = default)
        {
            var response = await _db.Set<DocumentType>()
                .AsNoTracking()
                .Where(x => x.Id == documentTypeId)
                .Select(DocumentTypeProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                return Result.Fail<DocumentTypeResponse>(
                    new NotFoundError(
                        nameof(DocumentType),
                        documentTypeId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar document types.
        /// </summary>
        public async Task<Result<PagedResult<DocumentTypeResponse>>> QueryAsync(DocumentTypeQueryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<DocumentTypeResponse>>(
                    validationResult.Errors);
            }

            IQueryable<DocumentType> query = _db.Set<DocumentType>()
                .AsNoTracking();

            if (!request.IncludeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            if (request.Category.HasValue)
            {
                query = query.Where(x => x.Category == request.Category.Value);
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
                .Select(DocumentTypeProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok(
                new PagedResult<DocumentTypeResponse>
                {
                    Items = items,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                });
        }

        /// <summary>
        /// Skapar en document type och låter code fungera som stabil reference-identitet.
        /// </summary>
        public async Task<Result<DocumentTypeResponse>> CreateAsync(CreateDocumentTypeRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<DocumentTypeResponse>(
                    validationResult.Errors);
            }

            var code = request.Code.Trim();

            var duplicateExists = await _db.Set<DocumentType>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Code == code,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<DocumentTypeResponse>(
                    new ConflictError(
                        $"DocumentType code '{code}' already exists."));
            }

            var documentType = new DocumentType
            {
                Code = code,
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Category = request.Category,
                IsLotSpecificByDefault = request.IsLotSpecificByDefault,
                CanExpire = request.CanExpire,
                IsActive = true
            };

            _db.Set<DocumentType>()
                .Add(documentType);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception)
                when (IsUniqueViolation(exception))
            {
                return Result.Fail<DocumentTypeResponse>(
                    new ConflictError(
                        $"DocumentType code '{code}' already exists."));
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    documentType.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar metadata utan att ändra document typens code.
        /// </summary>
        public async Task<Result<DocumentTypeResponse>> UpdateAsync(int documentTypeId, UpdateDocumentTypeRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<DocumentTypeResponse>(
                    validationResult.Errors);
            }

            var documentType = await _db.Set<DocumentType>()
                .SingleOrDefaultAsync(
                    x => x.Id == documentTypeId,
                    cancellationToken);

            if (documentType is null)
            {
                return Result.Fail<DocumentTypeResponse>(
                    new NotFoundError(
                        nameof(DocumentType),
                        documentTypeId));
            }

            if (documentType.Name == request.Name.Trim()
                && documentType.Description == request.Description.Trim()
                && documentType.Category == request.Category
                && documentType.IsLotSpecificByDefault == request.IsLotSpecificByDefault
                && documentType.CanExpire == request.CanExpire)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        documentType.Id,
                        cancellationToken));
            }

            _db.Entry(documentType)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            documentType.Name = request.Name.Trim();
            documentType.Description = request.Description.Trim();
            documentType.Category = request.Category;
            documentType.IsLotSpecificByDefault = request.IsLotSpecificByDefault;
            documentType.CanExpire = request.CanExpire;

            var saveResult = await SaveChangesAsync(
                documentType.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<DocumentTypeResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    documentType.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Aktiverar eller inaktiverar reference-posten utan fysisk delete.
        /// </summary>
        public async Task<Result<DocumentTypeResponse>> SetActiveAsync(int documentTypeId, SetDocumentTypeActiveRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<DocumentTypeResponse>(
                    validationResult.Errors);
            }

            var documentType = await _db.Set<DocumentType>()
                .SingleOrDefaultAsync(
                    x => x.Id == documentTypeId,
                    cancellationToken);

            if (documentType is null)
            {
                return Result.Fail<DocumentTypeResponse>(
                    new NotFoundError(
                        nameof(DocumentType),
                        documentTypeId));
            }

            if (documentType.IsActive == request.IsActive)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        documentType.Id,
                        cancellationToken));
            }

            _db.Entry(documentType)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            documentType.IsActive = request.IsActive;

            var saveResult = await SaveChangesAsync(
                documentType.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<DocumentTypeResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    documentType.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Sparar en reference-dataändring och översätter stale RowVersion till concurrency-fel.
        /// </summary>
        private async Task<Result> SaveChangesAsync(int documentTypeId, CancellationToken cancellationToken)
        {
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail(
                    new ConcurrencyError(
                        nameof(DocumentType),
                        documentTypeId));
            }
            catch (DbUpdateException exception)
                when (IsUniqueViolation(exception))
            {
                return Result.Fail(
                    new ConflictError(
                        "Another DocumentType already uses the same Code."));
            }
        }

        /// <summary>
        /// Projekterar en document type direkt från databasen.
        /// </summary>
        private async Task<DocumentTypeResponse?> ProjectAsync(int documentTypeId, CancellationToken cancellationToken)
        {
            return await _db.Set<DocumentType>()
                .AsNoTracking()
                .Where(x => x.Id == documentTypeId)
                .Select(DocumentTypeProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad document type och kastar om den oväntat saknas.
        /// </summary>
        private async Task<DocumentTypeResponse> ProjectRequiredAsync(int documentTypeId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    documentTypeId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"DocumentType '{documentTypeId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Lägger på stabil sortering före paginering.
        /// </summary>
        private static IQueryable<DocumentType> ApplySorting(IQueryable<DocumentType> query, DocumentTypeSortField sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (DocumentTypeSortField.Name, SortDirection.Descending) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (DocumentTypeSortField.Code, SortDirection.Ascending) => query.OrderBy(x => x.Code).ThenBy(x => x.Id),
                (DocumentTypeSortField.Code, SortDirection.Descending) => query.OrderByDescending(x => x.Code).ThenByDescending(x => x.Id),
                (DocumentTypeSortField.Category, SortDirection.Ascending) => query.OrderBy(x => x.Category).ThenBy(x => x.Name).ThenBy(x => x.Id),
                (DocumentTypeSortField.Category, SortDirection.Descending) => query.OrderByDescending(x => x.Category).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                (DocumentTypeSortField.IsActive, SortDirection.Ascending) => query.OrderBy(x => x.IsActive).ThenBy(x => x.Name).ThenBy(x => x.Id),
                (DocumentTypeSortField.IsActive, SortDirection.Descending) => query.OrderByDescending(x => x.IsActive).ThenByDescending(x => x.Name).ThenByDescending(x => x.Id),
                _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id)
            };
        }

        /// <summary>
        /// Kontrollerar om databasfelet kommer från en unik constraint.
        /// </summary>
        private static bool IsUniqueViolation(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            };
        }
    }
}
