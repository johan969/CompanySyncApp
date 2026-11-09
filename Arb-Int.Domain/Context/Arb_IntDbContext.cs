
using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Economy;
using Arb_Int.Domain.Email;
using Arb_Int.Domain.Location;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Sourcing;
using Arb_Int.Domain.Taxonomy;
using Arb_Int.Domain.Trading;
using Arb_Int.Domain.User;
using Arb_Int.Domain.Worker;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.Jwt.Core.Model;
using NetDevPack.Security.Jwt.Store.EntityFrameworkCore;


namespace Arb_Int.Domain
{
    public class Arb_IntDbContext : IdentityDbContext<
                ApplicationUser, ApplicationRole, Guid,
                ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
                ApplicationRoleClaim, ApplicationUserToken>, ISecurityKeyContext
    {
        public Arb_IntDbContext(DbContextOptions<Arb_IntDbContext> options) : base(options)
        {

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new NamedMaxLengthConvention(256));
            configurationBuilder.Conventions.Add(_ => new ActionTrackerMaxLengthConvention(256));
            configurationBuilder.Conventions.Add(_ => new DescribableMaxLengthConvention(4096));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Arb_IntDbContext).Assembly);
        }

        public DbSet<Continent> Continents => Set<Continent>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<CurrencyExchangeRateChange> CurrencyExchangeRateChanges => Set<CurrencyExchangeRateChange>();
        public DbSet<EmailOutboxMessage> EmailOutboxMessages => Set<EmailOutboxMessage>();
        public DbSet<EmailOutboxRecipient> EmailOutboxRecipients => Set<EmailOutboxRecipient>();
        public DbSet<Water> Waters => Set<Water>();
        public DbSet<ScheduledTask> ScheduledTasks => Set<ScheduledTask>();
        public DbSet<ScheduledTaskDefinition> ScheduledTaskDefinitions => Set<ScheduledTaskDefinition>();
        public DbSet<KeyMaterial> SecurityKeys { get; set; }
        public DbSet<WorkerHeartbeat> WorkerHeartbeats => Set<WorkerHeartbeat>();

        // Articles
        public DbSet<Taxon> Taxons => Set<Taxon>();
        public DbSet<BiologicalSource> BiologicalSources => Set<BiologicalSource>();
        public DbSet<BiologicalSourceName> BiologicalSourceNames => Set<BiologicalSourceName>();
        public DbSet<CharacteristicType> CharacteristicTypes => Set<CharacteristicType>();
        public DbSet<Characteristic> Characteristics => Set<Characteristic>();
        public DbSet<BiologicalSourceCharacteristic> BiologicalSourceCharacteristics => Set<BiologicalSourceCharacteristic>();
        public DbSet<MaterialCharacteristic> MaterialCharacteristics => Set<MaterialCharacteristic>();
        public DbSet<ConstituentType> ConstituentTypes => Set<ConstituentType>();
        public DbSet<Constituent> Constituents => Set<Constituent>();
        public DbSet<BiologicalSourceConstituent> BiologicalSourceConstituents => Set<BiologicalSourceConstituent>();
        public DbSet<MaterialConstituent> MaterialConstituents => Set<MaterialConstituent>();
        public DbSet<GeographicLocation> GeographicLocations => Set<GeographicLocation>();
        public DbSet<BiologicalSourceRegion> BiologicalSourceRegions => Set<BiologicalSourceRegion>();
        public DbSet<MaterialOrigin> MaterialOrigins => Set<MaterialOrigin>();
        public DbSet<ReferenceEntry> ReferenceEntries => Set<ReferenceEntry>();
        public DbSet<BiologicalSourceReference> BiologicalSourceReferences => Set<BiologicalSourceReference>();
        public DbSet<MaterialReference> MaterialReferences => Set<MaterialReference>();
        public DbSet<MaterialType> MaterialTypes => Set<MaterialType>();
        public DbSet<Material> Materials => Set<Material>();
        public DbSet<MaterialBiologicalSource> MaterialBiologicalSources => Set<MaterialBiologicalSource>();
        public DbSet<ProcessType> ProcessTypes => Set<ProcessType>();
        public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
        public DbSet<MaterialTransformation> MaterialTransformations => Set<MaterialTransformation>();
        public DbSet<MaterialTransformationInput> MaterialTransformationInputs => Set<MaterialTransformationInput>();
        public DbSet<MaterialTransformationOutput> MaterialTransformationOutputs => Set<MaterialTransformationOutput>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductIdentifier> ProductIdentifiers => Set<ProductIdentifier>();
        public DbSet<ProductCharacteristic> ProductCharacteristics => Set<ProductCharacteristic>();
        public DbSet<ProductReference> ProductReferences => Set<ProductReference>();

        // Parties
        public DbSet<Party> Parties => Set<Party>();
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<Person> People => Set<Person>();
        public DbSet<OrganizationIdentifier> OrganizationIdentifiers => Set<OrganizationIdentifier>();
        public DbSet<PartyExternalReference> PartyExternalReferences => Set<PartyExternalReference>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<PartyAddress> PartyAddresses => Set<PartyAddress>();
        public DbSet<PartyContactPoint> PartyContactPoints => Set<PartyContactPoint>();
        public DbSet<OrganizationPerson> OrganizationPeople => Set<OrganizationPerson>();
        public DbSet<ContactFunctionType> ContactFunctionTypes => Set<ContactFunctionType>();
        public DbSet<OrganizationPersonFunction> OrganizationPersonFunctions => Set<OrganizationPersonFunction>();
        public DbSet<PersonUserAccount> PersonUserAccounts => Set<PersonUserAccount>();

        // CRM
        public DbSet<BusinessRelationshipType> BusinessRelationshipTypes => Set<BusinessRelationshipType>();
        public DbSet<BusinessRelationshipStage> BusinessRelationshipStages => Set<BusinessRelationshipStage>();
        public DbSet<BusinessRelationship> BusinessRelationships => Set<BusinessRelationship>();
        public DbSet<BusinessRelationshipContact> BusinessRelationshipContacts => Set<BusinessRelationshipContact>();
        public DbSet<BusinessRelationshipStageHistory> BusinessRelationshipStageHistory => Set<BusinessRelationshipStageHistory>();
        public DbSet<BusinessRelationshipBiologicalSourceClaim> BusinessRelationshipBiologicalSourceClaims => Set<BusinessRelationshipBiologicalSourceClaim>();
        public DbSet<BusinessRelationshipBiologicalSourceClaimEvidence> BusinessRelationshipBiologicalSourceClaimEvidence => Set<BusinessRelationshipBiologicalSourceClaimEvidence>();
        public DbSet<BusinessRelationshipBiologicalSourceClaimRegion> BusinessRelationshipBiologicalSourceClaimRegions => Set<BusinessRelationshipBiologicalSourceClaimRegion>();
        public DbSet<CrmActivity> CrmActivities => Set<CrmActivity>();
        public DbSet<CrmActivityParticipant> CrmActivityParticipants => Set<CrmActivityParticipant>();
        public DbSet<BuyerDemand> BuyerDemands => Set<BuyerDemand>();
        public DbSet<BuyerDemandScope> BuyerDemandScopes => Set<BuyerDemandScope>();
        public DbSet<BuyerDemandEvidence> BuyerDemandEvidence => Set<BuyerDemandEvidence>();
        public DbSet<BuyerDemandStateHistory> BuyerDemandStateHistory => Set<BuyerDemandStateHistory>();

        // Supplier offerings and capabilities
        public DbSet<SupplierMaterialOffering> SupplierMaterialOfferings => Set<SupplierMaterialOffering>();
        public DbSet<SupplierMaterialOfferingBiologicalSource> SupplierMaterialOfferingBiologicalSources => Set<SupplierMaterialOfferingBiologicalSource>();
        public DbSet<SupplierMaterialOfferingOrigin> SupplierMaterialOfferingOrigins => Set<SupplierMaterialOfferingOrigin>();
        public DbSet<SupplierMaterialOfferingCharacteristic> SupplierMaterialOfferingCharacteristics => Set<SupplierMaterialOfferingCharacteristic>();
        public DbSet<SupplierMaterialOfferingEvidence> SupplierMaterialOfferingEvidence => Set<SupplierMaterialOfferingEvidence>();
        public DbSet<SupplierProcessCapability> SupplierProcessCapabilities => Set<SupplierProcessCapability>();
        public DbSet<SupplierProcessCapabilityBiologicalSourceScope> SupplierProcessCapabilityBiologicalSourceScopes => Set<SupplierProcessCapabilityBiologicalSourceScope>();
        public DbSet<SupplierProcessCapabilityEvidence> SupplierProcessCapabilityEvidence => Set<SupplierProcessCapabilityEvidence>();
        public DbSet<SupplierOfferingAvailability> SupplierOfferingAvailabilities => Set<SupplierOfferingAvailability>();
        public DbSet<SupplierOfferingSpecification> SupplierOfferingSpecifications => Set<SupplierOfferingSpecification>();

        // Supply quality
        public DbSet<CertificationScheme> CertificationSchemes => Set<CertificationScheme>();
        public DbSet<CertificationSchemeJurisdiction> CertificationSchemeJurisdictions => Set<CertificationSchemeJurisdiction>();
        public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
        public DbSet<SupplyCertification> SupplyCertifications => Set<SupplyCertification>();
        public DbSet<SupplyDocument> SupplyDocuments => Set<SupplyDocument>();
        public DbSet<SupplyAnalysis> SupplyAnalyses => Set<SupplyAnalysis>();
        public DbSet<SupplyAnalysisResult> SupplyAnalysisResults => Set<SupplyAnalysisResult>();

        // Supply inventory
        public DbSet<InventoryLot> InventoryLots => Set<InventoryLot>();
        public DbSet<InventoryLotOrigin> InventoryLotOrigins => Set<InventoryLotOrigin>();
        public DbSet<InventoryLotLink> InventoryLotLinks => Set<InventoryLotLink>();

        // Trading reference data
        public DbSet<IncotermRule> IncotermRules => Set<IncotermRule>();
        public DbSet<ShipmentMilestoneType> ShipmentMilestoneTypes => Set<ShipmentMilestoneType>();
        public DbSet<ExternalSystem> ExternalSystems => Set<ExternalSystem>();

        // RFQ
        public DbSet<BuyerRfq> BuyerRfqs => Set<BuyerRfq>();
        public DbSet<BuyerRfqRevision> BuyerRfqRevisions => Set<BuyerRfqRevision>();
        public DbSet<BuyerRfqLine> BuyerRfqLines => Set<BuyerRfqLine>();
        public DbSet<BuyerRfqLineMaterialOption> BuyerRfqLineMaterialOptions => Set<BuyerRfqLineMaterialOption>();
        public DbSet<BuyerRfqDelivery> BuyerRfqDeliveries => Set<BuyerRfqDelivery>();
        public DbSet<BuyerRfqDeliveryLineAllocation> BuyerRfqDeliveryLineAllocations => Set<BuyerRfqDeliveryLineAllocation>();
        public DbSet<BuyerRfqRequirement> BuyerRfqRequirements => Set<BuyerRfqRequirement>();

        // Matching
        public DbSet<MatchingRuleSet> MatchingRuleSets => Set<MatchingRuleSet>();
        public DbSet<MatchingRuleSetVersion> MatchingRuleSetVersions => Set<MatchingRuleSetVersion>();
        public DbSet<MatchingRun> MatchingRuns => Set<MatchingRun>();
        public DbSet<MatchingRunLine> MatchingRunLines => Set<MatchingRunLine>();
        public DbSet<SupplySolutionCandidate> SupplySolutionCandidates => Set<SupplySolutionCandidate>();
        public DbSet<SupplySolutionLine> SupplySolutionLines => Set<SupplySolutionLine>();
        public DbSet<SupplyAllocation> SupplyAllocations => Set<SupplyAllocation>();
        public DbSet<SupplySolutionStep> SupplySolutionSteps => Set<SupplySolutionStep>();
        public DbSet<SupplySolutionStepDependency> SupplySolutionStepDependencies => Set<SupplySolutionStepDependency>();
        public DbSet<RequirementEvaluation> RequirementEvaluations => Set<RequirementEvaluation>();
        public DbSet<ScoreContribution> ScoreContributions => Set<ScoreContribution>();
        public DbSet<CapacityEvaluation> CapacityEvaluations => Set<CapacityEvaluation>();
        public DbSet<MatchingModerationDecision> MatchingModerationDecisions => Set<MatchingModerationDecision>();

        // Quotes and proposals
        public DbSet<SupplierQuote> SupplierQuotes => Set<SupplierQuote>();
        public DbSet<SupplierQuoteRevision> SupplierQuoteRevisions => Set<SupplierQuoteRevision>();
        public DbSet<SupplierQuoteLine> SupplierQuoteLines => Set<SupplierQuoteLine>();
        public DbSet<BuyerProposal> BuyerProposals => Set<BuyerProposal>();
        public DbSet<BuyerProposalRevision> BuyerProposalRevisions => Set<BuyerProposalRevision>();
        public DbSet<BuyerProposalOption> BuyerProposalOptions => Set<BuyerProposalOption>();
        public DbSet<BuyerProposalLine> BuyerProposalLines => Set<BuyerProposalLine>();
        public DbSet<BuyerProposalAcceptance> BuyerProposalAcceptances => Set<BuyerProposalAcceptance>();
        public DbSet<BuyerProposalAcceptanceLine> BuyerProposalAcceptanceLines => Set<BuyerProposalAcceptanceLine>();

        // Orders and responsibility transfers
        public DbSet<BuyerSalesOrder> BuyerSalesOrders => Set<BuyerSalesOrder>();
        public DbSet<BuyerSalesOrderLine> BuyerSalesOrderLines => Set<BuyerSalesOrderLine>();
        public DbSet<BuyerSalesOrderDelivery> BuyerSalesOrderDeliveries => Set<BuyerSalesOrderDelivery>();
        public DbSet<BuyerSalesOrderDeliveryLine> BuyerSalesOrderDeliveryLines => Set<BuyerSalesOrderDeliveryLine>();
        public DbSet<BuyerSalesOrderStatusHistory> BuyerSalesOrderStatusHistory => Set<BuyerSalesOrderStatusHistory>();
        public DbSet<SupplierPurchaseOrder> SupplierPurchaseOrders => Set<SupplierPurchaseOrder>();
        public DbSet<SupplierPurchaseOrderLine> SupplierPurchaseOrderLines => Set<SupplierPurchaseOrderLine>();
        public DbSet<SupplierPurchaseOrderStatusHistory> SupplierPurchaseOrderStatusHistory => Set<SupplierPurchaseOrderStatusHistory>();
        public DbSet<SalesPurchaseAllocation> SalesPurchaseAllocations => Set<SalesPurchaseAllocation>();
        public DbSet<GoodsResponsibilityTransfer> GoodsResponsibilityTransfers => Set<GoodsResponsibilityTransfer>();

        // Logistics
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<ShipmentLeg> ShipmentLegs => Set<ShipmentLeg>();
        public DbSet<ShipmentOrderLineAllocation> ShipmentOrderLineAllocations => Set<ShipmentOrderLineAllocation>();
        public DbSet<ShipmentLotAllocation> ShipmentLotAllocations => Set<ShipmentLotAllocation>();
        public DbSet<ShipmentMilestone> ShipmentMilestones => Set<ShipmentMilestone>();
        public DbSet<ShipmentTrackingEvent> ShipmentTrackingEvents => Set<ShipmentTrackingEvent>();
        public DbSet<CarrierBooking> CarrierBookings => Set<CarrierBooking>();
        public DbSet<ShipmentDocument> ShipmentDocuments => Set<ShipmentDocument>();
        public DbSet<CustomsReference> CustomsReferences => Set<CustomsReference>();

        // Invoicing and external systems
        public DbSet<CommercialInvoice> CommercialInvoices => Set<CommercialInvoice>();
        public DbSet<CommercialInvoiceLine> CommercialInvoiceLines => Set<CommercialInvoiceLine>();
        public DbSet<ExternalAccountingReference> ExternalAccountingReferences => Set<ExternalAccountingReference>();
        public DbSet<InvoiceSyncAttempt> InvoiceSyncAttempts => Set<InvoiceSyncAttempt>();
        public DbSet<PaymentStatusSnapshot> PaymentStatusSnapshots => Set<PaymentStatusSnapshot>();
        public DbSet<ReconciliationTask> ReconciliationTasks => Set<ReconciliationTask>();

        // Moderated communication
        public DbSet<DisclosurePackage> DisclosurePackages => Set<DisclosurePackage>();
        public DbSet<DisclosurePackageRevision> DisclosurePackageRevisions => Set<DisclosurePackageRevision>();
        public DbSet<DisclosurePackageItem> DisclosurePackageItems => Set<DisclosurePackageItem>();
        public DbSet<ExternalConversation> ExternalConversations => Set<ExternalConversation>();
        public DbSet<ExternalMessage> ExternalMessages => Set<ExternalMessage>();
        public DbSet<ExternalMessageAttachment> ExternalMessageAttachments => Set<ExternalMessageAttachment>();

        // Execution checkpoints
        public DbSet<TradeExecutionCheckpointType> TradeExecutionCheckpointTypes => Set<TradeExecutionCheckpointType>();
        public DbSet<TradeExecutionCheckpoint> TradeExecutionCheckpoints => Set<TradeExecutionCheckpoint>();
    }
}