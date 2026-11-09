namespace Arb_Int.Domain.Articles
{
    public enum ProductStatus
    {
        /// <summary>
        /// Produkten är under uppbyggnad och ska normalt inte vara synlig
        /// eller möjlig att sälja.
        /// </summary>
        Draft = 0,

        /// <summary>
        /// Produkten är publicerad och kommersiellt aktiv.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Produkten säljs inte längre men behålls för historik,
        /// spårbarhet och tidigare transaktioner.
        /// </summary>
        Discontinued = 2,

        /// <summary>
        /// Produkten är arkiverad och ska normalt inte visas i vanliga
        /// administrations- eller katalogvyer.
        /// </summary>
        Archived = 3
    }
}