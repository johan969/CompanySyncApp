namespace Arb_Int.Domain.Email
{
    public class EmailMessageResponse
    {
        public string? EmailProvider { get; set; }
        public string? Subject { get; set; }
        public string? BodyText { get; set; }
        public string BodyHtml { get; set; } = null!;
    }
}