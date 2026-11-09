
using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Economy;
using Arb_Int.Domain.Email;
using Arb_Int.Domain.Location;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Sourcing;
using Arb_Int.Domain.Trading;
using Arb_Int.Domain.Trading.Snapshots;
using Arb_Int.Domain.User;
using Arb_Int.Repository.System;
using Microsoft.Extensions.DependencyInjection;

namespace Arb_Int.Domain.DependencyInjection
{
    public static class ArbIntServiceDependencyInjection
    {
        /// <summary>
        /// Dependency injection för Arb-Int-domänens tjänster. Registrerar services och policies som används i domänen.
        /// </summary>
        public static IArbIntServiceBuilder AddArbIntServiceManager(this IServiceCollection services)
        {

            services.AddScoped<IArb_IntDbContextFactory, Arb_IntDbContextFactory>();

            // User
            services.AddScoped<IUserSettingsService, UserSettingsService>();
            services.AddScoped<IPersonUserAccountService, PersonUserAccountService>();
            services.AddScoped<IUserAdministrationService, UserAdministrationService>();
            services.AddScoped<IUserAuthContactPointResolver, UserAuthContactPointResolver>();
            services.AddScoped<IUserIdentitySynchronizationService, UserIdentitySynchronizationService>();
            services.AddScoped<IUserInvitationEmailService, UserInvitationEmailService>();

            services.AddScoped<IScheduledTaskService, ScheduledTaskService>();

            services.AddScoped<IEmailOutboxRepository, EmailOutboxRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IEmailTemplateVariantRepository, EmailTemplateVariantRepository>();
            services.AddScoped<IProviderEmailTemplateMappingRepository, ProviderEmailTemplateMappingRepository>();

            services.AddScoped<IEmailTemplatePolicyRegistry, EmailTemplatePolicyRegistry>();
            services.AddScoped<IEmailTemplateRenderer, SimpleTokenEmailTemplateRenderer>();
            services.AddScoped<IEmailTemplatePolicy, TwoStepVerificationEmailPolicy>();
            services.AddScoped<IEmailTemplatePolicy, EmailVerificationEmailPolicy>();

            // Common
            services.AddScoped<ICanonicalJsonService, CanonicalJsonService>();

            // Location
            services.AddScoped<IContinentService, ContinentService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IWaterService, WaterService>();

            services.AddScoped<ICurrencyService, CurrencyService>();

            // Articles - Reference
            services.AddScoped<IProcessTypeService, ProcessTypeService>();
            services.AddScoped<IMaterialTypeService, MaterialTypeService>();
            services.AddScoped<IMaterialService, MaterialService>();
            services.AddScoped<IBiologicalSourceService, BiologicalSourceService>();

            // Party
            services.AddScoped<IPartyService, PartyService>();
            services.AddScoped<IAddressKeyService, AddressKeyService>();
            services.AddScoped<IPartyAddressService, PartyAddressService>();
            services.AddScoped<IPartyContactPointService, PartyContactPointService>();
            services.AddScoped<IPartyExternalReferenceService, PartyExternalReferenceService>();
            services.AddSingleton<IPartyContactPointNormalizer, PartyContactPointNormalizer>();

            services.AddScoped<IOrganizationIdentifierNormalizer, OrganizationIdentifierNormalizer>();
            services.AddScoped<IOrganizationIdentifierService, OrganizationIdentifierService>();
            services.AddScoped<IOrganizationPersonService, OrganizationPersonService>();

            // CRM
            services.AddScoped<IBusinessRelationshipService, BusinessRelationshipService>();
            services.AddScoped<IBusinessRelationshipContactService, BusinessRelationshipContactService>();
            services.AddScoped<IBusinessRelationshipBiologicalSourceClaimService, BusinessRelationshipBiologicalSourceClaimService>();
            services.AddScoped<ICrmActivityService, CrmActivityService>();
            services.AddScoped<IBuyerDemandService, BuyerDemandService>();
            services.AddSingleton<IBiologicalSourceClaimNormalizer, BiologicalSourceClaimNormalizer>();

            // Trading - BuyerRfq
            services.AddScoped<IBuyerRfqService, BuyerRfqService>();
            services.AddScoped<IBuyerRfqLineService, BuyerRfqLineService>();
            services.AddScoped<IBuyerRfqRequirementService, BuyerRfqRequirementService>();
            services.AddScoped<IBuyerRfqDeliveryService, BuyerRfqDeliveryService>();
            services.AddScoped<IBuyerRfqRevisionSnapshotService, BuyerRfqRevisionSnapshotService>();
            services.AddScoped<IBuyerRfqService, BuyerRfqService>();

            // Trading - Matching
            services.AddScoped<IMatchingRuleSetService, MatchingRuleSetService>();
            services.AddScoped<IMatchingRunService, MatchingRunService>();
            services.AddScoped<IMatchingRunExecutionService, MatchingRunExecutionService>();
            services.AddScoped<ISupplySolutionCandidateService, SupplySolutionCandidateService>();
            services.AddScoped<IMatchingCandidateExecutionService, MatchingCandidateExecutionService>();
            services.AddScoped<IMatchingEvaluationExecutionService, MatchingEvaluationExecutionService>();
            services.AddScoped<IMatchingStepExecutionService, MatchingStepExecutionService>();
            services.AddScoped<ISupplySolutionCandidateSnapshotService, SupplySolutionCandidateSnapshotService>();
            services.AddScoped<ISupplySolutionCandidateFinalizationService, SupplySolutionCandidateFinalizationService>();
            services.AddScoped<ISupplySolutionCandidateLifecycleService, SupplySolutionCandidateLifecycleService>();

            // Trading - Moderation
            services.AddScoped<IMatchingModerationService, MatchingModerationService>();

            // Trading - SupplierQuote
            services.AddScoped<ISupplierQuoteReferenceValidator, SupplierQuoteReferenceValidator>();
            services.AddScoped<ISupplierQuoteRevisionSnapshotService, SupplierQuoteRevisionSnapshotService>();
            services.AddScoped<ISupplierQuoteService, SupplierQuoteService>();
            services.AddScoped<ISupplierQuoteLineService, SupplierQuoteLineService>();
            services.AddScoped<ISupplierQuoteLifecycleService, SupplierQuoteLifecycleService>();

            // Trading - BuyerProposal
            services.AddScoped<IBuyerProposalReferenceValidator, BuyerProposalReferenceValidator>();
            services.AddScoped<IBuyerProposalRevisionSnapshotService, BuyerProposalRevisionSnapshotService>();
            services.AddScoped<IBuyerProposalService, BuyerProposalService>();
            services.AddScoped<IBuyerProposalOptionService, BuyerProposalOptionService>();
            services.AddScoped<IBuyerProposalLineService, BuyerProposalLineService>();
            services.AddScoped<IBuyerProposalAcceptanceService, BuyerProposalAcceptanceService>();

            // Sourcing - Offering
            services.AddScoped<ISupplierMaterialOfferingNormalizer, SupplierMaterialOfferingNormalizer>();
            services.AddScoped<ISupplierMaterialOfferingService, SupplierMaterialOfferingService>();
            services.AddScoped<ISupplierMaterialOfferingBiologicalSourceService, SupplierMaterialOfferingBiologicalSourceService>();
            services.AddScoped<ISupplierMaterialOfferingOriginService, SupplierMaterialOfferingOriginService>();
            services.AddScoped<ISupplierMaterialOfferingCharacteristicService, SupplierMaterialOfferingCharacteristicService>();
            services.AddScoped<ISupplierMaterialOfferingEvidenceService, SupplierMaterialOfferingEvidenceService>();
            services.AddScoped<ISupplierOfferingAvailabilityService, SupplierOfferingAvailabilityService>();
            services.AddScoped<ISupplierOfferingSpecificationService, SupplierOfferingSpecificationService>();

            // Sourcing - ProcessCapabilities
            services.AddScoped<ISupplierProcessCapabilityNormalizer, SupplierProcessCapabilityNormalizer>();
            services.AddScoped<ISupplierProcessCapabilityService, SupplierProcessCapabilityService>();
            services.AddScoped<ISupplierProcessCapabilityBiologicalSourceScopeService, SupplierProcessCapabilityBiologicalSourceScopeService>();
            services.AddScoped<ISupplierProcessCapabilityEvidenceService, SupplierProcessCapabilityEvidenceService>();

            // Sourcing - InventoryLot
            services.AddScoped<IInventoryLotService, InventoryLotService>();
            services.AddScoped<IInventoryLotOriginService, InventoryLotOriginService>();
            services.AddScoped<IInventoryLotLinkService, InventoryLotLinkService>();

            // Sourcing - Quality
            services.AddScoped<ISupplyDocumentService, SupplyDocumentService>();
            services.AddScoped<IDocumentTypeService, DocumentTypeService>();
            services.AddScoped<ICertificationSchemeService, CertificationSchemeService>();
            services.AddScoped<ICertificationSchemeJurisdictionService, CertificationSchemeJurisdictionService>();
            services.AddScoped<ISupplyCertificationService, SupplyCertificationService>();
            services.AddScoped<ISupplyAnalysisService, SupplyAnalysisService>();
            services.AddScoped<ISupplyAnalysisResultService, SupplyAnalysisResultService>();

            services.AddSingleton(TimeProvider.System);
            return new ArbIntServiceBuilder(services);
        }
    }

    public class ArbIntServiceBuilder : IArbIntServiceBuilder
    {
        public IServiceCollection Services { get; }
        public ArbIntServiceBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}