namespace Arb_Int.Domain.Files
{
    public class FileUploadRequest
    {
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Content type som klienten uppger.
        /// Används inte som sanningskälla.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Storlek som klienten uppger.
        /// Den faktiska storleken verifieras när strömmen läses.
        /// </summary>
        public long Length { get; set; }

        public Stream Content { get; set; } = Stream.Null;

        public int SortOrder { get; set; }
    }
}