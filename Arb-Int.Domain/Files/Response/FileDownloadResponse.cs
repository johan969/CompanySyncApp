namespace Arb_Int.Domain.Files
{
    public sealed record FileDownloadResponse(
    string FileName,
    string ContentType,
    long FileSize,
    byte[] Data,
    bool DisplayInline);
}