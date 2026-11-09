namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Stabila maskinkoder för importerad och datadriven metadata.
    ///
    /// Dessa är inte begränsade till alger. Nya typer kan också skapas
    /// som vanliga databasrader utan kodändring.
    /// </summary>
    public static class CharacteristicTypeCodes
    {
        public const string OrganismForm = "organism-form";
        public const string BiologicalPart = "biological-part";

        public const string PotentialProductFormat = "potential-product-format";
        public const string CultivationEnvironment = "cultivation-environment";
        public const string MarketCharacteristic = "market-characteristic";
        public const string Application = "application";
        public const string ValueAddPotential = "value-add-potential";
        public const string PrimaryMarket = "primary-market";
        public const string SupplyModel = "supply-model";

        public const string ProductCategory = "product-category";
        public const string PackagingFormat = "packaging-format";
        public const string Certification = "certification";
        public const string IntendedUse = "intended-use";
    }
}